using Dapper;
using ErpCore.Domain.Entities.SystemExtension;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.SystemExtension;

/// <summary>
/// 系統擴展報表 Repository 實作 (SYSX140)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class SystemExtensionReportRepository : BaseRepository, ISystemExtensionReportRepository
{
    public SystemExtensionReportRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<SystemExtensionReport?> GetByReportIdAsync(long reportId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM SystemExtensionReports 
                WHERE ReportId = @ReportId";

            return await QueryFirstOrDefaultAsync<SystemExtensionReport>(sql, new { ReportId = reportId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢系統擴展報表記錄失敗: {reportId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<SystemExtensionReport>> QueryAsync(SystemExtensionReportQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM SystemExtensionReports
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ReportName))
            {
                sql += " AND ReportName LIKE @ReportName";
                parameters.Add("ReportName", $"%{query.ReportName}%");
            }

            if (!string.IsNullOrEmpty(query.ReportType))
            {
                sql += " AND ReportType = @ReportType";
                parameters.Add("ReportType", query.ReportType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (query.StartDate.HasValue)
            {
                sql += " AND GeneratedDate >= @StartDate";
                parameters.Add("StartDate", query.StartDate.Value);
            }

            if (query.EndDate.HasValue)
            {
                sql += " AND GeneratedDate <= @EndDate";
                parameters.Add("EndDate", query.EndDate.Value);
            }

            if (!string.IsNullOrEmpty(query.GeneratedBy))
            {
                sql += " AND GeneratedBy = @GeneratedBy";
                parameters.Add("GeneratedBy", query.GeneratedBy);
            }

            // 排序
            sql += " ORDER BY GeneratedDate DESC";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<SystemExtensionReport>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM SystemExtensionReports
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.ReportName))
            {
                countSql += " AND ReportName LIKE @ReportName";
                countParameters.Add("ReportName", $"%{query.ReportName}%");
            }
            if (!string.IsNullOrEmpty(query.ReportType))
            {
                countSql += " AND ReportType = @ReportType";
                countParameters.Add("ReportType", query.ReportType);
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND Status = @Status";
                countParameters.Add("Status", query.Status);
            }
            if (query.StartDate.HasValue)
            {
                countSql += " AND GeneratedDate >= @StartDate";
                countParameters.Add("StartDate", query.StartDate.Value);
            }
            if (query.EndDate.HasValue)
            {
                countSql += " AND GeneratedDate <= @EndDate";
                countParameters.Add("EndDate", query.EndDate.Value);
            }
            if (!string.IsNullOrEmpty(query.GeneratedBy))
            {
                countSql += " AND GeneratedBy = @GeneratedBy";
                countParameters.Add("GeneratedBy", query.GeneratedBy);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<SystemExtensionReport>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢系統擴展報表記錄列表失敗", ex);
            throw;
        }
    }

    public async Task<SystemExtensionReport> CreateAsync(SystemExtensionReport report)
    {
        try
        {
            const string sql = @"
                INSERT INTO SystemExtensionReports (
                    ReportName, ReportType, ReportTemplate, QueryConditions,
                    GeneratedDate, GeneratedBy, FileUrl, FileSize, Status,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                )
                OUTPUT INSERTED.*
                VALUES (
                    @ReportName, @ReportType, @ReportTemplate, @QueryConditions,
                    @GeneratedDate, @GeneratedBy, @FileUrl, @FileSize, @Status,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                )";

            var result = await QueryFirstOrDefaultAsync<SystemExtensionReport>(sql, report);
            if (result == null)
            {
                throw new InvalidOperationException("新增系統擴展報表記錄失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增系統擴展報表記錄失敗: {report.ReportName}", ex);
            throw;
        }
    }

    public async Task<SystemExtensionReport> UpdateAsync(SystemExtensionReport report)
    {
        try
        {
            const string sql = @"
                UPDATE SystemExtensionReports SET
                    ReportName = @ReportName,
                    ReportType = @ReportType,
                    ReportTemplate = @ReportTemplate,
                    QueryConditions = @QueryConditions,
                    GeneratedDate = @GeneratedDate,
                    GeneratedBy = @GeneratedBy,
                    FileUrl = @FileUrl,
                    FileSize = @FileSize,
                    Status = @Status,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                OUTPUT INSERTED.*
                WHERE ReportId = @ReportId";

            var result = await QueryFirstOrDefaultAsync<SystemExtensionReport>(sql, report);
            if (result == null)
            {
                throw new InvalidOperationException($"系統擴展報表記錄不存在: {report.ReportId}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改系統擴展報表記錄失敗: {report.ReportId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long reportId)
    {
        try
        {
            const string sql = @"
                DELETE FROM SystemExtensionReports
                WHERE ReportId = @ReportId";

            var rowsAffected = await ExecuteAsync(sql, new { ReportId = reportId });
            if (rowsAffected == 0)
            {
                throw new InvalidOperationException($"系統擴展報表記錄不存在: {reportId}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除系統擴展報表記錄失敗: {reportId}", ex);
            throw;
        }
    }
}

