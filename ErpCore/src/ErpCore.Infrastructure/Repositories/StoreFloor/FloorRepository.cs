using System.Data;
using Dapper;
using ErpCore.Domain.Entities.StoreFloor;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.StoreFloor;

/// <summary>
/// 樓層 Repository 實作 (SYS6310-SYS6370 - 樓層資料維護)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class FloorRepository : BaseRepository, IFloorRepository
{
    public FloorRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Floor?> GetByIdAsync(string floorId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Floors 
                WHERE FloorId = @FloorId";

            return await QueryFirstOrDefaultAsync<Floor>(sql, new { FloorId = floorId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢樓層失敗: {floorId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<Floor>> QueryAsync(FloorQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Floors
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.FloorId))
            {
                sql += " AND FloorId LIKE @FloorId";
                parameters.Add("FloorId", $"%{query.FloorId}%");
            }

            if (!string.IsNullOrEmpty(query.FloorName))
            {
                sql += " AND FloorName LIKE @FloorName";
                parameters.Add("FloorName", $"%{query.FloorName}%");
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 排序
            var sortField = query.SortField ?? "FloorNumber";
            var sortOrder = query.SortOrder ?? "ASC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<Floor>(sql, parameters);
            var totalCount = await GetCountAsync(query);

            return new PagedResult<Floor>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢樓層列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(FloorQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM Floors
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.FloorId))
            {
                sql += " AND FloorId LIKE @FloorId";
                parameters.Add("FloorId", $"%{query.FloorId}%");
            }

            if (!string.IsNullOrEmpty(query.FloorName))
            {
                sql += " AND FloorName LIKE @FloorName";
                parameters.Add("FloorName", $"%{query.FloorName}%");
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            return await QuerySingleAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢樓層總數失敗", ex);
            throw;
        }
    }

    public async Task<Floor> CreateAsync(Floor floor)
    {
        try
        {
            const string sql = @"
                INSERT INTO Floors (
                    FloorId, FloorName, FloorNameEn, FloorNumber, Description, Status,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                )
                VALUES (
                    @FloorId, @FloorName, @FloorNameEn, @FloorNumber, @Description, @Status,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                )";

            var parameters = new DynamicParameters();
            parameters.Add("FloorId", floor.FloorId);
            parameters.Add("FloorName", floor.FloorName);
            parameters.Add("FloorNameEn", floor.FloorNameEn);
            parameters.Add("FloorNumber", floor.FloorNumber);
            parameters.Add("Description", floor.Description);
            parameters.Add("Status", floor.Status);
            parameters.Add("CreatedBy", floor.CreatedBy);
            parameters.Add("CreatedAt", floor.CreatedAt);
            parameters.Add("UpdatedBy", floor.UpdatedBy);
            parameters.Add("UpdatedAt", floor.UpdatedAt);

            await ExecuteAsync(sql, parameters);

            return floor;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增樓層失敗: {floor.FloorId}", ex);
            throw;
        }
    }

    public async Task<Floor> UpdateAsync(Floor floor)
    {
        try
        {
            const string sql = @"
                UPDATE Floors SET
                    FloorName = @FloorName,
                    FloorNameEn = @FloorNameEn,
                    FloorNumber = @FloorNumber,
                    Description = @Description,
                    Status = @Status,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE FloorId = @FloorId";

            var parameters = new DynamicParameters();
            parameters.Add("FloorId", floor.FloorId);
            parameters.Add("FloorName", floor.FloorName);
            parameters.Add("FloorNameEn", floor.FloorNameEn);
            parameters.Add("FloorNumber", floor.FloorNumber);
            parameters.Add("Description", floor.Description);
            parameters.Add("Status", floor.Status);
            parameters.Add("UpdatedBy", floor.UpdatedBy);
            parameters.Add("UpdatedAt", floor.UpdatedAt);

            await ExecuteAsync(sql, parameters);

            return floor;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改樓層失敗: {floor.FloorId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string floorId)
    {
        try
        {
            // 軟刪除：將狀態設為停用
            const string sql = @"
                UPDATE Floors SET
                    Status = 'I',
                    UpdatedAt = GETDATE()
                WHERE FloorId = @FloorId";

            await ExecuteAsync(sql, new { FloorId = floorId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除樓層失敗: {floorId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string floorId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM Floors
                WHERE FloorId = @FloorId";

            var count = await QuerySingleAsync<int>(sql, new { FloorId = floorId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查樓層代碼是否存在失敗: {floorId}", ex);
            throw;
        }
    }

    public async Task UpdateStatusAsync(string floorId, string status)
    {
        try
        {
            const string sql = @"
                UPDATE Floors SET
                    Status = @Status,
                    UpdatedAt = GETDATE()
                WHERE FloorId = @FloorId";

            await ExecuteAsync(sql, new { FloorId = floorId, Status = status });
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新樓層狀態失敗: {floorId}", ex);
            throw;
        }
    }

    public async Task<int> GetShopCountAsync(string floorId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM ShopFloors
                WHERE FloorId = @FloorId AND Status = 'A'";

            return await QuerySingleAsync<int>(sql, new { FloorId = floorId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢樓層商店數量失敗: {floorId}", ex);
            throw;
        }
    }
}

