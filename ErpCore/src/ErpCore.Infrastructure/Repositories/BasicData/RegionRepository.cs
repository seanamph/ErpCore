using Dapper;
using ErpCore.Domain.Entities.BasicData;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.BasicData;

/// <summary>
/// 地區 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class RegionRepository : BaseRepository, IRegionRepository
{
    public RegionRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Region?> GetByIdAsync(string regionId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Regions 
                WHERE RegionId = @RegionId";

            return await QueryFirstOrDefaultAsync<Region>(sql, new { RegionId = regionId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢地區失敗: {regionId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<Region>> QueryAsync(RegionQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Regions
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.RegionId))
            {
                sql += " AND RegionId LIKE @RegionId";
                parameters.Add("RegionId", $"%{query.RegionId}%");
            }

            if (!string.IsNullOrEmpty(query.RegionName))
            {
                sql += " AND RegionName LIKE @RegionName";
                parameters.Add("RegionName", $"%{query.RegionName}%");
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "RegionId" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<Region>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM Regions
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.RegionId))
            {
                countSql += " AND RegionId LIKE @RegionId";
                countParameters.Add("RegionId", $"%{query.RegionId}%");
            }
            if (!string.IsNullOrEmpty(query.RegionName))
            {
                countSql += " AND RegionName LIKE @RegionName";
                countParameters.Add("RegionName", $"%{query.RegionName}%");
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<Region>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢地區列表失敗", ex);
            throw;
        }
    }

    public async Task<Region> CreateAsync(Region region)
    {
        try
        {
            const string sql = @"
                INSERT INTO Regions (
                    RegionId, RegionName, Memo,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup
                )
                OUTPUT INSERTED.*
                VALUES (
                    @RegionId, @RegionName, @Memo,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup
                )";

            var result = await QueryFirstOrDefaultAsync<Region>(sql, region);
            if (result == null)
            {
                throw new InvalidOperationException("新增地區失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增地區失敗: {region.RegionId}", ex);
            throw;
        }
    }

    public async Task<Region> UpdateAsync(Region region)
    {
        try
        {
            const string sql = @"
                UPDATE Regions SET
                    RegionName = @RegionName,
                    Memo = @Memo,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                OUTPUT INSERTED.*
                WHERE RegionId = @RegionId";

            var result = await QueryFirstOrDefaultAsync<Region>(sql, region);
            if (result == null)
            {
                throw new InvalidOperationException($"地區不存在: {region.RegionId}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改地區失敗: {region.RegionId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string regionId)
    {
        try
        {
            const string sql = @"
                DELETE FROM Regions 
                WHERE RegionId = @RegionId";

            await ExecuteAsync(sql, new { RegionId = regionId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除地區失敗: {regionId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string regionId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM Regions 
                WHERE RegionId = @RegionId";

            var count = await QuerySingleAsync<int>(sql, new { RegionId = regionId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查地區是否存在失敗: {regionId}", ex);
            throw;
        }
    }
}

