using Dapper;
using ErpCore.Domain.Entities.SystemConfig;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.SystemConfig;

/// <summary>
/// 主系統項目 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class ConfigSystemRepository : BaseRepository, IConfigSystemRepository
{
    public ConfigSystemRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<ConfigSystem?> GetByIdAsync(string systemId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM ConfigSystems 
                WHERE SystemId = @SystemId";

            return await QueryFirstOrDefaultAsync<ConfigSystem>(sql, new { SystemId = systemId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢主系統失敗: {systemId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<ConfigSystem>> QueryAsync(ConfigSystemQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM ConfigSystems
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.SystemId))
            {
                sql += " AND SystemId LIKE @SystemId";
                parameters.Add("SystemId", $"%{query.SystemId}%");
            }

            if (!string.IsNullOrEmpty(query.SystemName))
            {
                sql += " AND SystemName LIKE @SystemName";
                parameters.Add("SystemName", $"%{query.SystemName}%");
            }

            if (!string.IsNullOrEmpty(query.SystemType))
            {
                sql += " AND SystemType = @SystemType";
                parameters.Add("SystemType", query.SystemType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "SeqNo, SystemId" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<ConfigSystem>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM ConfigSystems
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.SystemId))
            {
                countSql += " AND SystemId LIKE @SystemId";
                countParameters.Add("SystemId", $"%{query.SystemId}%");
            }
            if (!string.IsNullOrEmpty(query.SystemName))
            {
                countSql += " AND SystemName LIKE @SystemName";
                countParameters.Add("SystemName", $"%{query.SystemName}%");
            }
            if (!string.IsNullOrEmpty(query.SystemType))
            {
                countSql += " AND SystemType = @SystemType";
                countParameters.Add("SystemType", query.SystemType);
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND Status = @Status";
                countParameters.Add("Status", query.Status);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<ConfigSystem>
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

    public async Task<ConfigSystem> CreateAsync(ConfigSystem configSystem)
    {
        try
        {
            const string sql = @"
                INSERT INTO ConfigSystems (
                    SystemId, SystemName, SeqNo, SystemType, ServerIp, ModuleId, DbUser, DbPass, Notes, Status,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup
                )
                OUTPUT INSERTED.*
                VALUES (
                    @SystemId, @SystemName, @SeqNo, @SystemType, @ServerIp, @ModuleId, @DbUser, @DbPass, @Notes, @Status,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup
                )";

            var result = await QueryFirstOrDefaultAsync<ConfigSystem>(sql, configSystem);
            if (result == null)
            {
                throw new InvalidOperationException("新增主系統失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增主系統失敗: {configSystem.SystemId}", ex);
            throw;
        }
    }

    public async Task<ConfigSystem> UpdateAsync(ConfigSystem configSystem)
    {
        try
        {
            const string sql = @"
                UPDATE ConfigSystems SET
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

            var result = await QueryFirstOrDefaultAsync<ConfigSystem>(sql, configSystem);
            if (result == null)
            {
                throw new InvalidOperationException($"主系統不存在: {configSystem.SystemId}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改主系統失敗: {configSystem.SystemId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string systemId)
    {
        try
        {
            const string sql = @"
                DELETE FROM ConfigSystems 
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
                SELECT COUNT(*) FROM ConfigSystems 
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
}

