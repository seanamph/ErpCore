using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Lease;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Lease;

/// <summary>
/// 租賃報表查詢記錄 Repository 實作 (SYSM141-SYSM144)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class LeaseReportQueryRepository : BaseRepository, ILeaseReportQueryRepository
{
    public LeaseReportQueryRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<LeaseReportQuery?> GetByIdAsync(string queryId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM LeaseReportQueries 
                WHERE QueryId = @QueryId";

            return await QueryFirstOrDefaultAsync<LeaseReportQuery>(sql, new { QueryId = queryId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢租賃報表查詢記錄失敗: {queryId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<LeaseReportQuery>> QueryAsync(LeaseReportQueryQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM LeaseReportQueries 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.QueryId))
            {
                sql += " AND QueryId LIKE @QueryId";
                parameters.Add("QueryId", $"%{query.QueryId}%");
            }

            if (!string.IsNullOrEmpty(query.ReportType))
            {
                sql += " AND ReportType = @ReportType";
                parameters.Add("ReportType", query.ReportType);
            }

            if (!string.IsNullOrEmpty(query.QueryName))
            {
                sql += " AND QueryName LIKE @QueryName";
                parameters.Add("QueryName", $"%{query.QueryName}%");
            }

            if (query.QueryDateFrom.HasValue)
            {
                sql += " AND QueryDate >= @QueryDateFrom";
                parameters.Add("QueryDateFrom", query.QueryDateFrom);
            }

            if (query.QueryDateTo.HasValue)
            {
                sql += " AND QueryDate <= @QueryDateTo";
                parameters.Add("QueryDateTo", query.QueryDateTo);
            }

            sql += " ORDER BY QueryDate DESC, QueryId";
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<LeaseReportQuery>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢租賃報表查詢記錄列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(LeaseReportQueryQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM LeaseReportQueries 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.QueryId))
            {
                sql += " AND QueryId LIKE @QueryId";
                parameters.Add("QueryId", $"%{query.QueryId}%");
            }

            if (!string.IsNullOrEmpty(query.ReportType))
            {
                sql += " AND ReportType = @ReportType";
                parameters.Add("ReportType", query.ReportType);
            }

            if (!string.IsNullOrEmpty(query.QueryName))
            {
                sql += " AND QueryName LIKE @QueryName";
                parameters.Add("QueryName", $"%{query.QueryName}%");
            }

            if (query.QueryDateFrom.HasValue)
            {
                sql += " AND QueryDate >= @QueryDateFrom";
                parameters.Add("QueryDateFrom", query.QueryDateFrom);
            }

            if (query.QueryDateTo.HasValue)
            {
                sql += " AND QueryDate <= @QueryDateTo";
                parameters.Add("QueryDateTo", query.QueryDateTo);
            }

            return await ExecuteScalarAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢租賃報表查詢記錄數量失敗", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string queryId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM LeaseReportQueries 
                WHERE QueryId = @QueryId";

            var count = await ExecuteScalarAsync<int>(sql, new { QueryId = queryId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查租賃報表查詢記錄是否存在失敗: {queryId}", ex);
            throw;
        }
    }

    public async Task<LeaseReportQuery> CreateAsync(LeaseReportQuery reportQuery)
    {
        try
        {
            const string sql = @"
                INSERT INTO LeaseReportQueries (
                    QueryId, ReportType, QueryName, QueryParams, QueryResult, QueryDate, CreatedBy, CreatedAt
                ) VALUES (
                    @QueryId, @ReportType, @QueryName, @QueryParams, @QueryResult, @QueryDate, @CreatedBy, @CreatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, new
            {
                reportQuery.QueryId,
                reportQuery.ReportType,
                reportQuery.QueryName,
                reportQuery.QueryParams,
                reportQuery.QueryResult,
                reportQuery.QueryDate,
                reportQuery.CreatedBy,
                reportQuery.CreatedAt
            });

            reportQuery.TKey = tKey;
            return reportQuery;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增租賃報表查詢記錄失敗: {reportQuery.QueryId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string queryId)
    {
        try
        {
            const string sql = @"
                DELETE FROM LeaseReportQueries 
                WHERE QueryId = @QueryId";

            await ExecuteAsync(sql, new { QueryId = queryId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除租賃報表查詢記錄失敗: {queryId}", ex);
            throw;
        }
    }
}

