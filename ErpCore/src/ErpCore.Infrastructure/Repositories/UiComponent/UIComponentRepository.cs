using Dapper;
using ErpCore.Domain.Entities.UiComponent;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.UiComponent;

/// <summary>
/// UI組件儲存庫實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class UIComponentRepository : BaseRepository, IUIComponentRepository
{
    public UIComponentRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<UIComponent?> GetByIdAsync(long componentId)
    {
        try
        {
            var sql = @"
                SELECT * FROM [dbo].[UIComponents]
                WHERE [ComponentId] = @ComponentId";

            return await QueryFirstOrDefaultAsync<UIComponent>(sql, new { ComponentId = componentId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢UI組件失敗: {componentId}", ex);
            throw;
        }
    }

    public async Task<UIComponent?> GetByCodeAndVersionAsync(string componentCode, string componentVersion)
    {
        try
        {
            var sql = @"
                SELECT * FROM [dbo].[UIComponents]
                WHERE [ComponentCode] = @ComponentCode AND [ComponentVersion] = @ComponentVersion";

            return await QueryFirstOrDefaultAsync<UIComponent>(sql, new { ComponentCode = componentCode, ComponentVersion = componentVersion });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢UI組件失敗: {componentCode}, {componentVersion}", ex);
            throw;
        }
    }

    public async Task<PagedResult<UIComponent>> QueryAsync(UIComponentQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM [dbo].[UIComponents]
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ComponentCode))
            {
                sql += " AND [ComponentCode] LIKE @ComponentCode";
                parameters.Add("ComponentCode", $"%{query.ComponentCode}%");
            }

            if (!string.IsNullOrEmpty(query.ComponentType))
            {
                sql += " AND [ComponentType] = @ComponentType";
                parameters.Add("ComponentType", query.ComponentType);
            }

            if (!string.IsNullOrEmpty(query.ComponentVersion))
            {
                sql += " AND [ComponentVersion] = @ComponentVersion";
                parameters.Add("ComponentVersion", query.ComponentVersion);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND [Status] = @Status";
                parameters.Add("Status", query.Status);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "CreatedAt" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY [{sortField}] {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<UIComponent>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM [dbo].[UIComponents]
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.ComponentCode))
            {
                countSql += " AND [ComponentCode] LIKE @ComponentCode";
                countParameters.Add("ComponentCode", $"%{query.ComponentCode}%");
            }
            if (!string.IsNullOrEmpty(query.ComponentType))
            {
                countSql += " AND [ComponentType] = @ComponentType";
                countParameters.Add("ComponentType", query.ComponentType);
            }
            if (!string.IsNullOrEmpty(query.ComponentVersion))
            {
                countSql += " AND [ComponentVersion] = @ComponentVersion";
                countParameters.Add("ComponentVersion", query.ComponentVersion);
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND [Status] = @Status";
                countParameters.Add("Status", query.Status);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<UIComponent>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢UI組件列表失敗", ex);
            throw;
        }
    }

    public async Task<UIComponent> CreateAsync(UIComponent entity)
    {
        try
        {
            var sql = @"
                INSERT INTO [dbo].[UIComponents] 
                ([ComponentCode], [ComponentName], [ComponentType], [ComponentVersion], [ConfigJson], [Status], [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt])
                VALUES 
                (@ComponentCode, @ComponentName, @ComponentType, @ComponentVersion, @ConfigJson, @Status, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var id = await ExecuteScalarAsync<long>(sql, entity);
            entity.ComponentId = id;
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError("建立UI組件失敗", ex);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(UIComponent entity)
    {
        try
        {
            var sql = @"
                UPDATE [dbo].[UIComponents]
                SET [ComponentName] = @ComponentName,
                    [ConfigJson] = @ConfigJson,
                    [Status] = @Status,
                    [UpdatedBy] = @UpdatedBy,
                    [UpdatedAt] = @UpdatedAt
                WHERE [ComponentId] = @ComponentId";

            var rowsAffected = await ExecuteAsync(sql, entity);
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新UI組件失敗: {entity.ComponentId}", ex);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(long componentId)
    {
        try
        {
            var sql = @"
                DELETE FROM [dbo].[UIComponents]
                WHERE [ComponentId] = @ComponentId";

            var rowsAffected = await ExecuteAsync(sql, new { ComponentId = componentId });
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除UI組件失敗: {componentId}", ex);
            throw;
        }
    }

    public async Task<List<UIComponentUsage>> GetUsagesAsync(long componentId)
    {
        try
        {
            var sql = @"
                SELECT * FROM [dbo].[UIComponentUsages]
                WHERE [ComponentId] = @ComponentId
                ORDER BY [LastUsedAt] DESC";

            var result = await QueryAsync<UIComponentUsage>(sql, new { ComponentId = componentId });
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢UI組件使用記錄失敗: {componentId}", ex);
            throw;
        }
    }

    public async Task<UIComponentUsage> CreateUsageAsync(UIComponentUsage usage)
    {
        try
        {
            var sql = @"
                INSERT INTO [dbo].[UIComponentUsages] 
                ([ComponentId], [ModuleCode], [ModuleName], [UsageCount], [LastUsedAt], [CreatedAt])
                VALUES 
                (@ComponentId, @ModuleCode, @ModuleName, @UsageCount, @LastUsedAt, @CreatedAt);
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var id = await ExecuteScalarAsync<long>(sql, usage);
            usage.UsageId = id;
            return usage;
        }
        catch (Exception ex)
        {
            _logger.LogError("建立UI組件使用記錄失敗", ex);
            throw;
        }
    }

    public async Task<bool> UpdateUsageAsync(UIComponentUsage usage)
    {
        try
        {
            var sql = @"
                UPDATE [dbo].[UIComponentUsages]
                SET [UsageCount] = @UsageCount,
                    [LastUsedAt] = @LastUsedAt
                WHERE [UsageId] = @UsageId";

            var rowsAffected = await ExecuteAsync(sql, usage);
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新UI組件使用記錄失敗: {usage.UsageId}", ex);
            throw;
        }
    }
}

