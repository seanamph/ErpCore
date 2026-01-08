using Dapper;
using ErpCore.Domain.Entities.BasicData;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.BasicData;

/// <summary>
/// 區域 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class AreaRepository : BaseRepository, IAreaRepository
{
    public AreaRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Area?> GetByIdAsync(string areaId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Areas 
                WHERE AreaId = @AreaId";

            return await QueryFirstOrDefaultAsync<Area>(sql, new { AreaId = areaId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢區域失敗: {areaId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<Area>> QueryAsync(AreaQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Areas
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.AreaId))
            {
                sql += " AND AreaId LIKE @AreaId";
                parameters.Add("AreaId", $"%{query.AreaId}%");
            }

            if (!string.IsNullOrEmpty(query.AreaName))
            {
                sql += " AND AreaName LIKE @AreaName";
                parameters.Add("AreaName", $"%{query.AreaName}%");
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "AreaId" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<Area>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM Areas
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.AreaId))
            {
                countSql += " AND AreaId LIKE @AreaId";
                countParameters.Add("AreaId", $"%{query.AreaId}%");
            }
            if (!string.IsNullOrEmpty(query.AreaName))
            {
                countSql += " AND AreaName LIKE @AreaName";
                countParameters.Add("AreaName", $"%{query.AreaName}%");
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND Status = @Status";
                countParameters.Add("Status", query.Status);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<Area>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢區域列表失敗", ex);
            throw;
        }
    }

    public async Task<Area> CreateAsync(Area area)
    {
        try
        {
            const string sql = @"
                INSERT INTO Areas (
                    AreaId, AreaName, SeqNo, Status, Notes,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup
                )
                OUTPUT INSERTED.*
                VALUES (
                    @AreaId, @AreaName, @SeqNo, @Status, @Notes,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup
                )";

            var result = await QueryFirstOrDefaultAsync<Area>(sql, area);
            if (result == null)
            {
                throw new InvalidOperationException("新增區域失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增區域失敗: {area.AreaId}", ex);
            throw;
        }
    }

    public async Task<Area> UpdateAsync(Area area)
    {
        try
        {
            const string sql = @"
                UPDATE Areas SET
                    AreaName = @AreaName,
                    SeqNo = @SeqNo,
                    Status = @Status,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                OUTPUT INSERTED.*
                WHERE AreaId = @AreaId";

            var result = await QueryFirstOrDefaultAsync<Area>(sql, area);
            if (result == null)
            {
                throw new InvalidOperationException($"區域不存在: {area.AreaId}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改區域失敗: {area.AreaId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string areaId)
    {
        try
        {
            const string sql = @"
                DELETE FROM Areas 
                WHERE AreaId = @AreaId";

            await ExecuteAsync(sql, new { AreaId = areaId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除區域失敗: {areaId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string areaId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM Areas 
                WHERE AreaId = @AreaId";

            var count = await QuerySingleAsync<int>(sql, new { AreaId = areaId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查區域是否存在失敗: {areaId}", ex);
            throw;
        }
    }
}

