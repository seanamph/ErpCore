using System.Data;
using Dapper;
using ErpCore.Domain.Entities.CustomerInvoice;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.CustomerInvoice;

/// <summary>
/// 總帳資料 Repository 實作 (SYS2000 - 總帳資料維護)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class LedgerDataRepository : BaseRepository, ILedgerDataRepository
{
    public LedgerDataRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<GeneralLedger?> GetByIdAsync(string ledgerId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM GeneralLedger 
                WHERE LedgerId = @LedgerId";

            return await QueryFirstOrDefaultAsync<GeneralLedger>(sql, new { LedgerId = ledgerId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢總帳失敗: {ledgerId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<GeneralLedger>> QueryAsync(GeneralLedgerQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM GeneralLedger
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.LedgerId))
            {
                sql += " AND LedgerId LIKE @LedgerId";
                parameters.Add("LedgerId", $"%{query.LedgerId}%");
            }

            if (!string.IsNullOrEmpty(query.AccountId))
            {
                sql += " AND AccountId = @AccountId";
                parameters.Add("AccountId", query.AccountId);
            }

            if (!string.IsNullOrEmpty(query.Period))
            {
                sql += " AND Period = @Period";
                parameters.Add("Period", query.Period);
            }

            if (query.LedgerDateFrom.HasValue)
            {
                sql += " AND LedgerDate >= @LedgerDateFrom";
                parameters.Add("LedgerDateFrom", query.LedgerDateFrom);
            }

            if (query.LedgerDateTo.HasValue)
            {
                sql += " AND LedgerDate <= @LedgerDateTo";
                parameters.Add("LedgerDateTo", query.LedgerDateTo);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 排序
            var sortField = query.SortField ?? "LedgerDate";
            var sortOrder = query.SortOrder ?? "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<GeneralLedger>(sql, parameters);
            var totalCount = await GetCountAsync(query);

            return new PagedResult<GeneralLedger>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢總帳列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(GeneralLedgerQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM GeneralLedger
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.LedgerId))
            {
                sql += " AND LedgerId LIKE @LedgerId";
                parameters.Add("LedgerId", $"%{query.LedgerId}%");
            }

            if (!string.IsNullOrEmpty(query.AccountId))
            {
                sql += " AND AccountId = @AccountId";
                parameters.Add("AccountId", query.AccountId);
            }

            if (!string.IsNullOrEmpty(query.Period))
            {
                sql += " AND Period = @Period";
                parameters.Add("Period", query.Period);
            }

            if (query.LedgerDateFrom.HasValue)
            {
                sql += " AND LedgerDate >= @LedgerDateFrom";
                parameters.Add("LedgerDateFrom", query.LedgerDateFrom);
            }

            if (query.LedgerDateTo.HasValue)
            {
                sql += " AND LedgerDate <= @LedgerDateTo";
                parameters.Add("LedgerDateTo", query.LedgerDateTo);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            return await ExecuteScalarAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢總帳總數失敗", ex);
            throw;
        }
    }

    public async Task<GeneralLedger> CreateAsync(GeneralLedger ledger)
    {
        try
        {
            ledger.CreatedAt = DateTime.Now;
            ledger.UpdatedAt = DateTime.Now;

            const string insertSql = @"
                INSERT INTO GeneralLedger 
                (LedgerId, LedgerDate, AccountId, VoucherNo, Description, DebitAmount, CreditAmount, Balance,
                 CurrencyId, Period, Status, Memo, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@LedgerId, @LedgerDate, @AccountId, @VoucherNo, @Description, @DebitAmount, @CreditAmount, @Balance,
                 @CurrencyId, @Period, @Status, @Memo, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            using var connection = _connectionFactory.CreateConnection();
            var tKey = await connection.QuerySingleAsync<long>(insertSql, ledger);
            ledger.TKey = tKey;

            _logger.LogInfo($"建立總帳成功: {ledger.LedgerId}");
            return ledger;
        }
        catch (Exception ex)
        {
            _logger.LogError($"建立總帳失敗: {ledger.LedgerId}", ex);
            throw;
        }
    }

    public async Task<GeneralLedger> UpdateAsync(GeneralLedger ledger)
    {
        try
        {
            ledger.UpdatedAt = DateTime.Now;

            const string updateSql = @"
                UPDATE GeneralLedger SET
                    LedgerDate = @LedgerDate,
                    AccountId = @AccountId,
                    VoucherNo = @VoucherNo,
                    Description = @Description,
                    DebitAmount = @DebitAmount,
                    CreditAmount = @CreditAmount,
                    Balance = @Balance,
                    CurrencyId = @CurrencyId,
                    Period = @Period,
                    Status = @Status,
                    Memo = @Memo,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE LedgerId = @LedgerId";

            await ExecuteAsync(updateSql, ledger);
            _logger.LogInfo($"更新總帳成功: {ledger.LedgerId}");
            return ledger;
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新總帳失敗: {ledger.LedgerId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string ledgerId)
    {
        try
        {
            const string sql = "DELETE FROM GeneralLedger WHERE LedgerId = @LedgerId AND Status = 'DRAFT'";
            await ExecuteAsync(sql, new { LedgerId = ledgerId });
            _logger.LogInfo($"刪除總帳成功: {ledgerId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除總帳失敗: {ledgerId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string ledgerId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM GeneralLedger 
                WHERE LedgerId = @LedgerId";

            var count = await ExecuteScalarAsync<int>(sql, new { LedgerId = ledgerId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查總帳編號是否存在失敗: {ledgerId}", ex);
            throw;
        }
    }

    public async Task UpdateStatusAsync(string ledgerId, string status)
    {
        try
        {
            const string sql = @"
                UPDATE GeneralLedger 
                SET Status = @Status, UpdatedAt = GETDATE()
                WHERE LedgerId = @LedgerId";

            await ExecuteAsync(sql, new { LedgerId = ledgerId, Status = status });
            _logger.LogInfo($"更新總帳狀態成功: LedgerId={ledgerId}, Status={status}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新總帳狀態失敗: LedgerId={ledgerId}", ex);
            throw;
        }
    }

    public async Task<AccountBalance?> GetAccountBalanceAsync(string accountId, string period)
    {
        try
        {
            const string sql = @"
                SELECT * FROM AccountBalances 
                WHERE AccountId = @AccountId AND Period = @Period";

            return await QueryFirstOrDefaultAsync<AccountBalance>(sql, new { AccountId = accountId, Period = period });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢會計科目餘額失敗: AccountId={accountId}, Period={period}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<AccountBalance>> GetAccountBalancesAsync(AccountBalanceQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM AccountBalances
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.AccountId))
            {
                sql += " AND AccountId = @AccountId";
                parameters.Add("AccountId", query.AccountId);
            }

            if (!string.IsNullOrEmpty(query.Period))
            {
                sql += " AND Period = @Period";
                parameters.Add("Period", query.Period);
            }

            if (query.PeriodFrom.HasValue)
            {
                var periodFrom = query.PeriodFrom.Value.ToString("yyyyMM");
                sql += " AND Period >= @PeriodFrom";
                parameters.Add("PeriodFrom", periodFrom);
            }

            if (query.PeriodTo.HasValue)
            {
                var periodTo = query.PeriodTo.Value.ToString("yyyyMM");
                sql += " AND Period <= @PeriodTo";
                parameters.Add("PeriodTo", periodTo);
            }

            sql += " ORDER BY Period DESC, AccountId";

            return await QueryAsync<AccountBalance>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢會計科目餘額列表失敗", ex);
            throw;
        }
    }

    public async Task UpdateAccountBalanceAsync(AccountBalance balance)
    {
        try
        {
            balance.UpdatedAt = DateTime.Now;

            // 使用 MERGE 語句，如果不存在則新增，存在則更新
            const string mergeSql = @"
                MERGE AccountBalances AS target
                USING (SELECT @AccountId AS AccountId, @Period AS Period) AS source
                ON target.AccountId = source.AccountId AND target.Period = source.Period
                WHEN MATCHED THEN
                    UPDATE SET
                        OpeningBalance = @OpeningBalance,
                        DebitAmount = @DebitAmount,
                        CreditAmount = @CreditAmount,
                        ClosingBalance = @ClosingBalance,
                        UpdatedBy = @UpdatedBy,
                        UpdatedAt = @UpdatedAt
                WHEN NOT MATCHED THEN
                    INSERT (AccountId, Period, OpeningBalance, DebitAmount, CreditAmount, ClosingBalance, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                    VALUES (@AccountId, @Period, @OpeningBalance, @DebitAmount, @CreditAmount, @ClosingBalance, @CreatedBy, GETDATE(), @UpdatedBy, @UpdatedAt);";

            await ExecuteAsync(mergeSql, balance);
            _logger.LogInfo($"更新會計科目餘額成功: AccountId={balance.AccountId}, Period={balance.Period}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新會計科目餘額失敗: AccountId={balance.AccountId}, Period={balance.Period}", ex);
            throw;
        }
    }
}

