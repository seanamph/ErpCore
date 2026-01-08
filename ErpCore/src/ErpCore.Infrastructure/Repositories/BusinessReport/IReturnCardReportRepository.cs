using ErpCore.Domain.Entities.BusinessReport;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.BusinessReport;

/// <summary>
/// 銷退卡報表 Repository 介面 (SYSL310)
/// </summary>
public interface IReturnCardReportRepository
{
    /// <summary>
    /// 查詢銷退卡報表資料
    /// </summary>
    Task<PagedResult<ReturnCardReportEntity>> QueryReportAsync(ReturnCardReportQuery query);

    /// <summary>
    /// 查詢銷退卡報表統計資訊
    /// </summary>
    Task<ReturnCardReportSummary> GetSummaryAsync(ReturnCardReportQuery query);
}

/// <summary>
/// 銷退卡報表查詢條件
/// </summary>
public class ReturnCardReportQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? SiteId { get; set; }
    public string? OrgId { get; set; }
    public int? CardYear { get; set; }
    public int? CardMonth { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? EmployeeId { get; set; }
    public string? ReportType { get; set; } // detail: 明細, summary: 統計
}

/// <summary>
/// 銷退卡報表實體
/// </summary>
public class ReturnCardReportEntity
{
    public long TKey { get; set; }
    public string SiteId { get; set; } = string.Empty;
    public string? SiteName { get; set; }
    public string? OrgId { get; set; }
    public string? OrgName { get; set; }
    public int CardYear { get; set; }
    public int CardMonth { get; set; }
    public string? EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string? ReturnReason { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = "1";
}

/// <summary>
/// 銷退卡報表統計資訊
/// </summary>
public class ReturnCardReportSummary
{
    public int TotalCount { get; set; }
    public decimal TotalAmount { get; set; }
}

