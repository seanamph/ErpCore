using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Procurement;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Procurement;

/// <summary>
/// 採購擴展維護 Repository 實作 (SYSPA10-SYSPB60)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class PurchaseExtendedMaintenanceRepository : BaseRepository, IPurchaseExtendedMaintenanceRepository
{
    public PurchaseExtendedMaintenanceRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PurchaseExtendedMaintenance?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM PurchaseExtendedMaintenance 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<PurchaseExtendedMaintenance>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢採購擴展維護失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<PurchaseExtendedMaintenance?> GetByMaintenanceIdAsync(string maintenanceId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM PurchaseExtendedMaintenance 
                WHERE MaintenanceId = @MaintenanceId";

            return await QueryFirstOrDefaultAsync<PurchaseExtendedMaintenance>(sql, new { MaintenanceId = maintenanceId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢採購擴展維護失敗: {maintenanceId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<PurchaseExtendedMaintenance>> QueryAsync(PurchaseExtendedMaintenanceQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM PurchaseExtendedMaintenance 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.MaintenanceId))
            {
                sql += " AND MaintenanceId LIKE @MaintenanceId";
                parameters.Add("MaintenanceId", $"%{query.MaintenanceId}%");
            }

            if (!string.IsNullOrEmpty(query.MaintenanceName))
            {
                sql += " AND MaintenanceName LIKE @MaintenanceName";
                parameters.Add("MaintenanceName", $"%{query.MaintenanceName}%");
            }

            if (!string.IsNullOrEmpty(query.MaintenanceType))
            {
                sql += " AND MaintenanceType = @MaintenanceType";
                parameters.Add("MaintenanceType", query.MaintenanceType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            sql += " ORDER BY SeqNo, MaintenanceId";
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<PurchaseExtendedMaintenance>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢採購擴展維護列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(PurchaseExtendedMaintenanceQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM PurchaseExtendedMaintenance 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.MaintenanceId))
            {
                sql += " AND MaintenanceId LIKE @MaintenanceId";
                parameters.Add("MaintenanceId", $"%{query.MaintenanceId}%");
            }

            if (!string.IsNullOrEmpty(query.MaintenanceName))
            {
                sql += " AND MaintenanceName LIKE @MaintenanceName";
                parameters.Add("MaintenanceName", $"%{query.MaintenanceName}%");
            }

            if (!string.IsNullOrEmpty(query.MaintenanceType))
            {
                sql += " AND MaintenanceType = @MaintenanceType";
                parameters.Add("MaintenanceType", query.MaintenanceType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            return await ExecuteScalarAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢採購擴展維護數量失敗", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string maintenanceId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM PurchaseExtendedMaintenance 
                WHERE MaintenanceId = @MaintenanceId";

            var count = await ExecuteScalarAsync<int>(sql, new { MaintenanceId = maintenanceId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查採購擴展維護是否存在失敗: {maintenanceId}", ex);
            throw;
        }
    }

    public async Task<PurchaseExtendedMaintenance> CreateAsync(PurchaseExtendedMaintenance purchaseExtendedMaintenance)
    {
        try
        {
            const string sql = @"
                INSERT INTO PurchaseExtendedMaintenance (
                    MaintenanceId, MaintenanceName, MaintenanceType, MaintenanceDesc, MaintenanceConfig, ParameterConfig,
                    Status, SeqNo, Memo,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                ) VALUES (
                    @MaintenanceId, @MaintenanceName, @MaintenanceType, @MaintenanceDesc, @MaintenanceConfig, @ParameterConfig,
                    @Status, @SeqNo, @Memo,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, new
            {
                purchaseExtendedMaintenance.MaintenanceId,
                purchaseExtendedMaintenance.MaintenanceName,
                purchaseExtendedMaintenance.MaintenanceType,
                purchaseExtendedMaintenance.MaintenanceDesc,
                purchaseExtendedMaintenance.MaintenanceConfig,
                purchaseExtendedMaintenance.ParameterConfig,
                purchaseExtendedMaintenance.Status,
                purchaseExtendedMaintenance.SeqNo,
                purchaseExtendedMaintenance.Memo,
                purchaseExtendedMaintenance.CreatedBy,
                purchaseExtendedMaintenance.CreatedAt,
                purchaseExtendedMaintenance.UpdatedBy,
                purchaseExtendedMaintenance.UpdatedAt
            });

            purchaseExtendedMaintenance.TKey = tKey;
            return purchaseExtendedMaintenance;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增採購擴展維護失敗: {purchaseExtendedMaintenance.MaintenanceId}", ex);
            throw;
        }
    }

    public async Task<PurchaseExtendedMaintenance> UpdateAsync(PurchaseExtendedMaintenance purchaseExtendedMaintenance)
    {
        try
        {
            const string sql = @"
                UPDATE PurchaseExtendedMaintenance SET
                    MaintenanceName = @MaintenanceName,
                    MaintenanceType = @MaintenanceType,
                    MaintenanceDesc = @MaintenanceDesc,
                    MaintenanceConfig = @MaintenanceConfig,
                    ParameterConfig = @ParameterConfig,
                    Status = @Status,
                    SeqNo = @SeqNo,
                    Memo = @Memo,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new
            {
                purchaseExtendedMaintenance.TKey,
                purchaseExtendedMaintenance.MaintenanceName,
                purchaseExtendedMaintenance.MaintenanceType,
                purchaseExtendedMaintenance.MaintenanceDesc,
                purchaseExtendedMaintenance.MaintenanceConfig,
                purchaseExtendedMaintenance.ParameterConfig,
                purchaseExtendedMaintenance.Status,
                purchaseExtendedMaintenance.SeqNo,
                purchaseExtendedMaintenance.Memo,
                purchaseExtendedMaintenance.UpdatedBy,
                purchaseExtendedMaintenance.UpdatedAt
            });

            return purchaseExtendedMaintenance;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改採購擴展維護失敗: {purchaseExtendedMaintenance.MaintenanceId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM PurchaseExtendedMaintenance 
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除採購擴展維護失敗: {tKey}", ex);
            throw;
        }
    }
}
