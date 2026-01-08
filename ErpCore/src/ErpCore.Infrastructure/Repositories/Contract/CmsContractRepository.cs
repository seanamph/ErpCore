using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Contract;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Contract;

/// <summary>
/// CMS合同 Repository 實作 (CMS2310-CMS2320)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class CmsContractRepository : BaseRepository, ICmsContractRepository
{
    public CmsContractRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<CmsContract?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM CmsContracts 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<CmsContract>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢CMS合同失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<CmsContract?> GetByContractIdAsync(string cmsContractId, int version)
    {
        try
        {
            const string sql = @"
                SELECT * FROM CmsContracts 
                WHERE CmsContractId = @CmsContractId AND Version = @Version";

            return await QueryFirstOrDefaultAsync<CmsContract>(sql, new { CmsContractId = cmsContractId, Version = version });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢CMS合同失敗: {cmsContractId}, Version: {version}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<CmsContract>> QueryAsync(CmsContractQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM CmsContracts 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.CmsContractId))
            {
                sql += " AND CmsContractId LIKE @CmsContractId";
                parameters.Add("CmsContractId", $"%{query.CmsContractId}%");
            }

            if (!string.IsNullOrEmpty(query.VendorId))
            {
                sql += " AND VendorId LIKE @VendorId";
                parameters.Add("VendorId", $"%{query.VendorId}%");
            }

            if (!string.IsNullOrEmpty(query.ContractType))
            {
                sql += " AND ContractType = @ContractType";
                parameters.Add("ContractType", query.ContractType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            sql += " ORDER BY CmsContractId, Version DESC";
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<CmsContract>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢CMS合同列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(CmsContractQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM CmsContracts 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.CmsContractId))
            {
                sql += " AND CmsContractId LIKE @CmsContractId";
                parameters.Add("CmsContractId", $"%{query.CmsContractId}%");
            }

            if (!string.IsNullOrEmpty(query.VendorId))
            {
                sql += " AND VendorId LIKE @VendorId";
                parameters.Add("VendorId", $"%{query.VendorId}%");
            }

            if (!string.IsNullOrEmpty(query.ContractType))
            {
                sql += " AND ContractType = @ContractType";
                parameters.Add("ContractType", query.ContractType);
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
            _logger.LogError("查詢CMS合同數量失敗", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string cmsContractId, int version)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM CmsContracts 
                WHERE CmsContractId = @CmsContractId AND Version = @Version";

            var count = await ExecuteScalarAsync<int>(sql, new { CmsContractId = cmsContractId, Version = version });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查CMS合同是否存在失敗: {cmsContractId}, Version: {version}", ex);
            throw;
        }
    }

    public async Task<CmsContract> CreateAsync(CmsContract contract)
    {
        try
        {
            const string sql = @"
                INSERT INTO CmsContracts (
                    CmsContractId, ContractType, Version, VendorId, VendorName, SignDate,
                    EffectiveDate, ExpiryDate, Status, TotalAmount, CurrencyId, ExchangeRate,
                    LocationId, Memo,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                ) VALUES (
                    @CmsContractId, @ContractType, @Version, @VendorId, @VendorName, @SignDate,
                    @EffectiveDate, @ExpiryDate, @Status, @TotalAmount, @CurrencyId, @ExchangeRate,
                    @LocationId, @Memo,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, new
            {
                contract.CmsContractId,
                contract.ContractType,
                contract.Version,
                contract.VendorId,
                contract.VendorName,
                contract.SignDate,
                contract.EffectiveDate,
                contract.ExpiryDate,
                contract.Status,
                contract.TotalAmount,
                contract.CurrencyId,
                contract.ExchangeRate,
                contract.LocationId,
                contract.Memo,
                contract.CreatedBy,
                contract.CreatedAt,
                contract.UpdatedBy,
                contract.UpdatedAt
            });

            contract.TKey = tKey;
            return contract;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增CMS合同失敗: {contract.CmsContractId}, Version: {contract.Version}", ex);
            throw;
        }
    }

    public async Task<CmsContract> UpdateAsync(CmsContract contract)
    {
        try
        {
            const string sql = @"
                UPDATE CmsContracts SET
                    ContractType = @ContractType,
                    VendorId = @VendorId,
                    VendorName = @VendorName,
                    SignDate = @SignDate,
                    EffectiveDate = @EffectiveDate,
                    ExpiryDate = @ExpiryDate,
                    Status = @Status,
                    TotalAmount = @TotalAmount,
                    CurrencyId = @CurrencyId,
                    ExchangeRate = @ExchangeRate,
                    LocationId = @LocationId,
                    Memo = @Memo,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new
            {
                contract.TKey,
                contract.ContractType,
                contract.VendorId,
                contract.VendorName,
                contract.SignDate,
                contract.EffectiveDate,
                contract.ExpiryDate,
                contract.Status,
                contract.TotalAmount,
                contract.CurrencyId,
                contract.ExchangeRate,
                contract.LocationId,
                contract.Memo,
                contract.UpdatedBy,
                contract.UpdatedAt
            });

            return contract;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改CMS合同失敗: {contract.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM CmsContracts 
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除CMS合同失敗: {tKey}", ex);
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
                DELETE FROM CmsContracts 
                WHERE TKey IN @TKeys";

            return await ExecuteAsync(sql, new { TKeys = tKeys });
        }
        catch (Exception ex)
        {
            _logger.LogError($"批次刪除CMS合同失敗", ex);
            throw;
        }
    }

    public async Task<int> GetNextVersionAsync(string cmsContractId)
    {
        try
        {
            const string sql = @"
                SELECT ISNULL(MAX(Version), 0) + 1 FROM CmsContracts 
                WHERE CmsContractId = @CmsContractId";

            return await ExecuteScalarAsync<int>(sql, new { CmsContractId = cmsContractId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"取得CMS合同下一版本號失敗: {cmsContractId}", ex);
            throw;
        }
    }
}

