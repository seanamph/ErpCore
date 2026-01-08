using Dapper;
using ErpCore.Domain.Entities.ReportExtension;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.ReportExtension;

/// <summary>
/// 報表列印記錄儲存庫實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class ReportPrintLogRepository : BaseRepository, IReportPrintLogRepository
{
    public ReportPrintLogRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<ReportPrintLog?> GetByIdAsync(long printLogId)
    {
        try
        {
            var sql = @"
                SELECT * FROM [dbo].[ReportPrintLogs]
                WHERE [PrintLogId] = @PrintLogId";

            return await QueryFirstOrDefaultAsync<ReportPrintLog>(sql, new { PrintLogId = printLogId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢報表列印記錄失敗: {printLogId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<ReportPrintLog>> QueryAsync(ReportPrintLogQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM [dbo].[ReportPrintLogs]
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ReportCode))
            {
                sql += " AND [ReportCode] = @ReportCode";
                parameters.Add("ReportCode", query.ReportCode);
            }

            if (!string.IsNullOrEmpty(query.PrintStatus))
            {
                sql += " AND [PrintStatus] = @PrintStatus";
                parameters.Add("PrintStatus", query.PrintStatus);
            }

            if (query.StartDate.HasValue)
            {
                sql += " AND [CreatedAt] >= @StartDate";
                parameters.Add("StartDate", query.StartDate);
            }

            if (query.EndDate.HasValue)
            {
                sql += " AND [CreatedAt] <= @EndDate";
                parameters.Add("EndDate", query.EndDate);
            }

            sql += " ORDER BY [CreatedAt] DESC";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<ReportPrintLog>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM [dbo].[ReportPrintLogs]
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.ReportCode))
            {
                countSql += " AND [ReportCode] = @ReportCode";
                countParameters.Add("ReportCode", query.ReportCode);
            }
            if (!string.IsNullOrEmpty(query.PrintStatus))
            {
                countSql += " AND [PrintStatus] = @PrintStatus";
                countParameters.Add("PrintStatus", query.PrintStatus);
            }
            if (query.StartDate.HasValue)
            {
                countSql += " AND [CreatedAt] >= @StartDate";
                countParameters.Add("StartDate", query.StartDate);
            }
            if (query.EndDate.HasValue)
            {
                countSql += " AND [CreatedAt] <= @EndDate";
                countParameters.Add("EndDate", query.EndDate);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<ReportPrintLog>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢報表列印記錄列表失敗", ex);
            throw;
        }
    }

    public async Task<ReportPrintLog> CreateAsync(ReportPrintLog entity)
    {
        try
        {
            var sql = @"
                INSERT INTO [dbo].[ReportPrintLogs] 
                ([ReportCode], [ReportName], [PrintType], [PrintFormat], [FilePath], [FileName], [FileSize], [PrintStatus], [PrintCount], [CreatedBy], [CreatedAt], [PrintedAt])
                VALUES 
                (@ReportCode, @ReportName, @PrintType, @PrintFormat, @FilePath, @FileName, @FileSize, @PrintStatus, @PrintCount, @CreatedBy, @CreatedAt, @PrintedAt);
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var id = await ExecuteScalarAsync<long>(sql, entity);
            entity.PrintLogId = id;
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError("建立報表列印記錄失敗", ex);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(ReportPrintLog entity)
    {
        try
        {
            var sql = @"
                UPDATE [dbo].[ReportPrintLogs]
                SET [PrintStatus] = @PrintStatus,
                    [PrintedAt] = @PrintedAt,
                    [PrintCount] = @PrintCount
                WHERE [PrintLogId] = @PrintLogId";

            var rowsAffected = await ExecuteAsync(sql, entity);
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新報表列印記錄失敗: {entity.PrintLogId}", ex);
            throw;
        }
    }
}

