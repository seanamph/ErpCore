using Dapper;
using ErpCore.Domain.Entities.BusinessReport;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.BusinessReport;

/// <summary>
/// 業務報表管理 Repository 實作 (SYSL145)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class BusinessReportManagementRepository : BaseRepository, IBusinessReportManagementRepository
{
    public BusinessReportManagementRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<BusinessReportManagement?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM BusinessReportManagement 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<BusinessReportManagement>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢業務報表管理失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<PagedResult<BusinessReportManagement>> QueryAsync(BusinessReportManagementQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM BusinessReportManagement
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.SiteId))
            {
                sql += " AND SiteId = @SiteId";
                parameters.Add("SiteId", query.SiteId);
            }

            if (!string.IsNullOrEmpty(query.Type))
            {
                sql += " AND Type = @Type";
                parameters.Add("Type", query.Type);
            }

            if (!string.IsNullOrEmpty(query.Id))
            {
                sql += " AND Id LIKE @Id";
                parameters.Add("Id", $"%{query.Id}%");
            }

            if (!string.IsNullOrEmpty(query.UserId))
            {
                sql += " AND UserId = @UserId";
                parameters.Add("UserId", query.UserId);
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

            var items = await QueryAsync<BusinessReportManagement>(sql, parameters);

            return new PagedResult<BusinessReportManagement>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢業務報表管理列表失敗", ex);
            throw;
        }
    }

    public async Task<BusinessReportManagement> CreateAsync(BusinessReportManagement entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO BusinessReportManagement 
                (SiteId, Type, Id, UserId, UserName, Status, Notes, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup)
                VALUES 
                (@SiteId, @Type, @Id, @UserId, @UserName, @Status, @Notes, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup);
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, entity);
            entity.TKey = tKey;

            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增業務報表管理失敗", ex);
            throw;
        }
    }

    public async Task<BusinessReportManagement> UpdateAsync(BusinessReportManagement entity)
    {
        try
        {
            const string sql = @"
                UPDATE BusinessReportManagement SET
                    SiteId = @SiteId,
                    Type = @Type,
                    Id = @Id,
                    UserId = @UserId,
                    UserName = @UserName,
                    Status = @Status,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, entity);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改業務報表管理失敗: {entity.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = "DELETE FROM BusinessReportManagement WHERE TKey = @TKey";
            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除業務報表管理失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<int> BatchDeleteAsync(List<long> tKeys)
    {
        try
        {
            const string sql = @"
                DELETE FROM BusinessReportManagement 
                WHERE TKey IN @TKeys";

            var parameters = new { TKeys = tKeys };
            return await ExecuteAsync(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("批次刪除業務報表管理失敗", ex);
            throw;
        }
    }

    public async Task<BusinessReportManagement?> CheckDuplicateAsync(string siteId, string type, string id, long? excludeTKey = null)
    {
        try
        {
            var sql = @"
                SELECT * FROM BusinessReportManagement 
                WHERE SiteId = @SiteId AND Type = @Type AND Id = @Id";

            var parameters = new DynamicParameters();
            parameters.Add("SiteId", siteId);
            parameters.Add("Type", type);
            parameters.Add("Id", id);

            if (excludeTKey.HasValue)
            {
                sql += " AND TKey != @ExcludeTKey";
                parameters.Add("ExcludeTKey", excludeTKey.Value);
            }

            return await QueryFirstOrDefaultAsync<BusinessReportManagement>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查重複資料失敗: SiteId={siteId}, Type={type}, Id={id}", ex);
            throw;
        }
    }
}

