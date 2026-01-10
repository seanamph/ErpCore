namespace ErpCore.Application.DTOs.Accounting;

/// <summary>
/// 財務報表查詢 DTO (SYSN510-SYSN540)
/// </summary>
public class FinancialReportQueryDto
{
    public string ReportType { get; set; } = string.Empty; // 報表類型 (INCOME:損益表, BALANCE:資產負債表, CASHFLOW:現金流量表)
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? StypeId { get; set; }
    public string? SiteId { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// 財務報表資料 DTO
/// </summary>
public class FinancialReportDto
{
    public string ReportType { get; set; } = string.Empty;
    public DateTime ReportDate { get; set; }
    public string StypeId { get; set; } = string.Empty;
    public string? StypeName { get; set; }
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public decimal Balance { get; set; }
}

/// <summary>
/// 匯出財務報表 DTO
/// </summary>
public class ExportFinancialReportDto
{
    public FinancialReportQueryDto Query { get; set; } = new();
    public string ExportFormat { get; set; } = "EXCEL";
}

