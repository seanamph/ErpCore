using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Contract;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Contract;

/// <summary>
/// 合同處理 Repository 實作 (SYSF210-SYSF220)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class ContractProcessRepository : BaseRepository, IContractProcessRepository
{
    public ContractProcessRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<ContractProcess?> GetByIdAsync(string processId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM ContractProcesses 
                WHERE ProcessId = @ProcessId";

            return await QueryFirstOrDefaultAsync<ContractProcess>(sql, new { ProcessId = processId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢合同處理失敗: {processId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<ContractProcess>> QueryAsync(ContractProcessQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM ContractProcesses 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ProcessId))
            {
                sql += " AND ProcessId LIKE @ProcessId";
                parameters.Add("ProcessId", $"%{query.ProcessId}%");
            }

            if (!string.IsNullOrEmpty(query.ContractId))
            {
                sql += " AND ContractId LIKE @ContractId";
                parameters.Add("ContractId", $"%{query.ContractId}%");
            }

            if (query.Version.HasValue)
            {
                sql += " AND Version = @Version";
                parameters.Add("Version", query.Version);
            }

            if (!string.IsNullOrEmpty(query.ProcessType))
            {
                sql += " AND ProcessType = @ProcessType";
                parameters.Add("ProcessType", query.ProcessType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (query.ProcessDateFrom.HasValue)
            {
                sql += " AND ProcessDate >= @ProcessDateFrom";
                parameters.Add("ProcessDateFrom", query.ProcessDateFrom);
            }

            if (query.ProcessDateTo.HasValue)
            {
                sql += " AND ProcessDate <= @ProcessDateTo";
                parameters.Add("ProcessDateTo", query.ProcessDateTo);
            }

            sql += " ORDER BY ProcessDate DESC, ProcessId";
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<ContractProcess>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢合同處理列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(ContractProcessQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM ContractProcesses 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ProcessId))
            {
                sql += " AND ProcessId LIKE @ProcessId";
                parameters.Add("ProcessId", $"%{query.ProcessId}%");
            }

            if (!string.IsNullOrEmpty(query.ContractId))
            {
                sql += " AND ContractId LIKE @ContractId";
                parameters.Add("ContractId", $"%{query.ContractId}%");
            }

            if (query.Version.HasValue)
            {
                sql += " AND Version = @Version";
                parameters.Add("Version", query.Version);
            }

            if (!string.IsNullOrEmpty(query.ProcessType))
            {
                sql += " AND ProcessType = @ProcessType";
                parameters.Add("ProcessType", query.ProcessType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (query.ProcessDateFrom.HasValue)
            {
                sql += " AND ProcessDate >= @ProcessDateFrom";
                parameters.Add("ProcessDateFrom", query.ProcessDateFrom);
            }

            if (query.ProcessDateTo.HasValue)
            {
                sql += " AND ProcessDate <= @ProcessDateTo";
                parameters.Add("ProcessDateTo", query.ProcessDateTo);
            }

            return await ExecuteScalarAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢合同處理數量失敗", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string processId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM ContractProcesses 
                WHERE ProcessId = @ProcessId";

            var count = await ExecuteScalarAsync<int>(sql, new { ProcessId = processId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查合同處理是否存在失敗: {processId}", ex);
            throw;
        }
    }

    public async Task<ContractProcess> CreateAsync(ContractProcess process)
    {
        try
        {
            const string sql = @"
                INSERT INTO ContractProcesses (
                    ProcessId, ContractId, Version, ProcessType, ProcessDate, ProcessAmount,
                    Status, ProcessUserId, ProcessMemo, SiteId, OrgId,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                ) VALUES (
                    @ProcessId, @ContractId, @Version, @ProcessType, @ProcessDate, @ProcessAmount,
                    @Status, @ProcessUserId, @ProcessMemo, @SiteId, @OrgId,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, new
            {
                process.ProcessId,
                process.ContractId,
                process.Version,
                process.ProcessType,
                process.ProcessDate,
                process.ProcessAmount,
                process.Status,
                process.ProcessUserId,
                process.ProcessMemo,
                process.SiteId,
                process.OrgId,
                process.CreatedBy,
                process.CreatedAt,
                process.UpdatedBy,
                process.UpdatedAt
            });

            process.TKey = tKey;
            return process;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增合同處理失敗: {process.ProcessId}", ex);
            throw;
        }
    }

    public async Task<ContractProcess> UpdateAsync(ContractProcess process)
    {
        try
        {
            const string sql = @"
                UPDATE ContractProcesses SET
                    ContractId = @ContractId,
                    Version = @Version,
                    ProcessType = @ProcessType,
                    ProcessDate = @ProcessDate,
                    ProcessAmount = @ProcessAmount,
                    Status = @Status,
                    ProcessUserId = @ProcessUserId,
                    ProcessMemo = @ProcessMemo,
                    SiteId = @SiteId,
                    OrgId = @OrgId,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE ProcessId = @ProcessId";

            await ExecuteAsync(sql, new
            {
                process.ProcessId,
                process.ContractId,
                process.Version,
                process.ProcessType,
                process.ProcessDate,
                process.ProcessAmount,
                process.Status,
                process.ProcessUserId,
                process.ProcessMemo,
                process.SiteId,
                process.OrgId,
                process.UpdatedBy,
                process.UpdatedAt
            });

            return process;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改合同處理失敗: {process.ProcessId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string processId)
    {
        try
        {
            const string sql = @"
                DELETE FROM ContractProcesses 
                WHERE ProcessId = @ProcessId";

            await ExecuteAsync(sql, new { ProcessId = processId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除合同處理失敗: {processId}", ex);
            throw;
        }
    }
}

