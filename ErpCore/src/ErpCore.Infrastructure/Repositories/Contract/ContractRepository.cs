using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Contract;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Contract;

/// <summary>
/// 合同 Repository 實作 (SYSF110-SYSF140)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class ContractRepository : BaseRepository, IContractRepository
{
    public ContractRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Contract?> GetByIdAsync(string contractId, int version)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Contracts 
                WHERE ContractId = @ContractId AND Version = @Version";

            return await QueryFirstOrDefaultAsync<Contract>(sql, new { ContractId = contractId, Version = version });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢合同失敗: {contractId}, Version: {version}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<Contract>> QueryAsync(ContractQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Contracts 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ContractId))
            {
                sql += " AND ContractId LIKE @ContractId";
                parameters.Add("ContractId", $"%{query.ContractId}%");
            }

            if (!string.IsNullOrEmpty(query.ContractType))
            {
                sql += " AND ContractType = @ContractType";
                parameters.Add("ContractType", query.ContractType);
            }

            if (!string.IsNullOrEmpty(query.VendorId))
            {
                sql += " AND VendorId LIKE @VendorId";
                parameters.Add("VendorId", $"%{query.VendorId}%");
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (query.EffectiveDateFrom.HasValue)
            {
                sql += " AND EffectiveDate >= @EffectiveDateFrom";
                parameters.Add("EffectiveDateFrom", query.EffectiveDateFrom);
            }

            if (query.EffectiveDateTo.HasValue)
            {
                sql += " AND EffectiveDate <= @EffectiveDateTo";
                parameters.Add("EffectiveDateTo", query.EffectiveDateTo);
            }

            sql += " ORDER BY ContractId, Version DESC";
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<Contract>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢合同列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(ContractQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM Contracts 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ContractId))
            {
                sql += " AND ContractId LIKE @ContractId";
                parameters.Add("ContractId", $"%{query.ContractId}%");
            }

            if (!string.IsNullOrEmpty(query.ContractType))
            {
                sql += " AND ContractType = @ContractType";
                parameters.Add("ContractType", query.ContractType);
            }

            if (!string.IsNullOrEmpty(query.VendorId))
            {
                sql += " AND VendorId LIKE @VendorId";
                parameters.Add("VendorId", $"%{query.VendorId}%");
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (query.EffectiveDateFrom.HasValue)
            {
                sql += " AND EffectiveDate >= @EffectiveDateFrom";
                parameters.Add("EffectiveDateFrom", query.EffectiveDateFrom);
            }

            if (query.EffectiveDateTo.HasValue)
            {
                sql += " AND EffectiveDate <= @EffectiveDateTo";
                parameters.Add("EffectiveDateTo", query.EffectiveDateTo);
            }

            return await ExecuteScalarAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢合同數量失敗", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string contractId, int version)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM Contracts 
                WHERE ContractId = @ContractId AND Version = @Version";

            var count = await ExecuteScalarAsync<int>(sql, new { ContractId = contractId, Version = version });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查合同是否存在失敗: {contractId}, Version: {version}", ex);
            throw;
        }
    }

    public async Task<Contract> CreateAsync(Contract contract)
    {
        try
        {
            const string sql = @"
                INSERT INTO Contracts (
                    ContractId, ContractType, Version, VendorId, VendorName, SignDate, EffectiveDate, ExpiryDate,
                    Status, TotalAmount, CurrencyId, ExchangeRate, LocationId, RecruitId, Attorney, Salutation,
                    VerStatus, AgmStatus, Memo,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                ) VALUES (
                    @ContractId, @ContractType, @Version, @VendorId, @VendorName, @SignDate, @EffectiveDate, @ExpiryDate,
                    @Status, @TotalAmount, @CurrencyId, @ExchangeRate, @LocationId, @RecruitId, @Attorney, @Salutation,
                    @VerStatus, @AgmStatus, @Memo,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, new
            {
                contract.ContractId,
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
                contract.RecruitId,
                contract.Attorney,
                contract.Salutation,
                contract.VerStatus,
                contract.AgmStatus,
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
            _logger.LogError($"新增合同失敗: {contract.ContractId}, Version: {contract.Version}", ex);
            throw;
        }
    }

    public async Task<Contract> UpdateAsync(Contract contract)
    {
        try
        {
            const string sql = @"
                UPDATE Contracts SET
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
                    RecruitId = @RecruitId,
                    Attorney = @Attorney,
                    Salutation = @Salutation,
                    VerStatus = @VerStatus,
                    AgmStatus = @AgmStatus,
                    Memo = @Memo,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE ContractId = @ContractId AND Version = @Version";

            await ExecuteAsync(sql, new
            {
                contract.ContractId,
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
                contract.RecruitId,
                contract.Attorney,
                contract.Salutation,
                contract.VerStatus,
                contract.AgmStatus,
                contract.Memo,
                contract.UpdatedBy,
                contract.UpdatedAt
            });

            return contract;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改合同失敗: {contract.ContractId}, Version: {contract.Version}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string contractId, int version)
    {
        try
        {
            const string sql = @"
                DELETE FROM Contracts 
                WHERE ContractId = @ContractId AND Version = @Version";

            await ExecuteAsync(sql, new { ContractId = contractId, Version = version });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除合同失敗: {contractId}, Version: {version}", ex);
            throw;
        }
    }

    public async Task<int> GetNextVersionAsync(string contractId)
    {
        try
        {
            const string sql = @"
                SELECT ISNULL(MAX(Version), 0) + 1 FROM Contracts 
                WHERE ContractId = @ContractId";

            return await ExecuteScalarAsync<int>(sql, new { ContractId = contractId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"取得合同下一版本號失敗: {contractId}", ex);
            throw;
        }
    }
}

