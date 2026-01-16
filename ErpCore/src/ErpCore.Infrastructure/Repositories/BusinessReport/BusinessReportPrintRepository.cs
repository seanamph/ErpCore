using Dapper;
using ErpCore.Domain.Entities.BusinessReport;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.BusinessReport;

/// <summary>
/// 業務報表列印 Repository 實作 (SYSL150)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class BusinessReportPrintRepository : BaseRepository, IBusinessReportPrintRepository
{
    public BusinessReportPrintRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<BusinessReportPrint?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM BusinessReportPrint 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<BusinessReportPrint>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢業務報表列印失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<PagedResult<BusinessReportPrint>> QueryAsync(BusinessReportPrintQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM BusinessReportPrint
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (query.GiveYear.HasValue)
            {
                sql += " AND GiveYear = @GiveYear";
                parameters.Add("GiveYear", query.GiveYear.Value);
            }

            if (!string.IsNullOrEmpty(query.SiteId))
            {
                sql += " AND SiteId = @SiteId";
                parameters.Add("SiteId", query.SiteId);
            }

            if (!string.IsNullOrEmpty(query.OrgId))
            {
                sql += " AND OrgId = @OrgId";
                parameters.Add("OrgId", query.OrgId);
            }

            if (!string.IsNullOrEmpty(query.EmpId))
            {
                sql += " AND EmpId LIKE @EmpId";
                parameters.Add("EmpId", $"%{query.EmpId}%");
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 計算總筆數
            var countSql = sql.Replace("SELECT *", "SELECT COUNT(*)");
            var totalCount = await ExecuteScalarAsync<int>(countSql, parameters);

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "TKey" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<BusinessReportPrint>(sql, parameters);

            return new PagedResult<BusinessReportPrint>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢業務報表列印列表失敗", ex);
            throw;
        }
    }

    public async Task<BusinessReportPrint> CreateAsync(BusinessReportPrint entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO BusinessReportPrint 
                (GiveYear, SiteId, OrgId, EmpId, EmpName, Qty, Status, Verifier, VerifyDate, Notes, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup)
                VALUES 
                (@GiveYear, @SiteId, @OrgId, @EmpId, @EmpName, @Qty, @Status, @Verifier, @VerifyDate, @Notes, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup);
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, entity);
            entity.TKey = tKey;

            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增業務報表列印失敗", ex);
            throw;
        }
    }

    public async Task<BusinessReportPrint> UpdateAsync(BusinessReportPrint entity)
    {
        try
        {
            const string sql = @"
                UPDATE BusinessReportPrint SET
                    GiveYear = @GiveYear,
                    SiteId = @SiteId,
                    OrgId = @OrgId,
                    EmpId = @EmpId,
                    EmpName = @EmpName,
                    Qty = @Qty,
                    Status = @Status,
                    Verifier = @Verifier,
                    VerifyDate = @VerifyDate,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, entity);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改業務報表列印失敗: {entity.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = "DELETE FROM BusinessReportPrint WHERE TKey = @TKey";
            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除業務報表列印失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<int> BatchDeleteAsync(List<long> tKeys)
    {
        try
        {
            const string sql = @"
                DELETE FROM BusinessReportPrint 
                WHERE TKey IN @TKeys";

            var parameters = new { TKeys = tKeys };
            return await ExecuteAsync(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("批次刪除業務報表列印失敗", ex);
            throw;
        }
    }

    public async Task<int> BatchAuditAsync(List<long> tKeys, string status, string verifier, DateTime verifyDate, string? notes = null)
    {
        try
        {
            const string sql = @"
                UPDATE BusinessReportPrint SET
                    Status = @Status,
                    Verifier = @Verifier,
                    VerifyDate = @VerifyDate,
                    Notes = CASE WHEN @Notes IS NOT NULL THEN @Notes ELSE Notes END,
                    UpdatedBy = @Verifier,
                    UpdatedAt = @VerifyDate
                WHERE TKey IN @TKeys";

            var parameters = new
            {
                TKeys = tKeys,
                Status = status,
                Verifier = verifier,
                VerifyDate = verifyDate,
                Notes = notes
            };

            return await ExecuteAsync(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("批次審核業務報表列印失敗", ex);
            throw;
        }
    }

    public async Task<int> CopyNextYearAsync(int sourceYear, int targetYear, string? siteId = null)
    {
        try
        {
            var sql = @"
                INSERT INTO BusinessReportPrint 
                (GiveYear, SiteId, OrgId, EmpId, EmpName, Qty, Status, Verifier, VerifyDate, Notes, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup)
                SELECT 
                    @TargetYear AS GiveYear,
                    SiteId,
                    OrgId,
                    EmpId,
                    EmpName,
                    Qty,
                    'P' AS Status,
                    NULL AS Verifier,
                    NULL AS VerifyDate,
                    Notes,
                    CreatedBy,
                    GETDATE() AS CreatedAt,
                    NULL AS UpdatedBy,
                    GETDATE() AS UpdatedAt,
                    CreatedPriority,
                    CreatedGroup
                FROM BusinessReportPrint
                WHERE GiveYear = @SourceYear";

            var parameters = new DynamicParameters();
            parameters.Add("SourceYear", sourceYear);
            parameters.Add("TargetYear", targetYear);

            if (!string.IsNullOrEmpty(siteId))
            {
                sql += " AND SiteId = @SiteId";
                parameters.Add("SiteId", siteId);
            }

            return await ExecuteAsync(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError($"複製下一年度資料失敗: SourceYear={sourceYear}, TargetYear={targetYear}", ex);
            throw;
        }
    }

    public async Task<bool> IsYearAuditedAsync(int giveYear, string? siteId = null)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM BusinessReportPrint
                WHERE GiveYear = @GiveYear AND Status = 'A'";

            var parameters = new DynamicParameters();
            parameters.Add("GiveYear", giveYear);

            if (!string.IsNullOrEmpty(siteId))
            {
                sql += " AND SiteId = @SiteId";
                parameters.Add("SiteId", siteId);
            }

            var count = await ExecuteScalarAsync<int>(sql, parameters);
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查年度審核狀態失敗: GiveYear={giveYear}", ex);
            throw;
        }
    }
}
