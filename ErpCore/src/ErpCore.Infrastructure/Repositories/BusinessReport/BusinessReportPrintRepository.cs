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

    public async Task<PagedResult<BusinessReportPrint>> QueryAsync(BusinessReportPrintQuery query)
    {
        try
        {
            var sql = @"
                SELECT 
                    brp.TKey,
                    brp.GiveYear,
                    brp.SiteId,
                    s.SiteName,
                    brp.OrgId,
                    o.OrgName,
                    brp.EmpId,
                    brp.EmpName,
                    brp.Qty,
                    brp.Status,
                    brp.Verifier,
                    u.UserName AS VerifierName,
                    brp.VerifyDate,
                    brp.Notes,
                    brp.CreatedBy,
                    brp.CreatedAt,
                    brp.UpdatedBy,
                    brp.UpdatedAt,
                    brp.CreatedPriority,
                    brp.CreatedGroup
                FROM BusinessReportPrint brp
                LEFT JOIN Sites s ON brp.SiteId = s.SiteId
                LEFT JOIN Organizations o ON brp.OrgId = o.OrgId
                LEFT JOIN Users u ON brp.Verifier = u.UserId
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (query.GiveYear.HasValue)
            {
                sql += " AND brp.GiveYear = @GiveYear";
                parameters.Add("GiveYear", query.GiveYear.Value);
            }

            if (!string.IsNullOrEmpty(query.SiteId))
            {
                sql += " AND brp.SiteId = @SiteId";
                parameters.Add("SiteId", query.SiteId);
            }

            if (!string.IsNullOrEmpty(query.OrgId))
            {
                sql += " AND brp.OrgId = @OrgId";
                parameters.Add("OrgId", query.OrgId);
            }

            if (!string.IsNullOrEmpty(query.EmpId))
            {
                sql += " AND brp.EmpId LIKE @EmpId";
                parameters.Add("EmpId", $"%{query.EmpId}%");
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND brp.Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 計算總筆數
            var countSql = @"
                SELECT COUNT(*) 
                FROM BusinessReportPrint brp
                WHERE 1=1";
            if (query.GiveYear.HasValue)
            {
                countSql += " AND brp.GiveYear = @GiveYear";
            }
            if (!string.IsNullOrEmpty(query.SiteId))
            {
                countSql += " AND brp.SiteId = @SiteId";
            }
            if (!string.IsNullOrEmpty(query.OrgId))
            {
                countSql += " AND brp.OrgId = @OrgId";
            }
            if (!string.IsNullOrEmpty(query.EmpId))
            {
                countSql += " AND brp.EmpId LIKE @EmpId";
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND brp.Status = @Status";
            }
            var totalCount = await ExecuteScalarAsync<int>(countSql, parameters);

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "TKey" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY brp.{sortField} {sortOrder}";

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

    public async Task<BusinessReportPrint?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT 
                    brp.TKey,
                    brp.GiveYear,
                    brp.SiteId,
                    s.SiteName,
                    brp.OrgId,
                    o.OrgName,
                    brp.EmpId,
                    brp.EmpName,
                    brp.Qty,
                    brp.Status,
                    brp.Verifier,
                    u.UserName AS VerifierName,
                    brp.VerifyDate,
                    brp.Notes,
                    brp.CreatedBy,
                    brp.CreatedAt,
                    brp.UpdatedBy,
                    brp.UpdatedAt,
                    brp.CreatedPriority,
                    brp.CreatedGroup
                FROM BusinessReportPrint brp
                LEFT JOIN Sites s ON brp.SiteId = s.SiteId
                LEFT JOIN Organizations o ON brp.OrgId = o.OrgId
                LEFT JOIN Users u ON brp.Verifier = u.UserId
                WHERE brp.TKey = @TKey";

            return await QueryFirstOrDefaultAsync<BusinessReportPrint>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢業務報表列印失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<long> CreateAsync(BusinessReportPrint entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO BusinessReportPrint 
                (GiveYear, SiteId, OrgId, EmpId, EmpName, Qty, Status, Notes, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup)
                VALUES 
                (@GiveYear, @SiteId, @OrgId, @EmpId, @EmpName, @Qty, @Status, @Notes, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup);
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, entity);
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增業務報表列印失敗", ex);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(BusinessReportPrint entity)
    {
        try
        {
            const string sql = @"
                UPDATE BusinessReportPrint SET
                    OrgId = @OrgId,
                    EmpName = @EmpName,
                    Qty = @Qty,
                    Status = @Status,
                    Verifier = @Verifier,
                    VerifyDate = @VerifyDate,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            var affectedRows = await ExecuteAsync(sql, entity);
            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改業務報表列印失敗: {entity.TKey}", ex);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(long tKey)
    {
        try
        {
            const string sql = "DELETE FROM BusinessReportPrint WHERE TKey = @TKey";
            var affectedRows = await ExecuteAsync(sql, new { TKey = tKey });
            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除業務報表列印失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<int> BatchAuditAsync(List<long> tKeys, string status, string? verifier, DateTime verifyDate, string? notes)
    {
        try
        {
            const string sql = @"
                UPDATE BusinessReportPrint SET
                    Status = @Status,
                    Verifier = @Verifier,
                    VerifyDate = @VerifyDate,
                    Notes = @Notes,
                    UpdatedAt = GETDATE()
                WHERE TKey IN @TKeys";

            var parameters = new
            {
                Status = status,
                Verifier = verifier,
                VerifyDate = verifyDate,
                Notes = notes,
                TKeys = tKeys
            };

            return await ExecuteAsync(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("批次審核業務報表列印失敗", ex);
            throw;
        }
    }

    public async Task<int> CopyNextYearAsync(int sourceYear, int targetYear, string? siteId)
    {
        try
        {
            var sql = @"
                INSERT INTO BusinessReportPrint 
                (GiveYear, SiteId, OrgId, EmpId, EmpName, Qty, Status, Notes, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup)
                SELECT 
                    @TargetYear,
                    SiteId,
                    OrgId,
                    EmpId,
                    EmpName,
                    Qty,
                    'P',
                    Notes,
                    CreatedBy,
                    GETDATE(),
                    UpdatedBy,
                    GETDATE(),
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

    public async Task<decimal> CalculateQtyAsync(long tKey, Dictionary<string, object>? calculationRules)
    {
        try
        {
            // 基本數量計算邏輯（可根據業務需求擴展）
            var entity = await GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new Exception($"找不到業務報表列印資料: {tKey}");
            }

            // 預設返回現有數量，實際業務邏輯需根據需求實作
            return entity.Qty ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"計算數量失敗: {tKey}", ex);
            throw;
        }
    }
}

