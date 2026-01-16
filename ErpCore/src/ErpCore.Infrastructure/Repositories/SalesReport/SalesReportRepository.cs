using System.Data;
using System.Linq;
using Dapper;
using ErpCore.Domain.Entities.SalesReport;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using SalesReportEntity = ErpCore.Domain.Entities.SalesReport.SalesReport;

namespace ErpCore.Infrastructure.Repositories.SalesReport;

/// <summary>
/// 銷售報表 Repository 實作 (SYS1000 - 銷售報表模組系列)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class SalesReportRepository : BaseRepository, ISalesReportRepository
{
    public SalesReportRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<SalesReportEntity?> GetByIdAsync(string reportId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM SalesReports 
                WHERE ReportId = @ReportId";

            return await QueryFirstOrDefaultAsync<SalesReportEntity>(sql, new { ReportId = reportId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢銷售報表失敗: {reportId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<SalesReportEntity>> QueryAsync(SalesReportQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM SalesReports
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ReportCode))
            {
                sql += " AND ReportCode LIKE @ReportCode";
                parameters.Add("ReportCode", $"%{query.ReportCode}%");
            }

            if (!string.IsNullOrEmpty(query.ShopId))
            {
                sql += " AND ShopId = @ShopId";
                parameters.Add("ShopId", query.ShopId);
            }

            if (query.StartDate.HasValue)
            {
                sql += " AND StartDate >= @StartDate";
                parameters.Add("StartDate", query.StartDate.Value);
            }

            if (query.EndDate.HasValue)
            {
                sql += " AND EndDate <= @EndDate";
                parameters.Add("EndDate", query.EndDate.Value);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 排序
            sql += $" ORDER BY {query.SortField ?? "CreatedAt"} {(query.SortOrder == "DESC" ? "DESC" : "ASC")}";

            // 分頁
            var countSql = $"SELECT COUNT(*) FROM ({sql}) AS CountQuery";
            var totalCount = await QuerySingleAsync<int>(countSql, parameters);

            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<SalesReportEntity>(sql, parameters);

            return new PagedResult<SalesReportEntity>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢銷售報表列表失敗", ex);
            throw;
        }
    }

    public async Task<SalesReportEntity> CreateAsync(SalesReportEntity report)
    {
        try
        {
            const string sql = @"
                INSERT INTO SalesReports 
                (ReportId, ReportCode, ReportName, ReportType, ShopId, StartDate, EndDate, ReportData, Status, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@ReportId, @ReportCode, @ReportName, @ReportType, @ShopId, @StartDate, @EndDate, @ReportData, @Status, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt)";

            await ExecuteAsync(sql, report);
            return report;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增銷售報表失敗: {report.ReportId}", ex);
            throw;
        }
    }

    public async Task<SalesReportEntity> UpdateAsync(SalesReportEntity report)
    {
        try
        {
            const string sql = @"
                UPDATE SalesReports 
                SET ReportCode = @ReportCode, 
                    ReportName = @ReportName, 
                    ReportType = @ReportType, 
                    ShopId = @ShopId, 
                    StartDate = @StartDate, 
                    EndDate = @EndDate, 
                    ReportData = @ReportData, 
                    Status = @Status, 
                    UpdatedBy = @UpdatedBy, 
                    UpdatedAt = @UpdatedAt
                WHERE ReportId = @ReportId";

            await ExecuteAsync(sql, report);
            return report;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改銷售報表失敗: {report.ReportId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string reportId)
    {
        try
        {
            const string sql = @"
                DELETE FROM SalesReports 
                WHERE ReportId = @ReportId";

            await ExecuteAsync(sql, new { ReportId = reportId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除銷售報表失敗: {reportId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string reportId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM SalesReports 
                WHERE ReportId = @ReportId";

            var count = await QuerySingleAsync<int>(sql, new { ReportId = reportId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查銷售報表是否存在失敗: {reportId}", ex);
            throw;
        }
    }
}

