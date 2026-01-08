using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Lease;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Lease;

/// <summary>
/// 租賃處理 Repository 實作 (SYS8B50-SYS8B90)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class LeaseProcessRepository : BaseRepository, ILeaseProcessRepository
{
    public LeaseProcessRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<LeaseProcess?> GetByIdAsync(string processId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM LeaseProcesses 
                WHERE ProcessId = @ProcessId";

            return await QueryFirstOrDefaultAsync<LeaseProcess>(sql, new { ProcessId = processId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢租賃處理失敗: {processId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<LeaseProcess>> QueryAsync(LeaseProcessQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM LeaseProcesses 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ProcessId))
            {
                sql += " AND ProcessId LIKE @ProcessId";
                parameters.Add("ProcessId", $"%{query.ProcessId}%");
            }

            if (!string.IsNullOrEmpty(query.LeaseId))
            {
                sql += " AND LeaseId = @LeaseId";
                parameters.Add("LeaseId", query.LeaseId);
            }

            if (!string.IsNullOrEmpty(query.ProcessType))
            {
                sql += " AND ProcessType = @ProcessType";
                parameters.Add("ProcessType", query.ProcessType);
            }

            if (!string.IsNullOrEmpty(query.ProcessStatus))
            {
                sql += " AND ProcessStatus = @ProcessStatus";
                parameters.Add("ProcessStatus", query.ProcessStatus);
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

            sql += " ORDER BY ProcessDate DESC, ProcessId DESC";
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<LeaseProcess>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢租賃處理列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(LeaseProcessQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM LeaseProcesses 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ProcessId))
            {
                sql += " AND ProcessId LIKE @ProcessId";
                parameters.Add("ProcessId", $"%{query.ProcessId}%");
            }

            if (!string.IsNullOrEmpty(query.LeaseId))
            {
                sql += " AND LeaseId = @LeaseId";
                parameters.Add("LeaseId", query.LeaseId);
            }

            if (!string.IsNullOrEmpty(query.ProcessType))
            {
                sql += " AND ProcessType = @ProcessType";
                parameters.Add("ProcessType", query.ProcessType);
            }

            if (!string.IsNullOrEmpty(query.ProcessStatus))
            {
                sql += " AND ProcessStatus = @ProcessStatus";
                parameters.Add("ProcessStatus", query.ProcessStatus);
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
            _logger.LogError("查詢租賃處理數量失敗", ex);
            throw;
        }
    }

    public async Task<IEnumerable<LeaseProcess>> GetByLeaseIdAsync(string leaseId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM LeaseProcesses 
                WHERE LeaseId = @LeaseId
                ORDER BY ProcessDate DESC, ProcessId DESC";

            return await QueryAsync<LeaseProcess>(sql, new { LeaseId = leaseId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"根據租賃編號查詢處理列表失敗: {leaseId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string processId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM LeaseProcesses 
                WHERE ProcessId = @ProcessId";

            var count = await ExecuteScalarAsync<int>(sql, new { ProcessId = processId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查租賃處理是否存在失敗: {processId}", ex);
            throw;
        }
    }

    public async Task<LeaseProcess> CreateAsync(LeaseProcess process)
    {
        try
        {
            const string sql = @"
                INSERT INTO LeaseProcesses (
                    ProcessId, LeaseId, ProcessType, ProcessDate, ProcessStatus, ProcessResult,
                    ProcessUserId, ProcessUserName, ProcessMemo,
                    ApprovalUserId, ApprovalDate, ApprovalStatus,
                    SiteId, OrgId,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                ) VALUES (
                    @ProcessId, @LeaseId, @ProcessType, @ProcessDate, @ProcessStatus, @ProcessResult,
                    @ProcessUserId, @ProcessUserName, @ProcessMemo,
                    @ApprovalUserId, @ApprovalDate, @ApprovalStatus,
                    @SiteId, @OrgId,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, new
            {
                process.ProcessId,
                process.LeaseId,
                process.ProcessType,
                process.ProcessDate,
                process.ProcessStatus,
                process.ProcessResult,
                process.ProcessUserId,
                process.ProcessUserName,
                process.ProcessMemo,
                process.ApprovalUserId,
                process.ApprovalDate,
                process.ApprovalStatus,
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
            _logger.LogError($"新增租賃處理失敗: {process.ProcessId}", ex);
            throw;
        }
    }

    public async Task<LeaseProcess> UpdateAsync(LeaseProcess process)
    {
        try
        {
            const string sql = @"
                UPDATE LeaseProcesses SET
                    LeaseId = @LeaseId,
                    ProcessType = @ProcessType,
                    ProcessDate = @ProcessDate,
                    ProcessStatus = @ProcessStatus,
                    ProcessResult = @ProcessResult,
                    ProcessUserId = @ProcessUserId,
                    ProcessUserName = @ProcessUserName,
                    ProcessMemo = @ProcessMemo,
                    ApprovalUserId = @ApprovalUserId,
                    ApprovalDate = @ApprovalDate,
                    ApprovalStatus = @ApprovalStatus,
                    SiteId = @SiteId,
                    OrgId = @OrgId,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE ProcessId = @ProcessId";

            await ExecuteAsync(sql, new
            {
                process.ProcessId,
                process.LeaseId,
                process.ProcessType,
                process.ProcessDate,
                process.ProcessStatus,
                process.ProcessResult,
                process.ProcessUserId,
                process.ProcessUserName,
                process.ProcessMemo,
                process.ApprovalUserId,
                process.ApprovalDate,
                process.ApprovalStatus,
                process.SiteId,
                process.OrgId,
                process.UpdatedBy,
                process.UpdatedAt
            });

            return process;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改租賃處理失敗: {process.ProcessId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string processId)
    {
        try
        {
            const string sql = @"
                DELETE FROM LeaseProcesses 
                WHERE ProcessId = @ProcessId";

            await ExecuteAsync(sql, new { ProcessId = processId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除租賃處理失敗: {processId}", ex);
            throw;
        }
    }

    public async Task UpdateStatusAsync(string processId, string processStatus)
    {
        try
        {
            const string sql = @"
                UPDATE LeaseProcesses SET
                    ProcessStatus = @ProcessStatus,
                    UpdatedAt = GETDATE()
                WHERE ProcessId = @ProcessId";

            await ExecuteAsync(sql, new { ProcessId = processId, ProcessStatus = processStatus });
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新租賃處理狀態失敗: {processId}, Status: {processStatus}", ex);
            throw;
        }
    }
}

