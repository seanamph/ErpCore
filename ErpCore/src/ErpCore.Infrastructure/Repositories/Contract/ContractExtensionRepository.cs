using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Contract;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Contract;

/// <summary>
/// 合同擴展 Repository 實作 (SYSF350-SYSF540)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class ContractExtensionRepository : BaseRepository, IContractExtensionRepository
{
    public ContractExtensionRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<ContractExtension?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM ContractExtensions 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<ContractExtension>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢合同擴展失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<ContractExtension>> QueryAsync(ContractExtensionQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM ContractExtensions 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ContractId))
            {
                sql += " AND ContractId LIKE @ContractId";
                parameters.Add("ContractId", $"%{query.ContractId}%");
            }

            if (!string.IsNullOrEmpty(query.VendorId))
            {
                sql += " AND VendorId LIKE @VendorId";
                parameters.Add("VendorId", $"%{query.VendorId}%");
            }

            if (!string.IsNullOrEmpty(query.ExtensionType))
            {
                sql += " AND ExtensionType = @ExtensionType";
                parameters.Add("ExtensionType", query.ExtensionType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            sql += " ORDER BY ContractId, Version DESC";
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<ContractExtension>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢合同擴展列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(ContractExtensionQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM ContractExtensions 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ContractId))
            {
                sql += " AND ContractId LIKE @ContractId";
                parameters.Add("ContractId", $"%{query.ContractId}%");
            }

            if (!string.IsNullOrEmpty(query.VendorId))
            {
                sql += " AND VendorId LIKE @VendorId";
                parameters.Add("VendorId", $"%{query.VendorId}%");
            }

            if (!string.IsNullOrEmpty(query.ExtensionType))
            {
                sql += " AND ExtensionType = @ExtensionType";
                parameters.Add("ExtensionType", query.ExtensionType);
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
            _logger.LogError("查詢合同擴展數量失敗", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM ContractExtensions 
                WHERE TKey = @TKey";

            var count = await ExecuteScalarAsync<int>(sql, new { TKey = tKey });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查合同擴展是否存在失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<ContractExtension> CreateAsync(ContractExtension extension)
    {
        try
        {
            const string sql = @"
                INSERT INTO ContractExtensions (
                    ContractId, Version, ExtensionType, VendorId, VendorName, ExtensionDate,
                    ExtensionAmount, Status, Memo,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                ) VALUES (
                    @ContractId, @Version, @ExtensionType, @VendorId, @VendorName, @ExtensionDate,
                    @ExtensionAmount, @Status, @Memo,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, new
            {
                extension.ContractId,
                extension.Version,
                extension.ExtensionType,
                extension.VendorId,
                extension.VendorName,
                extension.ExtensionDate,
                extension.ExtensionAmount,
                extension.Status,
                extension.Memo,
                extension.CreatedBy,
                extension.CreatedAt,
                extension.UpdatedBy,
                extension.UpdatedAt
            });

            extension.TKey = tKey;
            return extension;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增合同擴展失敗: {extension.ContractId}, Version: {extension.Version}", ex);
            throw;
        }
    }

    public async Task<ContractExtension> UpdateAsync(ContractExtension extension)
    {
        try
        {
            const string sql = @"
                UPDATE ContractExtensions SET
                    ExtensionType = @ExtensionType,
                    VendorId = @VendorId,
                    VendorName = @VendorName,
                    ExtensionDate = @ExtensionDate,
                    ExtensionAmount = @ExtensionAmount,
                    Status = @Status,
                    Memo = @Memo,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new
            {
                extension.TKey,
                extension.ExtensionType,
                extension.VendorId,
                extension.VendorName,
                extension.ExtensionDate,
                extension.ExtensionAmount,
                extension.Status,
                extension.Memo,
                extension.UpdatedBy,
                extension.UpdatedAt
            });

            return extension;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改合同擴展失敗: {extension.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM ContractExtensions 
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除合同擴展失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<int> BatchDeleteAsync(List<long> tKeys)
    {
        try
        {
            if (tKeys == null || tKeys.Count == 0)
                return 0;

            const string sql = @"
                DELETE FROM ContractExtensions 
                WHERE TKey IN @TKeys";

            return await ExecuteAsync(sql, new { TKeys = tKeys });
        }
        catch (Exception ex)
        {
            _logger.LogError($"批次刪除合同擴展失敗", ex);
            throw;
        }
    }
}

