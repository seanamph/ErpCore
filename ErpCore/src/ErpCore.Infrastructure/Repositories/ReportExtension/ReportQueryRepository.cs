using Dapper;
using ErpCore.Domain.Entities.ReportExtension;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.ReportExtension;

/// <summary>
/// 報表查詢儲存庫實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class ReportQueryRepository : BaseRepository, IReportQueryRepository
{
    public ReportQueryRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<ReportQuery?> GetByIdAsync(Guid queryId)
    {
        try
        {
            var sql = @"
                SELECT * FROM [dbo].[ReportQueries]
                WHERE [QueryId] = @QueryId";

            return await QueryFirstOrDefaultAsync<ReportQuery>(sql, new { QueryId = queryId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢報表查詢設定失敗: {queryId}", ex);
            throw;
        }
    }

    public async Task<ReportQuery?> GetByReportCodeAsync(string reportCode)
    {
        try
        {
            var sql = @"
                SELECT * FROM [dbo].[ReportQueries]
                WHERE [ReportCode] = @ReportCode AND [Status] = '1'
                ORDER BY [CreatedAt] DESC";

            return await QueryFirstOrDefaultAsync<ReportQuery>(sql, new { ReportCode = reportCode });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢報表查詢設定失敗: {reportCode}", ex);
            throw;
        }
    }

    public async Task<PagedResult<ReportQuery>> QueryAsync(ReportQueryListQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM [dbo].[ReportQueries]
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ReportCode))
            {
                sql += " AND [ReportCode] = @ReportCode";
                parameters.Add("ReportCode", query.ReportCode);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND [Status] = @Status";
                parameters.Add("Status", query.Status);
            }

            sql += " ORDER BY [CreatedAt] DESC";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<ReportQuery>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM [dbo].[ReportQueries]
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.ReportCode))
            {
                countSql += " AND [ReportCode] = @ReportCode";
                countParameters.Add("ReportCode", query.ReportCode);
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND [Status] = @Status";
                countParameters.Add("Status", query.Status);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<ReportQuery>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢報表查詢設定列表失敗", ex);
            throw;
        }
    }

    public async Task<ReportQuery> CreateAsync(ReportQuery entity)
    {
        try
        {
            var sql = @"
                INSERT INTO [dbo].[ReportQueries] 
                ([QueryId], [ReportCode], [ReportName], [QueryName], [QueryParams], [QuerySql], [Status], [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt])
                VALUES 
                (@QueryId, @ReportCode, @ReportName, @QueryName, @QueryParams, @QuerySql, @Status, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt)";

            await ExecuteAsync(sql, entity);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError("建立報表查詢設定失敗", ex);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(ReportQuery entity)
    {
        try
        {
            var sql = @"
                UPDATE [dbo].[ReportQueries]
                SET [ReportName] = @ReportName,
                    [QueryName] = @QueryName,
                    [QueryParams] = @QueryParams,
                    [QuerySql] = @QuerySql,
                    [Status] = @Status,
                    [UpdatedBy] = @UpdatedBy,
                    [UpdatedAt] = @UpdatedAt
                WHERE [QueryId] = @QueryId";

            var rowsAffected = await ExecuteAsync(sql, entity);
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新報表查詢設定失敗: {entity.QueryId}", ex);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid queryId)
    {
        try
        {
            var sql = @"
                DELETE FROM [dbo].[ReportQueries]
                WHERE [QueryId] = @QueryId";

            var rowsAffected = await ExecuteAsync(sql, new { QueryId = queryId });
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除報表查詢設定失敗: {queryId}", ex);
            throw;
        }
    }

    public async Task<ReportQueryLog> CreateLogAsync(ReportQueryLog log)
    {
        try
        {
            var sql = @"
                INSERT INTO [dbo].[ReportQueryLogs] 
                ([LogId], [QueryId], [ReportCode], [UserId], [QueryParams], [QueryTime], [ExecutionTime], [RecordCount], [Status])
                VALUES 
                (@LogId, @QueryId, @ReportCode, @UserId, @QueryParams, @QueryTime, @ExecutionTime, @RecordCount, @Status)";

            await ExecuteAsync(sql, log);
            return log;
        }
        catch (Exception ex)
        {
            _logger.LogError("建立報表查詢記錄失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<ReportQueryLog>> QueryLogsAsync(ReportQueryLogQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM [dbo].[ReportQueryLogs]
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ReportCode))
            {
                sql += " AND [ReportCode] = @ReportCode";
                parameters.Add("ReportCode", query.ReportCode);
            }

            if (!string.IsNullOrEmpty(query.UserId))
            {
                sql += " AND [UserId] = @UserId";
                parameters.Add("UserId", query.UserId);
            }

            if (query.StartDate.HasValue)
            {
                sql += " AND [QueryTime] >= @StartDate";
                parameters.Add("StartDate", query.StartDate);
            }

            if (query.EndDate.HasValue)
            {
                sql += " AND [QueryTime] <= @EndDate";
                parameters.Add("EndDate", query.EndDate);
            }

            sql += " ORDER BY [QueryTime] DESC";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<ReportQueryLog>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM [dbo].[ReportQueryLogs]
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.ReportCode))
            {
                countSql += " AND [ReportCode] = @ReportCode";
                countParameters.Add("ReportCode", query.ReportCode);
            }
            if (!string.IsNullOrEmpty(query.UserId))
            {
                countSql += " AND [UserId] = @UserId";
                countParameters.Add("UserId", query.UserId);
            }
            if (query.StartDate.HasValue)
            {
                countSql += " AND [QueryTime] >= @StartDate";
                countParameters.Add("StartDate", query.StartDate);
            }
            if (query.EndDate.HasValue)
            {
                countSql += " AND [QueryTime] <= @EndDate";
                countParameters.Add("EndDate", query.EndDate);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<ReportQueryLog>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢報表查詢記錄列表失敗", ex);
            throw;
        }
    }
}

