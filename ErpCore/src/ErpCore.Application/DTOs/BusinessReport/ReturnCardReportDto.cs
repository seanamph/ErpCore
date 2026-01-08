namespace ErpCore.Application.DTOs.BusinessReport;

/// <summary>
/// 銷退卡報表 DTO (SYSL310)
/// </summary>
public class ReturnCardReportDto
{
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
    public string? ReportName { get; set; }
}

/// <summary>
/// 銷退卡報表查詢 DTO (SYSL310)
/// </summary>
public class ReturnCardReportQueryDto
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
/// 銷退卡報表結果 DTO (SYSL310)
/// </summary>
public class ReturnCardReportResultDto
{
    public List<ReturnCardReportDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public ReturnCardReportSummaryDto? Summary { get; set; }
}

/// <summary>
/// 銷退卡報表統計資訊 DTO (SYSL310)
/// </summary>
public class ReturnCardReportSummaryDto
{
    public int TotalCount { get; set; }
    public decimal TotalAmount { get; set; }
}

