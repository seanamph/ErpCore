using Dapper;
using ErpCore.Domain.Entities.BasicData;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.BasicData;

/// <summary>
/// 庫別 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class WarehouseRepository : BaseRepository, IWarehouseRepository
{
    public WarehouseRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Warehouse?> GetByIdAsync(string warehouseId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Warehouses 
                WHERE WarehouseId = @WarehouseId";

            return await QueryFirstOrDefaultAsync<Warehouse>(sql, new { WarehouseId = warehouseId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢庫別失敗: {warehouseId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<Warehouse>> QueryAsync(WarehouseQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Warehouses
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.WarehouseId))
            {
                sql += " AND WarehouseId LIKE @WarehouseId";
                parameters.Add("WarehouseId", $"%{query.WarehouseId}%");
            }

            if (!string.IsNullOrEmpty(query.WarehouseName))
            {
                sql += " AND WarehouseName LIKE @WarehouseName";
                parameters.Add("WarehouseName", $"%{query.WarehouseName}%");
            }

            if (!string.IsNullOrEmpty(query.WarehouseType))
            {
                sql += " AND WarehouseType = @WarehouseType";
                parameters.Add("WarehouseType", query.WarehouseType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "WarehouseId" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<Warehouse>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM Warehouses
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.WarehouseId))
            {
                countSql += " AND WarehouseId LIKE @WarehouseId";
                countParameters.Add("WarehouseId", $"%{query.WarehouseId}%");
            }
            if (!string.IsNullOrEmpty(query.WarehouseName))
            {
                countSql += " AND WarehouseName LIKE @WarehouseName";
                countParameters.Add("WarehouseName", $"%{query.WarehouseName}%");
            }
            if (!string.IsNullOrEmpty(query.WarehouseType))
            {
                countSql += " AND WarehouseType = @WarehouseType";
                countParameters.Add("WarehouseType", query.WarehouseType);
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND Status = @Status";
                countParameters.Add("Status", query.Status);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<Warehouse>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢庫別列表失敗", ex);
            throw;
        }
    }

    public async Task<Warehouse> CreateAsync(Warehouse warehouse)
    {
        try
        {
            const string sql = @"
                INSERT INTO Warehouses (
                    WarehouseId, WarehouseName, WarehouseType, Location, SeqNo, Status, Notes,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup
                )
                OUTPUT INSERTED.*
                VALUES (
                    @WarehouseId, @WarehouseName, @WarehouseType, @Location, @SeqNo, @Status, @Notes,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup
                )";

            var result = await QueryFirstOrDefaultAsync<Warehouse>(sql, warehouse);
            if (result == null)
            {
                throw new InvalidOperationException("新增庫別失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增庫別失敗: {warehouse.WarehouseId}", ex);
            throw;
        }
    }

    public async Task<Warehouse> UpdateAsync(Warehouse warehouse)
    {
        try
        {
            const string sql = @"
                UPDATE Warehouses SET
                    WarehouseName = @WarehouseName,
                    WarehouseType = @WarehouseType,
                    Location = @Location,
                    SeqNo = @SeqNo,
                    Status = @Status,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                OUTPUT INSERTED.*
                WHERE WarehouseId = @WarehouseId";

            var result = await QueryFirstOrDefaultAsync<Warehouse>(sql, warehouse);
            if (result == null)
            {
                throw new InvalidOperationException($"庫別不存在: {warehouse.WarehouseId}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改庫別失敗: {warehouse.WarehouseId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string warehouseId)
    {
        try
        {
            const string sql = @"
                DELETE FROM Warehouses 
                WHERE WarehouseId = @WarehouseId";

            await ExecuteAsync(sql, new { WarehouseId = warehouseId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除庫別失敗: {warehouseId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string warehouseId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM Warehouses 
                WHERE WarehouseId = @WarehouseId";

            var count = await QuerySingleAsync<int>(sql, new { WarehouseId = warehouseId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查庫別是否存在失敗: {warehouseId}", ex);
            throw;
        }
    }
}
