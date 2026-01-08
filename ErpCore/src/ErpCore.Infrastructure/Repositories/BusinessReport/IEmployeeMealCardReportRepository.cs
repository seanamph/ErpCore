using ErpCore.Domain.Entities.BusinessReport;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.BusinessReport;

/// <summary>
/// 員餐卡報表 Repository 介面 (SYSL210)
/// </summary>
public interface IEmployeeMealCardReportRepository
{
    /// <summary>
    /// 查詢員餐卡報表資料
    /// </summary>
    Task<PagedResult<EmployeeMealCardTransaction>> QueryReportAsync(EmployeeMealCardReportQuery query);
}

/// <summary>
/// 員餐卡報表查詢條件
/// </summary>
public class EmployeeMealCardReportQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? ReportType { get; set; }
    public string? SiteId { get; set; }
    public string? OrgId { get; set; }
    public string? YearMonth { get; set; }
    public string? ActionType { get; set; }
    public string? TxnNo { get; set; }
    public string? CardSurfaceId { get; set; }
}

