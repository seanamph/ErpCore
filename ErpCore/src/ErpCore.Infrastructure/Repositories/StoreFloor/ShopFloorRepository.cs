using System.Data;
using Dapper;
using ErpCore.Domain.Entities.StoreFloor;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.StoreFloor;

/// <summary>
/// 商店樓層 Repository 實作 (SYS6000 - 商店資料維護)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class ShopFloorRepository : BaseRepository, IShopFloorRepository
{
    public ShopFloorRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<ShopFloor?> GetByIdAsync(string shopId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM ShopFloors 
                WHERE ShopId = @ShopId";

            return await QueryFirstOrDefaultAsync<ShopFloor>(sql, new { ShopId = shopId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢商店失敗: {shopId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<ShopFloor>> QueryAsync(ShopFloorQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM ShopFloors
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ShopId))
            {
                sql += " AND ShopId LIKE @ShopId";
                parameters.Add("ShopId", $"%{query.ShopId}%");
            }

            if (!string.IsNullOrEmpty(query.ShopName))
            {
                sql += " AND ShopName LIKE @ShopName";
                parameters.Add("ShopName", $"%{query.ShopName}%");
            }

            if (!string.IsNullOrEmpty(query.ShopType))
            {
                sql += " AND ShopType = @ShopType";
                parameters.Add("ShopType", query.ShopType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (!string.IsNullOrEmpty(query.City))
            {
                sql += " AND City = @City";
                parameters.Add("City", query.City);
            }

            if (!string.IsNullOrEmpty(query.FloorId))
            {
                sql += " AND FloorId = @FloorId";
                parameters.Add("FloorId", query.FloorId);
            }

            // 排序
            var sortField = query.SortField ?? "ShopId";
            var sortOrder = query.SortOrder ?? "ASC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<ShopFloor>(sql, parameters);
            var totalCount = await GetCountAsync(query);

            return new PagedResult<ShopFloor>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢商店列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(ShopFloorQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM ShopFloors
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ShopId))
            {
                sql += " AND ShopId LIKE @ShopId";
                parameters.Add("ShopId", $"%{query.ShopId}%");
            }

            if (!string.IsNullOrEmpty(query.ShopName))
            {
                sql += " AND ShopName LIKE @ShopName";
                parameters.Add("ShopName", $"%{query.ShopName}%");
            }

            if (!string.IsNullOrEmpty(query.ShopType))
            {
                sql += " AND ShopType = @ShopType";
                parameters.Add("ShopType", query.ShopType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (!string.IsNullOrEmpty(query.City))
            {
                sql += " AND City = @City";
                parameters.Add("City", query.City);
            }

            if (!string.IsNullOrEmpty(query.FloorId))
            {
                sql += " AND FloorId = @FloorId";
                parameters.Add("FloorId", query.FloorId);
            }

            return await QuerySingleAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢商店總數失敗", ex);
            throw;
        }
    }

    public async Task<ShopFloor> CreateAsync(ShopFloor shopFloor)
    {
        try
        {
            const string sql = @"
                INSERT INTO ShopFloors (
                    ShopId, ShopName, ShopNameEn, FloorId, FloorName, ShopType,
                    Address, City, Zone, PostalCode, Phone, Fax, Email,
                    ManagerName, ManagerPhone, OpenDate, CloseDate, Status,
                    PosEnabled, PosSystemId, PosTerminalId, Notes,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                )
                VALUES (
                    @ShopId, @ShopName, @ShopNameEn, @FloorId, @FloorName, @ShopType,
                    @Address, @City, @Zone, @PostalCode, @Phone, @Fax, @Email,
                    @ManagerName, @ManagerPhone, @OpenDate, @CloseDate, @Status,
                    @PosEnabled, @PosSystemId, @PosTerminalId, @Notes,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var parameters = new DynamicParameters();
            parameters.Add("ShopId", shopFloor.ShopId);
            parameters.Add("ShopName", shopFloor.ShopName);
            parameters.Add("ShopNameEn", shopFloor.ShopNameEn);
            parameters.Add("FloorId", shopFloor.FloorId);
            parameters.Add("FloorName", shopFloor.FloorName);
            parameters.Add("ShopType", shopFloor.ShopType);
            parameters.Add("Address", shopFloor.Address);
            parameters.Add("City", shopFloor.City);
            parameters.Add("Zone", shopFloor.Zone);
            parameters.Add("PostalCode", shopFloor.PostalCode);
            parameters.Add("Phone", shopFloor.Phone);
            parameters.Add("Fax", shopFloor.Fax);
            parameters.Add("Email", shopFloor.Email);
            parameters.Add("ManagerName", shopFloor.ManagerName);
            parameters.Add("ManagerPhone", shopFloor.ManagerPhone);
            parameters.Add("OpenDate", shopFloor.OpenDate);
            parameters.Add("CloseDate", shopFloor.CloseDate);
            parameters.Add("Status", shopFloor.Status);
            parameters.Add("PosEnabled", shopFloor.PosEnabled);
            parameters.Add("PosSystemId", shopFloor.PosSystemId);
            parameters.Add("PosTerminalId", shopFloor.PosTerminalId);
            parameters.Add("Notes", shopFloor.Notes);
            parameters.Add("CreatedBy", shopFloor.CreatedBy);
            parameters.Add("CreatedAt", shopFloor.CreatedAt);
            parameters.Add("UpdatedBy", shopFloor.UpdatedBy);
            parameters.Add("UpdatedAt", shopFloor.UpdatedAt);

            var tKey = await QuerySingleAsync<long>(sql, parameters);
            shopFloor.TKey = tKey;

            return shopFloor;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增商店失敗: {shopFloor.ShopId}", ex);
            throw;
        }
    }

    public async Task<ShopFloor> UpdateAsync(ShopFloor shopFloor)
    {
        try
        {
            const string sql = @"
                UPDATE ShopFloors SET
                    ShopName = @ShopName,
                    ShopNameEn = @ShopNameEn,
                    FloorId = @FloorId,
                    FloorName = @FloorName,
                    ShopType = @ShopType,
                    Address = @Address,
                    City = @City,
                    Zone = @Zone,
                    PostalCode = @PostalCode,
                    Phone = @Phone,
                    Fax = @Fax,
                    Email = @Email,
                    ManagerName = @ManagerName,
                    ManagerPhone = @ManagerPhone,
                    OpenDate = @OpenDate,
                    CloseDate = @CloseDate,
                    Status = @Status,
                    PosEnabled = @PosEnabled,
                    PosSystemId = @PosSystemId,
                    PosTerminalId = @PosTerminalId,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE ShopId = @ShopId";

            var parameters = new DynamicParameters();
            parameters.Add("ShopId", shopFloor.ShopId);
            parameters.Add("ShopName", shopFloor.ShopName);
            parameters.Add("ShopNameEn", shopFloor.ShopNameEn);
            parameters.Add("FloorId", shopFloor.FloorId);
            parameters.Add("FloorName", shopFloor.FloorName);
            parameters.Add("ShopType", shopFloor.ShopType);
            parameters.Add("Address", shopFloor.Address);
            parameters.Add("City", shopFloor.City);
            parameters.Add("Zone", shopFloor.Zone);
            parameters.Add("PostalCode", shopFloor.PostalCode);
            parameters.Add("Phone", shopFloor.Phone);
            parameters.Add("Fax", shopFloor.Fax);
            parameters.Add("Email", shopFloor.Email);
            parameters.Add("ManagerName", shopFloor.ManagerName);
            parameters.Add("ManagerPhone", shopFloor.ManagerPhone);
            parameters.Add("OpenDate", shopFloor.OpenDate);
            parameters.Add("CloseDate", shopFloor.CloseDate);
            parameters.Add("Status", shopFloor.Status);
            parameters.Add("PosEnabled", shopFloor.PosEnabled);
            parameters.Add("PosSystemId", shopFloor.PosSystemId);
            parameters.Add("PosTerminalId", shopFloor.PosTerminalId);
            parameters.Add("Notes", shopFloor.Notes);
            parameters.Add("UpdatedBy", shopFloor.UpdatedBy);
            parameters.Add("UpdatedAt", shopFloor.UpdatedAt);

            await ExecuteAsync(sql, parameters);

            return shopFloor;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改商店失敗: {shopFloor.ShopId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string shopId)
    {
        try
        {
            // 軟刪除：將狀態設為停用
            const string sql = @"
                UPDATE ShopFloors SET
                    Status = 'I',
                    UpdatedAt = GETDATE()
                WHERE ShopId = @ShopId";

            await ExecuteAsync(sql, new { ShopId = shopId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除商店失敗: {shopId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string shopId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM ShopFloors
                WHERE ShopId = @ShopId";

            var count = await QuerySingleAsync<int>(sql, new { ShopId = shopId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查商店編號是否存在失敗: {shopId}", ex);
            throw;
        }
    }

    public async Task UpdateStatusAsync(string shopId, string status)
    {
        try
        {
            const string sql = @"
                UPDATE ShopFloors SET
                    Status = @Status,
                    UpdatedAt = GETDATE()
                WHERE ShopId = @ShopId";

            await ExecuteAsync(sql, new { ShopId = shopId, Status = status });
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新商店狀態失敗: {shopId}", ex);
            throw;
        }
    }
}

