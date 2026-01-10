using Dapper;
using ErpCore.Domain.Entities.Recruitment;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Recruitment;

/// <summary>
/// 租戶位置 Repository 實作 (SYSC999)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class TenantLocationRepository : BaseRepository, ITenantLocationRepository
{
    public TenantLocationRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<TenantLocation?> GetByKeyAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM TenantLocations 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<TenantLocation>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢租戶位置失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<PagedResult<TenantLocation>> QueryAsync(TenantLocationQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM TenantLocations
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (query.AgmTKey.HasValue)
            {
                sql += " AND AgmTKey = @AgmTKey";
                parameters.Add("AgmTKey", query.AgmTKey.Value);
            }

            if (!string.IsNullOrEmpty(query.LocationId))
            {
                sql += " AND LocationId LIKE @LocationId";
                parameters.Add("LocationId", $"%{query.LocationId}%");
            }

            if (!string.IsNullOrEmpty(query.AreaId))
            {
                sql += " AND AreaId LIKE @AreaId";
                parameters.Add("AreaId", $"%{query.AreaId}%");
            }

            if (!string.IsNullOrEmpty(query.FloorId))
            {
                sql += " AND FloorId LIKE @FloorId";
                parameters.Add("FloorId", $"%{query.FloorId}%");
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "LocationId" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<TenantLocation>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM TenantLocations
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (query.AgmTKey.HasValue)
            {
                countSql += " AND AgmTKey = @AgmTKey";
                countParameters.Add("AgmTKey", query.AgmTKey.Value);
            }
            if (!string.IsNullOrEmpty(query.LocationId))
            {
                countSql += " AND LocationId LIKE @LocationId";
                countParameters.Add("LocationId", $"%{query.LocationId}%");
            }
            if (!string.IsNullOrEmpty(query.AreaId))
            {
                countSql += " AND AreaId LIKE @AreaId";
                countParameters.Add("AreaId", $"%{query.AreaId}%");
            }
            if (!string.IsNullOrEmpty(query.FloorId))
            {
                countSql += " AND FloorId LIKE @FloorId";
                countParameters.Add("FloorId", $"%{query.FloorId}%");
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND Status = @Status";
                countParameters.Add("Status", query.Status);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<TenantLocation>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢租戶位置列表失敗", ex);
            throw;
        }
    }

    public async Task<List<TenantLocation>> GetByTenantAsync(long agmTKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM TenantLocations
                WHERE AgmTKey = @AgmTKey
                ORDER BY LocationId";

            var items = await QueryAsync<TenantLocation>(sql, new { AgmTKey = agmTKey });
            return items.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"根據租戶查詢位置列表失敗: {agmTKey}", ex);
            throw;
        }
    }

    public async Task<TenantLocation> CreateAsync(TenantLocation tenantLocation)
    {
        try
        {
            const string sql = @"
                INSERT INTO TenantLocations (
                    AgmTKey, LocationId, AreaId, FloorId, Status, Notes,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup
                )
                OUTPUT INSERTED.*
                VALUES (
                    @AgmTKey, @LocationId, @AreaId, @FloorId, @Status, @Notes,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup
                )";

            var result = await QueryFirstOrDefaultAsync<TenantLocation>(sql, tenantLocation);
            if (result == null)
            {
                throw new InvalidOperationException("新增租戶位置失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增租戶位置失敗: {tenantLocation.LocationId}", ex);
            throw;
        }
    }

    public async Task<TenantLocation> UpdateAsync(TenantLocation tenantLocation)
    {
        try
        {
            const string sql = @"
                UPDATE TenantLocations SET
                    LocationId = @LocationId,
                    AreaId = @AreaId,
                    FloorId = @FloorId,
                    Status = @Status,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                OUTPUT INSERTED.*
                WHERE TKey = @TKey";

            var result = await QueryFirstOrDefaultAsync<TenantLocation>(sql, tenantLocation);
            if (result == null)
            {
                throw new InvalidOperationException($"租戶位置不存在: {tenantLocation.TKey}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改租戶位置失敗: {tenantLocation.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM TenantLocations
                WHERE TKey = @TKey";

            var rowsAffected = await ExecuteAsync(sql, new { TKey = tKey });
            if (rowsAffected == 0)
            {
                throw new InvalidOperationException($"租戶位置不存在: {tKey}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除租戶位置失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM TenantLocations
                WHERE TKey = @TKey";

            var count = await QuerySingleAsync<int>(sql, new { TKey = tKey });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查租戶位置是否存在失敗: {tKey}", ex);
            throw;
        }
    }
}

