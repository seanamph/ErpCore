using Dapper;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.BusinessReport;

/// <summary>
/// 銷退卡報表 Repository 實作 (SYSL310)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class ReturnCardReportRepository : BaseRepository, IReturnCardReportRepository
{
    public ReturnCardReportRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PagedResult<ReturnCardReportEntity>> QueryReportAsync(ReturnCardReportQuery query)
    {
        try
        {
            var sql = @"
                SELECT 
                    rcd.TKey,
                    rc.SiteId,
                    s.SiteName,
                    rc.OrgId,
                    o.OrgName,
                    rc.CardYear,
                    rc.CardMonth,
                    rcd.EmployeeId,
                    rcd.EmployeeName,
                    rcd.ReturnDate,
                    rcd.ReturnReason,
                    rcd.Amount,
                    rcd.Status
                FROM ReturnCardDetails rcd
                INNER JOIN ReturnCards rc ON rcd.ReturnCardId = rc.TKey
                LEFT JOIN Sites s ON rc.SiteId = s.SiteId
                LEFT JOIN Organizations o ON rc.OrgId = o.OrgId
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.SiteId))
            {
                sql += " AND rc.SiteId = @SiteId";
                parameters.Add("SiteId", query.SiteId);
            }

            if (!string.IsNullOrEmpty(query.OrgId))
            {
                sql += " AND rc.OrgId = @OrgId";
                parameters.Add("OrgId", query.OrgId);
            }

            if (query.CardYear.HasValue)
            {
                sql += " AND rc.CardYear = @CardYear";
                parameters.Add("CardYear", query.CardYear.Value);
            }

            if (query.CardMonth.HasValue)
            {
                sql += " AND rc.CardMonth = @CardMonth";
                parameters.Add("CardMonth", query.CardMonth.Value);
            }

            if (query.StartDate.HasValue)
            {
                sql += " AND rcd.ReturnDate >= @StartDate";
                parameters.Add("StartDate", query.StartDate.Value);
            }

            if (query.EndDate.HasValue)
            {
                sql += " AND rcd.ReturnDate <= @EndDate";
                parameters.Add("EndDate", query.EndDate.Value);
            }

            if (!string.IsNullOrEmpty(query.EmployeeId))
            {
                sql += " AND (rcd.EmployeeId LIKE @EmployeeId OR rcd.EmployeeName LIKE @EmployeeId)";
                parameters.Add("EmployeeId", $"%{query.EmployeeId}%");
            }

            // 計算總筆數
            var countSql = "SELECT COUNT(*) FROM ReturnCardDetails rcd INNER JOIN ReturnCards rc ON rcd.ReturnCardId = rc.TKey WHERE 1=1";
            if (!string.IsNullOrEmpty(query.SiteId))
            {
                countSql += " AND rc.SiteId = @SiteId";
            }
            if (!string.IsNullOrEmpty(query.OrgId))
            {
                countSql += " AND rc.OrgId = @OrgId";
            }
            if (query.CardYear.HasValue)
            {
                countSql += " AND rc.CardYear = @CardYear";
            }
            if (query.CardMonth.HasValue)
            {
                countSql += " AND rc.CardMonth = @CardMonth";
            }
            if (query.StartDate.HasValue)
            {
                countSql += " AND rcd.ReturnDate >= @StartDate";
            }
            if (query.EndDate.HasValue)
            {
                countSql += " AND rcd.ReturnDate <= @EndDate";
            }
            if (!string.IsNullOrEmpty(query.EmployeeId))
            {
                countSql += " AND (rcd.EmployeeId LIKE @EmployeeId OR rcd.EmployeeName LIKE @EmployeeId)";
            }

            var totalCount = await QuerySingleAsync<int>(countSql, parameters);

            // 排序
            var sortField = query.SortField ?? "ReturnDate";
            var sortOrder = query.SortOrder ?? "DESC";
            sql += $" ORDER BY rcd.{sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = (await QueryAsync<ReturnCardReportEntity>(sql, parameters)).ToList();

            return new PagedResult<ReturnCardReportEntity>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢銷退卡報表失敗", ex);
            throw;
        }
    }

    public async Task<ReturnCardReportSummary> GetSummaryAsync(ReturnCardReportQuery query)
    {
        try
        {
            var sql = @"
                SELECT 
                    COUNT(*) AS TotalCount,
                    SUM(rcd.Amount) AS TotalAmount
                FROM ReturnCardDetails rcd
                INNER JOIN ReturnCards rc ON rcd.ReturnCardId = rc.TKey
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.SiteId))
            {
                sql += " AND rc.SiteId = @SiteId";
                parameters.Add("SiteId", query.SiteId);
            }

            if (!string.IsNullOrEmpty(query.OrgId))
            {
                sql += " AND rc.OrgId = @OrgId";
                parameters.Add("OrgId", query.OrgId);
            }

            if (query.CardYear.HasValue)
            {
                sql += " AND rc.CardYear = @CardYear";
                parameters.Add("CardYear", query.CardYear.Value);
            }

            if (query.CardMonth.HasValue)
            {
                sql += " AND rc.CardMonth = @CardMonth";
                parameters.Add("CardMonth", query.CardMonth.Value);
            }

            if (query.StartDate.HasValue)
            {
                sql += " AND rcd.ReturnDate >= @StartDate";
                parameters.Add("StartDate", query.StartDate.Value);
            }

            if (query.EndDate.HasValue)
            {
                sql += " AND rcd.ReturnDate <= @EndDate";
                parameters.Add("EndDate", query.EndDate.Value);
            }

            if (!string.IsNullOrEmpty(query.EmployeeId))
            {
                sql += " AND (rcd.EmployeeId LIKE @EmployeeId OR rcd.EmployeeName LIKE @EmployeeId)";
                parameters.Add("EmployeeId", $"%{query.EmployeeId}%");
            }

            var result = await QuerySingleAsync<ReturnCardReportSummary>(sql, parameters);
            return result ?? new ReturnCardReportSummary();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢銷退卡報表統計資訊失敗", ex);
            throw;
        }
    }
}

