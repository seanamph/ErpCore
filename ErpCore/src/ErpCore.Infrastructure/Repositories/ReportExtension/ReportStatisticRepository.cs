using Dapper;
using ErpCore.Domain.Entities.ReportExtension;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.ReportExtension;

/// <summary>
/// 報表統計記錄儲存庫實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class ReportStatisticRepository : BaseRepository, IReportStatisticRepository
{
    public ReportStatisticRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<ReportStatistic?> GetByIdAsync(long statisticId)
    {
        try
        {
            var sql = @"
                SELECT * FROM [dbo].[ReportStatistics]
                WHERE [StatisticId] = @StatisticId";

            return await QueryFirstOrDefaultAsync<ReportStatistic>(sql, new { StatisticId = statisticId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢報表統計記錄失敗: {statisticId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<ReportStatistic>> QueryAsync(ReportStatisticQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM [dbo].[ReportStatistics]
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ReportCode))
            {
                sql += " AND [ReportCode] = @ReportCode";
                parameters.Add("ReportCode", query.ReportCode);
            }

            if (!string.IsNullOrEmpty(query.StatisticType))
            {
                sql += " AND [StatisticType] = @StatisticType";
                parameters.Add("StatisticType", query.StatisticType);
            }

            if (query.StartDate.HasValue)
            {
                sql += " AND [StatisticDate] >= @StartDate";
                parameters.Add("StartDate", query.StartDate);
            }

            if (query.EndDate.HasValue)
            {
                sql += " AND [StatisticDate] <= @EndDate";
                parameters.Add("EndDate", query.EndDate);
            }

            sql += " ORDER BY [StatisticDate] DESC";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<ReportStatistic>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM [dbo].[ReportStatistics]
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.ReportCode))
            {
                countSql += " AND [ReportCode] = @ReportCode";
                countParameters.Add("ReportCode", query.ReportCode);
            }
            if (!string.IsNullOrEmpty(query.StatisticType))
            {
                countSql += " AND [StatisticType] = @StatisticType";
                countParameters.Add("StatisticType", query.StatisticType);
            }
            if (query.StartDate.HasValue)
            {
                countSql += " AND [StatisticDate] >= @StartDate";
                countParameters.Add("StartDate", query.StartDate);
            }
            if (query.EndDate.HasValue)
            {
                countSql += " AND [StatisticDate] <= @EndDate";
                countParameters.Add("EndDate", query.EndDate);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<ReportStatistic>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢報表統計記錄列表失敗", ex);
            throw;
        }
    }

    public async Task<ReportStatistic> CreateAsync(ReportStatistic entity)
    {
        try
        {
            var sql = @"
                INSERT INTO [dbo].[ReportStatistics] 
                ([ReportCode], [ReportName], [StatisticType], [StatisticDate], [StatisticValue], [StatisticData], [CreatedBy], [CreatedAt])
                VALUES 
                (@ReportCode, @ReportName, @StatisticType, @StatisticDate, @StatisticValue, @StatisticData, @CreatedBy, @CreatedAt);
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var id = await ExecuteScalarAsync<long>(sql, entity);
            entity.StatisticId = id;
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError("建立報表統計記錄失敗", ex);
            throw;
        }
    }
}

