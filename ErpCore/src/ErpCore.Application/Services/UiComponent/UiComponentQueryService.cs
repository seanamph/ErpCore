using Dapper;
using ErpCore.Application.DTOs.UiComponent;
using ErpCore.Application.Services.Base;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.UiComponent;

/// <summary>
/// UI組件查詢與報表服務實作
/// </summary>
public class UiComponentQueryService : BaseService, IUiComponentQueryService
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UiComponentQueryService(
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<UIComponentUsageStatsDto>> GetUsageStatsAsync(UIComponentUsageQueryDto query)
    {
        try
        {
            _logger.LogInfo("查詢UI組件使用統計");

            var sql = @"
                SELECT * FROM [dbo].[V_UIComponentUsageStats]
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

            if (!string.IsNullOrEmpty(query.ModuleCode))
            {
                sql += @" AND EXISTS (
                    SELECT 1 FROM [dbo].[UIComponentUsages] u
                    WHERE u.ComponentId = V_UIComponentUsageStats.ComponentId
                    AND u.ModuleCode = @ModuleCode
                )";
                parameters.Add("ModuleCode", query.ModuleCode);
            }

            sql += " ORDER BY [TotalUsageCount] DESC";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            using var connection = _connectionFactory.CreateConnection();
            var items = await connection.QueryAsync<UIComponentUsageStatsDto>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM [dbo].[V_UIComponentUsageStats]
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
            if (!string.IsNullOrEmpty(query.ModuleCode))
            {
                countSql += @" AND EXISTS (
                    SELECT 1 FROM [dbo].[UIComponentUsages] u
                    WHERE u.ComponentId = V_UIComponentUsageStats.ComponentId
                    AND u.ModuleCode = @ModuleCode
                )";
                countParameters.Add("ModuleCode", query.ModuleCode);
            }

            var totalCount = await connection.QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<UIComponentUsageStatsDto>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢UI組件使用統計失敗", ex);
            throw;
        }
    }

    public async Task<UIComponentUsageSummaryDto> GetUsageSummaryAsync()
    {
        try
        {
            _logger.LogInfo("取得UI組件使用統計摘要");

            var sql = @"
                SELECT 
                    COUNT(*) AS TotalComponents,
                    SUM(CASE WHEN Status = '1' THEN 1 ELSE 0 END) AS ActiveComponents,
                    ISNULL(SUM(TotalUsageCount), 0) AS TotalUsageCount
                FROM [dbo].[V_UIComponentUsageStats]";

            using var connection = _connectionFactory.CreateConnection();
            var summary = await connection.QueryFirstOrDefaultAsync<UIComponentUsageSummaryDto>(sql);

            if (summary == null)
            {
                summary = new UIComponentUsageSummaryDto();
            }

            // 查詢最常使用的組件
            var topSql = @"
                SELECT TOP 10 * FROM [dbo].[V_UIComponentUsageStats]
                ORDER BY [TotalUsageCount] DESC";

            var topComponents = await connection.QueryAsync<UIComponentUsageStatsDto>(topSql);
            summary.TopUsedComponents = topComponents.ToList();

            return summary;
        }
        catch (Exception ex)
        {
            _logger.LogError("取得UI組件使用統計摘要失敗", ex);
            throw;
        }
    }
}

