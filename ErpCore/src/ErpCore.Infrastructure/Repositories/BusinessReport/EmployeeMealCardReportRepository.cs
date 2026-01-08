using Dapper;
using ErpCore.Domain.Entities.BusinessReport;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.BusinessReport;

/// <summary>
/// 員餐卡報表 Repository 實作 (SYSL210)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class EmployeeMealCardReportRepository : BaseRepository, IEmployeeMealCardReportRepository
{
    public EmployeeMealCardReportRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PagedResult<EmployeeMealCardTransaction>> QueryReportAsync(EmployeeMealCardReportQuery query)
    {
        try
        {
            var sql = @"
                SELECT 
                    emct.TKey,
                    emct.TxnNo,
                    emct.SiteId,
                    s.SiteName,
                    emct.CardSurfaceId,
                    emct.OrgId,
                    o.OrgName,
                    emct.ActionType,
                    emct.ActionTypeName,
                    emct.YearMonth,
                    emct.Amt1,
                    emct.Amt4,
                    emct.Amt5,
                    emct.Status,
                    emct.Notes,
                    emct.CreatedBy,
                    emct.CreatedAt,
                    emct.UpdatedBy,
                    emct.UpdatedAt
                FROM EmployeeMealCardTransactions emct
                LEFT JOIN Sites s ON emct.SiteId = s.SiteId
                LEFT JOIN Organizations o ON emct.OrgId = o.OrgId
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.SiteId))
            {
                sql += " AND emct.SiteId = @SiteId";
                parameters.Add("SiteId", query.SiteId);
            }

            if (!string.IsNullOrEmpty(query.OrgId))
            {
                sql += " AND emct.OrgId = @OrgId";
                parameters.Add("OrgId", query.OrgId);
            }

            if (!string.IsNullOrEmpty(query.YearMonth))
            {
                sql += " AND emct.YearMonth = @YearMonth";
                parameters.Add("YearMonth", query.YearMonth);
            }

            if (!string.IsNullOrEmpty(query.ActionType))
            {
                sql += " AND emct.ActionType = @ActionType";
                parameters.Add("ActionType", query.ActionType);
            }

            if (!string.IsNullOrEmpty(query.TxnNo))
            {
                sql += " AND emct.TxnNo LIKE @TxnNo";
                parameters.Add("TxnNo", $"%{query.TxnNo}%");
            }

            if (!string.IsNullOrEmpty(query.CardSurfaceId))
            {
                sql += " AND emct.CardSurfaceId LIKE @CardSurfaceId";
                parameters.Add("CardSurfaceId", $"%{query.CardSurfaceId}%");
            }

            // 計算總筆數
            var countSql = "SELECT COUNT(*) FROM EmployeeMealCardTransactions emct WHERE 1=1";
            if (!string.IsNullOrEmpty(query.SiteId))
            {
                countSql += " AND emct.SiteId = @SiteId";
            }
            if (!string.IsNullOrEmpty(query.OrgId))
            {
                countSql += " AND emct.OrgId = @OrgId";
            }
            if (!string.IsNullOrEmpty(query.YearMonth))
            {
                countSql += " AND emct.YearMonth = @YearMonth";
            }
            if (!string.IsNullOrEmpty(query.ActionType))
            {
                countSql += " AND emct.ActionType = @ActionType";
            }
            if (!string.IsNullOrEmpty(query.TxnNo))
            {
                countSql += " AND emct.TxnNo LIKE @TxnNo";
            }
            if (!string.IsNullOrEmpty(query.CardSurfaceId))
            {
                countSql += " AND emct.CardSurfaceId LIKE @CardSurfaceId";
            }

            var totalCount = await QuerySingleAsync<int>(countSql, parameters);

            // 排序
            var sortField = query.SortField ?? "TxnNo";
            var sortOrder = query.SortOrder ?? "DESC";
            sql += $" ORDER BY emct.{sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = (await QueryAsync<EmployeeMealCardTransaction>(sql, parameters)).ToList();

            return new PagedResult<EmployeeMealCardTransaction>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢員餐卡報表失敗", ex);
            throw;
        }
    }
}

