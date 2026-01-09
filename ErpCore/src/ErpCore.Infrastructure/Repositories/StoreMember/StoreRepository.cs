using System.Data;
using Dapper;
using ErpCore.Domain.Entities.StoreMember;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.StoreMember;

/// <summary>
/// 商店 Repository 實作 (SYS3000 - 商店資料維護)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class StoreRepository : BaseRepository, IStoreRepository
{
    public StoreRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Shop?> GetByIdAsync(string shopId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Shops 
                WHERE ShopId = @ShopId";

            return await QueryFirstOrDefaultAsync<Shop>(sql, new { ShopId = shopId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢商店失敗: {shopId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<Shop>> QueryAsync(StoreQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Shops
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

            if (!string.IsNullOrEmpty(query.AreaId))
            {
                sql += " AND AreaId = @AreaId";
                parameters.Add("AreaId", query.AreaId);
            }

            // 排序
            var sortField = query.SortField ?? "ShopId";
            var sortOrder = query.SortOrder ?? "ASC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<Shop>(sql, parameters);
            var totalCount = await GetCountAsync(query);

            return new PagedResult<Shop>
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

    public async Task<int> GetCountAsync(StoreQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM Shops
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

            if (!string.IsNullOrEmpty(query.AreaId))
            {
                sql += " AND AreaId = @AreaId";
                parameters.Add("AreaId", query.AreaId);
            }

            return await ExecuteScalarAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢商店總數失敗", ex);
            throw;
        }
    }

    public async Task<Shop> CreateAsync(Shop shop)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            shop.CreatedAt = DateTime.Now;
            shop.UpdatedAt = DateTime.Now;

            const string sql = @"
                INSERT INTO Shops (
                    ShopId, ShopName, ShopNameEn, ShopType, Address, City, Zone, PostalCode,
                    Phone, Fax, Email, ManagerName, ManagerPhone, OpenDate, CloseDate,
                    Status, FloorId, AreaId, PosEnabled, PosSystemId, Notes,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                ) VALUES (
                    @ShopId, @ShopName, @ShopNameEn, @ShopType, @Address, @City, @Zone, @PostalCode,
                    @Phone, @Fax, @Email, @ManagerName, @ManagerPhone, @OpenDate, @CloseDate,
                    @Status, @FloorId, @AreaId, @PosEnabled, @PosSystemId, @Notes,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await connection.QuerySingleAsync<long>(sql, shop, transaction);
            shop.TKey = tKey;

            transaction.Commit();
            _logger.LogInfo($"建立商店成功: {shop.ShopId}");
            return shop;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError($"建立商店失敗: {shop.ShopId}", ex);
            throw;
        }
    }

    public async Task<Shop> UpdateAsync(Shop shop)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            shop.UpdatedAt = DateTime.Now;

            const string sql = @"
                UPDATE Shops SET
                    ShopName = @ShopName,
                    ShopNameEn = @ShopNameEn,
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
                    FloorId = @FloorId,
                    AreaId = @AreaId,
                    PosEnabled = @PosEnabled,
                    PosSystemId = @PosSystemId,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE ShopId = @ShopId";

            await connection.ExecuteAsync(sql, shop, transaction);

            transaction.Commit();
            _logger.LogInfo($"更新商店成功: {shop.ShopId}");
            return shop;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError($"更新商店失敗: {shop.ShopId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string shopId)
    {
        try
        {
            const string sql = @"
                UPDATE Shops SET
                    Status = 'I',
                    UpdatedAt = GETDATE()
                WHERE ShopId = @ShopId";

            await ExecuteAsync(sql, new { ShopId = shopId });
            _logger.LogInfo($"刪除商店成功: {shopId}");
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
                SELECT COUNT(*) FROM Shops
                WHERE ShopId = @ShopId";

            var count = await ExecuteScalarAsync<int>(sql, new { ShopId = shopId });
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
                UPDATE Shops SET
                    Status = @Status,
                    UpdatedAt = GETDATE()
                WHERE ShopId = @ShopId";

            await ExecuteAsync(sql, new { ShopId = shopId, Status = status });
            _logger.LogInfo($"更新商店狀態成功: {shopId}, 狀態: {status}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新商店狀態失敗: {shopId}", ex);
            throw;
        }
    }
}

