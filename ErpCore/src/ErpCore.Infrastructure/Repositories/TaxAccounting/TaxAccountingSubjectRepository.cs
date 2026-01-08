using Dapper;
using ErpCore.Domain.Entities.Accounting;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Infrastructure.Repositories.Accounting;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.TaxAccounting;

/// <summary>
/// 會計科目 Repository 實作 (SYST111-SYST11A)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class TaxAccountingSubjectRepository : BaseRepository, ITaxAccountingSubjectRepository
{
    public TaxAccountingSubjectRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<AccountSubject?> GetByIdAsync(string stypeId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM AccountSubjects 
                WHERE StypeId = @StypeId";

            return await QueryFirstOrDefaultAsync<AccountSubject>(sql, new { StypeId = stypeId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢會計科目失敗: {stypeId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<AccountSubject>> QueryAsync(AccountSubjectQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM AccountSubjects
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.StypeId))
            {
                sql += " AND StypeId LIKE @StypeId";
                parameters.Add("StypeId", $"%{query.StypeId}%");
            }

            if (!string.IsNullOrEmpty(query.StypeName))
            {
                sql += " AND StypeName LIKE @StypeName";
                parameters.Add("StypeName", $"%{query.StypeName}%");
            }

            if (!string.IsNullOrEmpty(query.Dc))
            {
                sql += " AND Dc = @Dc";
                parameters.Add("Dc", query.Dc);
            }

            if (!string.IsNullOrEmpty(query.LedgerMd))
            {
                sql += " AND LedgerMd = @LedgerMd";
                parameters.Add("LedgerMd", query.LedgerMd);
            }

            if (!string.IsNullOrEmpty(query.VoucherType))
            {
                sql += " AND VoucherType = @VoucherType";
                parameters.Add("VoucherType", query.VoucherType);
            }

            if (!string.IsNullOrEmpty(query.BudgetYn))
            {
                sql += " AND BudgetYn = @BudgetYn";
                parameters.Add("BudgetYn", query.BudgetYn);
            }

            if (!string.IsNullOrEmpty(query.StypeClass))
            {
                sql += " AND StypeClass = @StypeClass";
                parameters.Add("StypeClass", query.StypeClass);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "StypeId" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<AccountSubject>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM AccountSubjects
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.StypeId))
            {
                countSql += " AND StypeId LIKE @StypeId";
                countParameters.Add("StypeId", $"%{query.StypeId}%");
            }
            if (!string.IsNullOrEmpty(query.StypeName))
            {
                countSql += " AND StypeName LIKE @StypeName";
                countParameters.Add("StypeName", $"%{query.StypeName}%");
            }
            if (!string.IsNullOrEmpty(query.Dc))
            {
                countSql += " AND Dc = @Dc";
                countParameters.Add("Dc", query.Dc);
            }
            if (!string.IsNullOrEmpty(query.LedgerMd))
            {
                countSql += " AND LedgerMd = @LedgerMd";
                countParameters.Add("LedgerMd", query.LedgerMd);
            }
            if (!string.IsNullOrEmpty(query.VoucherType))
            {
                countSql += " AND VoucherType = @VoucherType";
                countParameters.Add("VoucherType", query.VoucherType);
            }
            if (!string.IsNullOrEmpty(query.BudgetYn))
            {
                countSql += " AND BudgetYn = @BudgetYn";
                countParameters.Add("BudgetYn", query.BudgetYn);
            }
            if (!string.IsNullOrEmpty(query.StypeClass))
            {
                countSql += " AND StypeClass = @StypeClass";
                countParameters.Add("StypeClass", query.StypeClass);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<AccountSubject>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢會計科目列表失敗", ex);
            throw;
        }
    }

    public async Task<AccountSubject> CreateAsync(AccountSubject accountSubject)
    {
        try
        {
            const string sql = @"
                INSERT INTO AccountSubjects (
                    StypeId, StypeName, StypeNameE, Dc, LedgerMd, MtypeId, AbatYn,
                    VoucherType, BudgetYn, OrgYn, ExpYear, ResiValue, DepreLid, AccudepreLid,
                    StypeYn, IfrsStypeId, RocStypeId, SapStypeId, StypeClass, StypeOrder,
                    Amt0, Amt1, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup
                ) VALUES (
                    @StypeId, @StypeName, @StypeNameE, @Dc, @LedgerMd, @MtypeId, @AbatYn,
                    @VoucherType, @BudgetYn, @OrgYn, @ExpYear, @ResiValue, @DepreLid, @AccudepreLid,
                    @StypeYn, @IfrsStypeId, @RocStypeId, @SapStypeId, @StypeClass, @StypeOrder,
                    @Amt0, @Amt1, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup
                );
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await QuerySingleAsync<long>(sql, new
            {
                accountSubject.StypeId,
                accountSubject.StypeName,
                accountSubject.StypeNameE,
                accountSubject.Dc,
                accountSubject.LedgerMd,
                accountSubject.MtypeId,
                accountSubject.AbatYn,
                accountSubject.VoucherType,
                accountSubject.BudgetYn,
                accountSubject.OrgYn,
                accountSubject.ExpYear,
                accountSubject.ResiValue,
                accountSubject.DepreLid,
                accountSubject.AccudepreLid,
                accountSubject.StypeYn,
                accountSubject.IfrsStypeId,
                accountSubject.RocStypeId,
                accountSubject.SapStypeId,
                accountSubject.StypeClass,
                accountSubject.StypeOrder,
                accountSubject.Amt0,
                accountSubject.Amt1,
                accountSubject.CreatedBy,
                accountSubject.CreatedAt,
                accountSubject.UpdatedBy,
                accountSubject.UpdatedAt,
                accountSubject.CreatedPriority,
                accountSubject.CreatedGroup
            });

            accountSubject.TKey = tKey;
            return accountSubject;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增會計科目失敗: {accountSubject.StypeId}", ex);
            throw;
        }
    }

    public async Task<AccountSubject> UpdateAsync(AccountSubject accountSubject)
    {
        try
        {
            const string sql = @"
                UPDATE AccountSubjects SET
                    StypeName = @StypeName,
                    StypeNameE = @StypeNameE,
                    Dc = @Dc,
                    LedgerMd = @LedgerMd,
                    MtypeId = @MtypeId,
                    AbatYn = @AbatYn,
                    VoucherType = @VoucherType,
                    BudgetYn = @BudgetYn,
                    OrgYn = @OrgYn,
                    ExpYear = @ExpYear,
                    ResiValue = @ResiValue,
                    DepreLid = @DepreLid,
                    AccudepreLid = @AccudepreLid,
                    StypeYn = @StypeYn,
                    IfrsStypeId = @IfrsStypeId,
                    RocStypeId = @RocStypeId,
                    SapStypeId = @SapStypeId,
                    StypeClass = @StypeClass,
                    StypeOrder = @StypeOrder,
                    Amt0 = @Amt0,
                    Amt1 = @Amt1,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE StypeId = @StypeId";

            await ExecuteAsync(sql, new
            {
                accountSubject.StypeId,
                accountSubject.StypeName,
                accountSubject.StypeNameE,
                accountSubject.Dc,
                accountSubject.LedgerMd,
                accountSubject.MtypeId,
                accountSubject.AbatYn,
                accountSubject.VoucherType,
                accountSubject.BudgetYn,
                accountSubject.OrgYn,
                accountSubject.ExpYear,
                accountSubject.ResiValue,
                accountSubject.DepreLid,
                accountSubject.AccudepreLid,
                accountSubject.StypeYn,
                accountSubject.IfrsStypeId,
                accountSubject.RocStypeId,
                accountSubject.SapStypeId,
                accountSubject.StypeClass,
                accountSubject.StypeOrder,
                accountSubject.Amt0,
                accountSubject.Amt1,
                accountSubject.UpdatedBy,
                accountSubject.UpdatedAt
            });

            return accountSubject;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改會計科目失敗: {accountSubject.StypeId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string stypeId)
    {
        try
        {
            const string sql = @"
                DELETE FROM AccountSubjects
                WHERE StypeId = @StypeId";

            await ExecuteAsync(sql, new { StypeId = stypeId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除會計科目失敗: {stypeId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string stypeId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM AccountSubjects
                WHERE StypeId = @StypeId";

            var count = await QuerySingleAsync<int>(sql, new { StypeId = stypeId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查會計科目是否存在失敗: {stypeId}", ex);
            throw;
        }
    }

    public async Task<decimal> GetUnsettledBalanceAsync(string stypeId)
    {
        try
        {
            // TODO: 實作未沖帳餘額查詢邏輯
            // 目前先返回 0，後續需要根據實際業務邏輯實作
            // 需要查詢傳票明細表，排除已作廢傳票
            const string sql = @"
                SELECT ISNULL(SUM(Amt1 - Amt0), 0) FROM AccountSubjects
                WHERE StypeId = @StypeId";

            var balance = await QuerySingleAsync<decimal>(sql, new { StypeId = stypeId });
            return balance;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢未沖帳餘額失敗: {stypeId}", ex);
            throw;
        }
    }
}

