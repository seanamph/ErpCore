using Dapper;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 主系統項目 Repository 實作 (SYS0410)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class SystemRepository : BaseRepository, ISystemRepository
{
    public SystemRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Systems?> GetByIdAsync(string systemId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Systems 
                WHERE SystemId = @SystemId";

            return await QueryFirstOrDefaultAsync<Systems>(sql, new { SystemId = systemId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢主系統失敗: {systemId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<Systems>> QueryAsync(SystemQuery query)
    {
        try
        {
            var sql = @"
                SELECT s.*
                FROM Systems s
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.SystemId))
            {
                sql += " AND s.SystemId LIKE @SystemId";
                parameters.Add("SystemId", $"%{query.SystemId}%");
            }

            if (!string.IsNullOrEmpty(query.SystemName))
            {
                sql += " AND s.SystemName LIKE @SystemName";
                parameters.Add("SystemName", $"%{query.SystemName}%");
            }

            if (!string.IsNullOrEmpty(query.SystemType))
            {
                if (query.SystemType.Contains(','))
                {
                    sql += " AND s.SystemType IN (SELECT value FROM STRING_SPLIT(@SystemType, ','))";
                    parameters.Add("SystemType", query.SystemType);
                }
                else
                {
                    sql += " AND s.SystemType = @SystemType";
                    parameters.Add("SystemType", query.SystemType);
                }
            }

            if (!string.IsNullOrEmpty(query.ServerIp))
            {
                sql += " AND s.ServerIp LIKE @ServerIp";
                parameters.Add("ServerIp", $"%{query.ServerIp}%");
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND s.Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "s.SeqNo, s.SystemId" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<Systems>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM Systems s
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.SystemId))
            {
                countSql += " AND s.SystemId LIKE @SystemId";
                countParameters.Add("SystemId", $"%{query.SystemId}%");
            }
            if (!string.IsNullOrEmpty(query.SystemName))
            {
                countSql += " AND s.SystemName LIKE @SystemName";
                countParameters.Add("SystemName", $"%{query.SystemName}%");
            }
            if (!string.IsNullOrEmpty(query.SystemType))
            {
                if (query.SystemType.Contains(','))
                {
                    countSql += " AND s.SystemType IN (SELECT value FROM STRING_SPLIT(@SystemType, ','))";
                    countParameters.Add("SystemType", query.SystemType);
                }
                else
                {
                    countSql += " AND s.SystemType = @SystemType";
                    countParameters.Add("SystemType", query.SystemType);
                }
            }
            if (!string.IsNullOrEmpty(query.ServerIp))
            {
                countSql += " AND s.ServerIp LIKE @ServerIp";
                countParameters.Add("ServerIp", $"%{query.ServerIp}%");
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND s.Status = @Status";
                countParameters.Add("Status", query.Status);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<Systems>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢主系統列表失敗", ex);
            throw;
        }
    }

    public async Task<Systems> CreateAsync(Systems system)
    {
        try
        {
            const string sql = @"
                INSERT INTO Systems (
                    SystemId, SystemName, SeqNo, SystemType, ServerIp, ModuleId, DbUser, DbPass, Notes, Status,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup
                )
                OUTPUT INSERTED.*
                VALUES (
                    @SystemId, @SystemName, @SeqNo, @SystemType, @ServerIp, @ModuleId, @DbUser, @DbPass, @Notes, @Status,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup
                )";

            var result = await QueryFirstOrDefaultAsync<Systems>(sql, system);
            if (result == null)
            {
                throw new InvalidOperationException("新增主系統失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增主系統失敗: {system.SystemId}", ex);
            throw;
        }
    }

    public async Task<Systems> UpdateAsync(Systems system)
    {
        try
        {
            const string sql = @"
                UPDATE Systems SET
                    SystemName = @SystemName,
                    SeqNo = @SeqNo,
                    SystemType = @SystemType,
                    ServerIp = @ServerIp,
                    ModuleId = @ModuleId,
                    DbUser = @DbUser,
                    DbPass = @DbPass,
                    Notes = @Notes,
                    Status = @Status,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                OUTPUT INSERTED.*
                WHERE SystemId = @SystemId";

            var result = await QueryFirstOrDefaultAsync<Systems>(sql, system);
            if (result == null)
            {
                throw new InvalidOperationException($"主系統不存在: {system.SystemId}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改主系統失敗: {system.SystemId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string systemId)
    {
        try
        {
            const string sql = @"
                DELETE FROM Systems 
                WHERE SystemId = @SystemId";

            await ExecuteAsync(sql, new { SystemId = systemId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除主系統失敗: {systemId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string systemId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM Systems 
                WHERE SystemId = @SystemId";

            var count = await QuerySingleAsync<int>(sql, new { SystemId = systemId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查主系統是否存在失敗: {systemId}", ex);
            throw;
        }
    }

    public async Task<bool> HasSubSystemsAsync(string systemId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM SubSystems 
                WHERE SystemId = @SystemId";

            var count = await QuerySingleAsync<int>(sql, new { SystemId = systemId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查是否有子系統關聯失敗: {systemId}", ex);
            throw;
        }
    }

    public async Task<bool> HasProgramsAsync(string systemId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM Programs 
                WHERE SystemId = @SystemId";

            var count = await QuerySingleAsync<int>(sql, new { SystemId = systemId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查是否有作業關聯失敗: {systemId}", ex);
            throw;
        }
    }
}
