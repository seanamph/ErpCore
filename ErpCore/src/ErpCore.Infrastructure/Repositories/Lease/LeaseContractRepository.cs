using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Lease;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Lease;

/// <summary>
/// 租賃合同資料 Repository 實作 (SYSM111-SYSM138)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class LeaseContractRepository : BaseRepository, ILeaseContractRepository
{
    public LeaseContractRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<LeaseContract?> GetByIdAsync(string contractNo)
    {
        try
        {
            const string sql = @"
                SELECT * FROM LeaseContracts 
                WHERE ContractNo = @ContractNo";

            return await QueryFirstOrDefaultAsync<LeaseContract>(sql, new { ContractNo = contractNo });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢租賃合同失敗: {contractNo}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<LeaseContract>> GetByLeaseIdAsync(string leaseId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM LeaseContracts 
                WHERE LeaseId = @LeaseId
                ORDER BY ContractDate DESC";

            return await QueryAsync<LeaseContract>(sql, new { LeaseId = leaseId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"根據租賃編號查詢合同失敗: {leaseId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<LeaseContract>> QueryAsync(LeaseContractQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM LeaseContracts 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ContractNo))
            {
                sql += " AND ContractNo LIKE @ContractNo";
                parameters.Add("ContractNo", $"%{query.ContractNo}%");
            }

            if (!string.IsNullOrEmpty(query.LeaseId))
            {
                sql += " AND LeaseId LIKE @LeaseId";
                parameters.Add("LeaseId", $"%{query.LeaseId}%");
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

            if (query.ContractDateFrom.HasValue)
            {
                sql += " AND ContractDate >= @ContractDateFrom";
                parameters.Add("ContractDateFrom", query.ContractDateFrom);
            }

            if (query.ContractDateTo.HasValue)
            {
                sql += " AND ContractDate <= @ContractDateTo";
                parameters.Add("ContractDateTo", query.ContractDateTo);
            }

            sql += " ORDER BY ContractDate DESC, ContractNo";
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<LeaseContract>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢租賃合同列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(LeaseContractQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM LeaseContracts 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ContractNo))
            {
                sql += " AND ContractNo LIKE @ContractNo";
                parameters.Add("ContractNo", $"%{query.ContractNo}%");
            }

            if (!string.IsNullOrEmpty(query.LeaseId))
            {
                sql += " AND LeaseId LIKE @LeaseId";
                parameters.Add("LeaseId", $"%{query.LeaseId}%");
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

            if (query.ContractDateFrom.HasValue)
            {
                sql += " AND ContractDate >= @ContractDateFrom";
                parameters.Add("ContractDateFrom", query.ContractDateFrom);
            }

            if (query.ContractDateTo.HasValue)
            {
                sql += " AND ContractDate <= @ContractDateTo";
                parameters.Add("ContractDateTo", query.ContractDateTo);
            }

            return await ExecuteScalarAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢租賃合同數量失敗", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string contractNo)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM LeaseContracts 
                WHERE ContractNo = @ContractNo";

            var count = await ExecuteScalarAsync<int>(sql, new { ContractNo = contractNo });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查租賃合同是否存在失敗: {contractNo}", ex);
            throw;
        }
    }

    public async Task<LeaseContract> CreateAsync(LeaseContract contract)
    {
        try
        {
            const string sql = @"
                INSERT INTO LeaseContracts (
                    ContractNo, LeaseId, ContractDate, ContractType, ContractContent, Status,
                    SignedBy, SignedDate, Memo,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                ) VALUES (
                    @ContractNo, @LeaseId, @ContractDate, @ContractType, @ContractContent, @Status,
                    @SignedBy, @SignedDate, @Memo,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, new
            {
                contract.ContractNo,
                contract.LeaseId,
                contract.ContractDate,
                contract.ContractType,
                contract.ContractContent,
                contract.Status,
                contract.SignedBy,
                contract.SignedDate,
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
            _logger.LogError($"新增租賃合同失敗: {contract.ContractNo}", ex);
            throw;
        }
    }

    public async Task<LeaseContract> UpdateAsync(LeaseContract contract)
    {
        try
        {
            const string sql = @"
                UPDATE LeaseContracts SET
                    ContractDate = @ContractDate,
                    ContractType = @ContractType,
                    ContractContent = @ContractContent,
                    Status = @Status,
                    SignedBy = @SignedBy,
                    SignedDate = @SignedDate,
                    Memo = @Memo,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE ContractNo = @ContractNo";

            await ExecuteAsync(sql, new
            {
                contract.ContractNo,
                contract.ContractDate,
                contract.ContractType,
                contract.ContractContent,
                contract.Status,
                contract.SignedBy,
                contract.SignedDate,
                contract.Memo,
                contract.UpdatedBy,
                contract.UpdatedAt
            });

            return contract;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改租賃合同失敗: {contract.ContractNo}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string contractNo)
    {
        try
        {
            const string sql = @"
                DELETE FROM LeaseContracts 
                WHERE ContractNo = @ContractNo";

            await ExecuteAsync(sql, new { ContractNo = contractNo });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除租賃合同失敗: {contractNo}", ex);
            throw;
        }
    }

    public async Task UpdateStatusAsync(string contractNo, string status)
    {
        try
        {
            const string sql = @"
                UPDATE LeaseContracts SET
                    Status = @Status,
                    UpdatedAt = GETDATE()
                WHERE ContractNo = @ContractNo";

            await ExecuteAsync(sql, new { ContractNo = contractNo, Status = status });
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新租賃合同狀態失敗: {contractNo}", ex);
            throw;
        }
    }
}

