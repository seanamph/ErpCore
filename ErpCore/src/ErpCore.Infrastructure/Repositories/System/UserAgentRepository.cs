using Dapper;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 使用者權限代理 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class UserAgentRepository : BaseRepository, IUserAgentRepository
{
    public UserAgentRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<UserAgent?> GetByIdAsync(Guid agentId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM UserAgent 
                WHERE AgentId = @AgentId";

            return await QueryFirstOrDefaultAsync<UserAgent>(sql, new { AgentId = agentId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢使用者權限代理失敗: {agentId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<UserAgent>> QueryAsync(UserAgentQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM UserAgent
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.PrincipalUserId))
            {
                sql += " AND PrincipalUserId LIKE @PrincipalUserId";
                parameters.Add("PrincipalUserId", $"%{query.PrincipalUserId}%");
            }

            if (!string.IsNullOrEmpty(query.AgentUserId))
            {
                sql += " AND AgentUserId LIKE @AgentUserId";
                parameters.Add("AgentUserId", $"%{query.AgentUserId}%");
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (query.BeginTimeFrom.HasValue)
            {
                sql += " AND BeginTime >= @BeginTimeFrom";
                parameters.Add("BeginTimeFrom", query.BeginTimeFrom.Value);
            }

            if (query.BeginTimeTo.HasValue)
            {
                sql += " AND BeginTime <= @BeginTimeTo";
                parameters.Add("BeginTimeTo", query.BeginTimeTo.Value);
            }

            if (query.EndTimeFrom.HasValue)
            {
                sql += " AND EndTime >= @EndTimeFrom";
                parameters.Add("EndTimeFrom", query.EndTimeFrom.Value);
            }

            if (query.EndTimeTo.HasValue)
            {
                sql += " AND EndTime <= @EndTimeTo";
                parameters.Add("EndTimeTo", query.EndTimeTo.Value);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "CreatedAt" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) ? "DESC" : query.SortOrder;
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            // 查詢總數
            var countSql = sql.Replace("SELECT *", "SELECT COUNT(*)").Split("ORDER BY")[0].Split("OFFSET")[0];
            var totalCount = await QuerySingleAsync<int>(countSql, parameters);

            // 查詢資料
            var items = (await QueryAsync<UserAgent>(sql, parameters)).ToList();

            return new PagedResult<UserAgent>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢使用者權限代理列表失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<UserAgent>> GetByPrincipalUserIdAsync(string principalUserId, int pageIndex = 1, int pageSize = 20)
    {
        try
        {
            var query = new UserAgentQuery
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                PrincipalUserId = principalUserId
            };
            return await QueryAsync(query);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢委託人代理記錄失敗: {principalUserId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<UserAgent>> GetByAgentUserIdAsync(string agentUserId, int pageIndex = 1, int pageSize = 20)
    {
        try
        {
            var query = new UserAgentQuery
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                AgentUserId = agentUserId
            };
            return await QueryAsync(query);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢代理人代理記錄失敗: {agentUserId}", ex);
            throw;
        }
    }

    public async Task<List<UserAgent>> GetActiveAgentsAsync(string? principalUserId = null, string? agentUserId = null, DateTime? currentTime = null)
    {
        try
        {
            var sql = @"
                SELECT * FROM UserAgent
                WHERE Status = 'A'
                AND BeginTime <= @CurrentTime
                AND EndTime >= @CurrentTime";

            var parameters = new DynamicParameters();
            parameters.Add("CurrentTime", currentTime ?? DateTime.Now);

            if (!string.IsNullOrEmpty(principalUserId))
            {
                sql += " AND PrincipalUserId = @PrincipalUserId";
                parameters.Add("PrincipalUserId", principalUserId);
            }

            if (!string.IsNullOrEmpty(agentUserId))
            {
                sql += " AND AgentUserId = @AgentUserId";
                parameters.Add("AgentUserId", agentUserId);
            }

            return (await QueryAsync<UserAgent>(sql, parameters)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢有效代理記錄失敗", ex);
            throw;
        }
    }

    public async Task<UserAgent> CreateAsync(UserAgent userAgent)
    {
        try
        {
            const string sql = @"
                INSERT INTO UserAgent (
                    AgentId, PrincipalUserId, AgentUserId, BeginTime, EndTime, 
                    Status, Notes, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                ) VALUES (
                    @AgentId, @PrincipalUserId, @AgentUserId, @BeginTime, @EndTime,
                    @Status, @Notes, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                )";

            await ExecuteAsync(sql, new
            {
                userAgent.AgentId,
                userAgent.PrincipalUserId,
                userAgent.AgentUserId,
                userAgent.BeginTime,
                userAgent.EndTime,
                userAgent.Status,
                userAgent.Notes,
                userAgent.CreatedBy,
                userAgent.CreatedAt,
                userAgent.UpdatedBy,
                userAgent.UpdatedAt
            });

            return userAgent;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增使用者權限代理失敗", ex);
            throw;
        }
    }

    public async Task<UserAgent> UpdateAsync(UserAgent userAgent)
    {
        try
        {
            const string sql = @"
                UPDATE UserAgent SET
                    PrincipalUserId = @PrincipalUserId,
                    AgentUserId = @AgentUserId,
                    BeginTime = @BeginTime,
                    EndTime = @EndTime,
                    Status = @Status,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE AgentId = @AgentId";

            await ExecuteAsync(sql, new
            {
                userAgent.AgentId,
                userAgent.PrincipalUserId,
                userAgent.AgentUserId,
                userAgent.BeginTime,
                userAgent.EndTime,
                userAgent.Status,
                userAgent.Notes,
                userAgent.UpdatedBy,
                userAgent.UpdatedAt
            });

            return userAgent;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改使用者權限代理失敗: {userAgent.AgentId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(Guid agentId)
    {
        try
        {
            const string sql = @"DELETE FROM UserAgent WHERE AgentId = @AgentId";

            await ExecuteAsync(sql, new { AgentId = agentId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除使用者權限代理失敗: {agentId}", ex);
            throw;
        }
    }

    public async Task DeleteBatchAsync(List<Guid> agentIds)
    {
        try
        {
            if (agentIds == null || agentIds.Count == 0)
                return;

            const string sql = @"DELETE FROM UserAgent WHERE AgentId IN @AgentIds";

            await ExecuteAsync(sql, new { AgentIds = agentIds });
        }
        catch (Exception ex)
        {
            _logger.LogError("批次刪除使用者權限代理失敗", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(Guid agentId)
    {
        try
        {
            const string sql = @"SELECT COUNT(1) FROM UserAgent WHERE AgentId = @AgentId";

            var count = await QuerySingleAsync<int>(sql, new { AgentId = agentId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查使用者權限代理是否存在失敗: {agentId}", ex);
            throw;
        }
    }
}

