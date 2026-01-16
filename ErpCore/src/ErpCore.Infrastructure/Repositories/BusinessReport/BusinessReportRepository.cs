using Dapper;
using ErpCore.Domain.Entities.BusinessReport;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using BusinessReportEntity = ErpCore.Domain.Entities.BusinessReport.BusinessReport;

namespace ErpCore.Infrastructure.Repositories.BusinessReport;

/// <summary>
/// 業務報表 Repository 實作 (SYSL135)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class BusinessReportRepository : BaseRepository, IBusinessReportRepository
{
    public BusinessReportRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PagedResult<BusinessReportEntity>> QueryAsync(BusinessReportQuery query)
    {
        try
        {
            var sql = @"
                SELECT 
                    br.ReportId,
                    br.SiteId,
                    br.SiteName,
                    br.CardType,
                    br.CardTypeName,
                    br.VendorId,
                    br.VendorName,
                    br.StoreId,
                    br.StoreName,
                    br.AgreementId,
                    br.OrgId,
                    br.OrgName,
                    br.ActionType,
                    br.ActionTypeName,
                    br.ReportDate,
                    br.Status,
                    br.Notes,
                    br.CreatedBy,
                    br.CreatedAt,
                    br.UpdatedBy,
                    br.UpdatedAt
                FROM BusinessReports br
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.SiteId))
            {
                sql += " AND br.SiteId = @SiteId";
                parameters.Add("SiteId", query.SiteId);
            }

            if (!string.IsNullOrEmpty(query.CardType))
            {
                sql += " AND br.CardType = @CardType";
                parameters.Add("CardType", query.CardType);
            }

            if (!string.IsNullOrEmpty(query.VendorId))
            {
                sql += " AND br.VendorId = @VendorId";
                parameters.Add("VendorId", query.VendorId);
            }

            if (!string.IsNullOrEmpty(query.StoreId))
            {
                sql += " AND br.StoreId = @StoreId";
                parameters.Add("StoreId", query.StoreId);
            }

            if (!string.IsNullOrEmpty(query.OrgId))
            {
                sql += " AND br.OrgId = @OrgId";
                parameters.Add("OrgId", query.OrgId);
            }

            if (query.StartDate.HasValue)
            {
                sql += " AND br.ReportDate >= @StartDate";
                parameters.Add("StartDate", query.StartDate.Value);
            }

            if (query.EndDate.HasValue)
            {
                sql += " AND br.ReportDate <= @EndDate";
                parameters.Add("EndDate", query.EndDate.Value);
            }

            // 計算總筆數
            var countSql = "SELECT COUNT(*) FROM BusinessReports br WHERE 1=1";
            if (!string.IsNullOrEmpty(query.SiteId))
            {
                countSql += " AND br.SiteId = @SiteId";
            }
            if (!string.IsNullOrEmpty(query.CardType))
            {
                countSql += " AND br.CardType = @CardType";
            }
            if (!string.IsNullOrEmpty(query.VendorId))
            {
                countSql += " AND br.VendorId = @VendorId";
            }
            if (!string.IsNullOrEmpty(query.StoreId))
            {
                countSql += " AND br.StoreId = @StoreId";
            }
            if (!string.IsNullOrEmpty(query.OrgId))
            {
                countSql += " AND br.OrgId = @OrgId";
            }
            if (query.StartDate.HasValue)
            {
                countSql += " AND br.ReportDate >= @StartDate";
            }
            if (query.EndDate.HasValue)
            {
                countSql += " AND br.ReportDate <= @EndDate";
            }
            var totalCount = await ExecuteScalarAsync<int>(countSql, parameters);

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "ReportId" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY br.{sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<BusinessReportEntity>(sql, parameters);

            return new PagedResult<BusinessReportEntity>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢業務報表列表失敗", ex);
            throw;
        }
    }
}

