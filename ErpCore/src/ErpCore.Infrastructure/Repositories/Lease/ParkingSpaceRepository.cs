using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Lease;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Lease;

/// <summary>
/// 停車位資料 Repository 實作 (SYSM111-SYSM138)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class ParkingSpaceRepository : BaseRepository, IParkingSpaceRepository
{
    public ParkingSpaceRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<ParkingSpace?> GetByIdAsync(string parkingSpaceId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM ParkingSpaces 
                WHERE ParkingSpaceId = @ParkingSpaceId";

            return await QueryFirstOrDefaultAsync<ParkingSpace>(sql, new { ParkingSpaceId = parkingSpaceId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢停車位失敗: {parkingSpaceId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<ParkingSpace>> QueryAsync(ParkingSpaceQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM ParkingSpaces 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ParkingSpaceId))
            {
                sql += " AND ParkingSpaceId LIKE @ParkingSpaceId";
                parameters.Add("ParkingSpaceId", $"%{query.ParkingSpaceId}%");
            }

            if (!string.IsNullOrEmpty(query.ShopId))
            {
                sql += " AND ShopId = @ShopId";
                parameters.Add("ShopId", query.ShopId);
            }

            if (!string.IsNullOrEmpty(query.FloorId))
            {
                sql += " AND FloorId = @FloorId";
                parameters.Add("FloorId", query.FloorId);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (!string.IsNullOrEmpty(query.LeaseId))
            {
                sql += " AND LeaseId = @LeaseId";
                parameters.Add("LeaseId", query.LeaseId);
            }

            sql += " ORDER BY ParkingSpaceId";
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<ParkingSpace>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢停車位列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(ParkingSpaceQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM ParkingSpaces 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ParkingSpaceId))
            {
                sql += " AND ParkingSpaceId LIKE @ParkingSpaceId";
                parameters.Add("ParkingSpaceId", $"%{query.ParkingSpaceId}%");
            }

            if (!string.IsNullOrEmpty(query.ShopId))
            {
                sql += " AND ShopId = @ShopId";
                parameters.Add("ShopId", query.ShopId);
            }

            if (!string.IsNullOrEmpty(query.FloorId))
            {
                sql += " AND FloorId = @FloorId";
                parameters.Add("FloorId", query.FloorId);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (!string.IsNullOrEmpty(query.LeaseId))
            {
                sql += " AND LeaseId = @LeaseId";
                parameters.Add("LeaseId", query.LeaseId);
            }

            return await ExecuteScalarAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢停車位數量失敗", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string parkingSpaceId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM ParkingSpaces 
                WHERE ParkingSpaceId = @ParkingSpaceId";

            var count = await ExecuteScalarAsync<int>(sql, new { ParkingSpaceId = parkingSpaceId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查停車位是否存在失敗: {parkingSpaceId}", ex);
            throw;
        }
    }

    public async Task<ParkingSpace> CreateAsync(ParkingSpace parkingSpace)
    {
        try
        {
            const string sql = @"
                INSERT INTO ParkingSpaces (
                    ParkingSpaceId, ParkingSpaceNo, ShopId, FloorId, Area, Status, LeaseId, Memo,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                ) VALUES (
                    @ParkingSpaceId, @ParkingSpaceNo, @ShopId, @FloorId, @Area, @Status, @LeaseId, @Memo,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, new
            {
                parkingSpace.ParkingSpaceId,
                parkingSpace.ParkingSpaceNo,
                parkingSpace.ShopId,
                parkingSpace.FloorId,
                parkingSpace.Area,
                parkingSpace.Status,
                parkingSpace.LeaseId,
                parkingSpace.Memo,
                parkingSpace.CreatedBy,
                parkingSpace.CreatedAt,
                parkingSpace.UpdatedBy,
                parkingSpace.UpdatedAt
            });

            parkingSpace.TKey = tKey;
            return parkingSpace;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增停車位失敗: {parkingSpace.ParkingSpaceId}", ex);
            throw;
        }
    }

    public async Task<ParkingSpace> UpdateAsync(ParkingSpace parkingSpace)
    {
        try
        {
            const string sql = @"
                UPDATE ParkingSpaces SET
                    ParkingSpaceNo = @ParkingSpaceNo,
                    ShopId = @ShopId,
                    FloorId = @FloorId,
                    Area = @Area,
                    Status = @Status,
                    LeaseId = @LeaseId,
                    Memo = @Memo,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE ParkingSpaceId = @ParkingSpaceId";

            await ExecuteAsync(sql, new
            {
                parkingSpace.ParkingSpaceId,
                parkingSpace.ParkingSpaceNo,
                parkingSpace.ShopId,
                parkingSpace.FloorId,
                parkingSpace.Area,
                parkingSpace.Status,
                parkingSpace.LeaseId,
                parkingSpace.Memo,
                parkingSpace.UpdatedBy,
                parkingSpace.UpdatedAt
            });

            return parkingSpace;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改停車位失敗: {parkingSpace.ParkingSpaceId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string parkingSpaceId)
    {
        try
        {
            const string sql = @"
                DELETE FROM ParkingSpaces 
                WHERE ParkingSpaceId = @ParkingSpaceId";

            await ExecuteAsync(sql, new { ParkingSpaceId = parkingSpaceId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除停車位失敗: {parkingSpaceId}", ex);
            throw;
        }
    }

    public async Task UpdateStatusAsync(string parkingSpaceId, string status)
    {
        try
        {
            const string sql = @"
                UPDATE ParkingSpaces SET
                    Status = @Status,
                    UpdatedAt = GETDATE()
                WHERE ParkingSpaceId = @ParkingSpaceId";

            await ExecuteAsync(sql, new { ParkingSpaceId = parkingSpaceId, Status = status });
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新停車位狀態失敗: {parkingSpaceId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<ParkingSpace>> GetAvailableParkingSpacesAsync(string? shopId)
    {
        try
        {
            var sql = @"
                SELECT * FROM ParkingSpaces 
                WHERE Status = 'A'";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(shopId))
            {
                sql += " AND ShopId = @ShopId";
                parameters.Add("ShopId", shopId);
            }

            sql += " ORDER BY ParkingSpaceId";

            return await QueryAsync<ParkingSpace>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢可用停車位失敗: {shopId}", ex);
            throw;
        }
    }
}

