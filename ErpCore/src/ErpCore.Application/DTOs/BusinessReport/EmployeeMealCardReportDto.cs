namespace ErpCore.Application.DTOs.BusinessReport;

/// <summary>
/// 員餐卡報表 DTO (SYSL210)
/// </summary>
public class EmployeeMealCardReportDto
{
    public string SiteId { get; set; } = string.Empty;
    public string? SiteName { get; set; }
    public string? OrgId { get; set; }
    public string? OrgName { get; set; }
    public string? CardSurfaceId { get; set; }
    public string? TxnNo { get; set; }
    public string? Act1 { get; set; }
    public string? Act1Name { get; set; }
    public decimal Amt1 { get; set; }
    public decimal Amt4 { get; set; }
    public decimal Amt5 { get; set; }
    public string? ReportName { get; set; }
}

/// <summary>
/// 員餐卡報表查詢 DTO (SYSL210)
/// </summary>
public class EmployeeMealCardReportQueryDto
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

/// <summary>
/// 報表類型選項 DTO (SYSL210)
/// </summary>
public class ReportTypeOptionDto
{
    public string Value { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
}

