using Dapper;
using ErpCore.Domain.Entities.Accounting;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Accounting;

/// <summary>
/// 財務交易 Repository 實作 (SYSN210)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class FinancialTransactionRepository : BaseRepository, IFinancialTransactionRepository
{
    public FinancialTransactionRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<FinancialTransaction?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM FinancialTransactions 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<FinancialTransaction>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢財務交易失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<FinancialTransaction?> GetByTxnNoAsync(string txnNo)
    {
        try
        {
            const string sql = @"
                SELECT * FROM FinancialTransactions 
                WHERE TxnNo = @TxnNo";

            return await QueryFirstOrDefaultAsync<FinancialTransaction>(sql, new { TxnNo = txnNo });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢財務交易失敗: {txnNo}", ex);
            throw;
        }
    }

    public async Task<PagedResult<FinancialTransaction>> QueryAsync(FinancialTransactionQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM FinancialTransactions
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.TxnNo))
            {
                sql += " AND TxnNo LIKE @TxnNo";
                parameters.Add("TxnNo", $"%{query.TxnNo}%");
            }

            if (query.TxnDateFrom.HasValue)
            {
                sql += " AND TxnDate >= @TxnDateFrom";
                parameters.Add("TxnDateFrom", query.TxnDateFrom.Value);
            }

            if (query.TxnDateTo.HasValue)
            {
                sql += " AND TxnDate <= @TxnDateTo";
                parameters.Add("TxnDateTo", query.TxnDateTo.Value);
            }

            if (!string.IsNullOrEmpty(query.TxnType))
            {
                sql += " AND TxnType = @TxnType";
                parameters.Add("TxnType", query.TxnType);
            }

            if (!string.IsNullOrEmpty(query.StypeId))
            {
                sql += " AND StypeId = @StypeId";
                parameters.Add("StypeId", query.StypeId);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "TxnDate" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 查詢總數
            var countSql = sql.Replace("SELECT *", "SELECT COUNT(*)");
            var totalCount = await QueryFirstOrDefaultAsync<int>(countSql, parameters);

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = (await QueryAsync<FinancialTransaction>(sql, parameters)).ToList();

            return new PagedResult<FinancialTransaction>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢財務交易列表失敗", ex);
            throw;
        }
    }

    public async Task<FinancialTransaction> CreateAsync(FinancialTransaction transaction)
    {
        try
        {
            const string sql = @"
                INSERT INTO FinancialTransactions 
                (TxnNo, TxnDate, TxnType, StypeId, Dc, Amount, Description, Status, Verifier, VerifyDate, PostedBy, PostedDate, Notes, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@TxnNo, @TxnDate, @TxnType, @StypeId, @Dc, @Amount, @Description, @Status, @Verifier, @VerifyDate, @PostedBy, @PostedDate, @Notes, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, transaction);
            transaction.TKey = tKey;

            return transaction;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增財務交易失敗: {transaction.TxnNo}", ex);
            throw;
        }
    }

    public async Task<FinancialTransaction> UpdateAsync(FinancialTransaction transaction)
    {
        try
        {
            const string sql = @"
                UPDATE FinancialTransactions 
                SET TxnDate = @TxnDate, TxnType = @TxnType, StypeId = @StypeId, Dc = @Dc, 
                    Amount = @Amount, Description = @Description, Status = @Status, 
                    Verifier = @Verifier, VerifyDate = @VerifyDate, PostedBy = @PostedBy, 
                    PostedDate = @PostedDate, Notes = @Notes, UpdatedBy = @UpdatedBy, UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, transaction);
            return transaction;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改財務交易失敗: {transaction.TxnNo}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"DELETE FROM FinancialTransactions WHERE TKey = @TKey";
            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除財務交易失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string txnNo)
    {
        try
        {
            const string sql = @"SELECT COUNT(*) FROM FinancialTransactions WHERE TxnNo = @TxnNo";
            var count = await QueryFirstOrDefaultAsync<int>(sql, new { TxnNo = txnNo });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查交易單號是否存在失敗: {txnNo}", ex);
            throw;
        }
    }

    public async Task<BalanceCheckResult> CheckBalanceAsync(List<FinancialTransaction> transactions)
    {
        try
        {
            var debitTotal = transactions.Where(t => t.Dc == "D").Sum(t => t.Amount);
            var creditTotal = transactions.Where(t => t.Dc == "C").Sum(t => t.Amount);
            var difference = Math.Abs(debitTotal - creditTotal);

            return new BalanceCheckResult
            {
                IsBalanced = difference < 0.01m, // 允許小數點誤差
                DebitTotal = debitTotal,
                CreditTotal = creditTotal,
                Difference = difference
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("檢查借貸平衡失敗", ex);
            throw;
        }
    }
}

