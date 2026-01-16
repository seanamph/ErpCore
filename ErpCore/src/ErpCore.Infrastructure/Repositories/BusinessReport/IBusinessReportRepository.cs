using ErpCore.Domain.Entities.BusinessReport;
using ErpCore.Shared.Common;
using BusinessReportEntity = ErpCore.Domain.Entities.BusinessReport.BusinessReport;

namespace ErpCore.Infrastructure.Repositories.BusinessReport;

/// <summary>
/// 業務報表 Repository 介面 (SYSL135)
/// </summary>
public interface IBusinessReportRepository
{
    /// <summary>
    /// 查詢業務報表列表
    /// </summary>
    Task<PagedResult<BusinessReportEntity>> QueryAsync(BusinessReportQuery query);
}

/// <summary>
/// 業務報表查詢條件
/// </summary>
public class BusinessReportQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? SiteId { get; set; }
    public string? CardType { get; set; }
    public string? VendorId { get; set; }
    public string? StoreId { get; set; }
    public string? OrgId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

