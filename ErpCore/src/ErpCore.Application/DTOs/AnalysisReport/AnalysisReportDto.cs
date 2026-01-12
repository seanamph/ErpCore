using ErpCore.Shared.Common;

namespace ErpCore.Application.DTOs.AnalysisReport;

/// <summary>
/// 進銷存分析報表查詢 DTO (SYSA1000)
/// </summary>
public class AnalysisReportQueryDto : PagedQuery
{
    public string? SiteId { get; set; }
    public string? YearMonth { get; set; } // YYYY/MM
    public string? BeginDate { get; set; }
    public string? EndDate { get; set; }
    public string? BId { get; set; }
    public string? MId { get; set; }
    public string? SId { get; set; }
    public string? GoodsId { get; set; }
    public string? FilterType { get; set; }
    public string? OrgId { get; set; }
    public string? Vendor { get; set; }
    public string? Use { get; set; }
    public string? BelongStatus { get; set; }
    public string? ApplyDateB { get; set; }
    public string? ApplyDateE { get; set; }
    public string? StartMonth { get; set; }
    public string? EndMonth { get; set; }
    public string? DateType { get; set; }
    public string? MaintainEmp { get; set; }
    public string? BelongOrg { get; set; }
    public string? ApplyType { get; set; }
}

/// <summary>
/// 進銷存分析報表回應 DTO (SYSA1000)
/// </summary>
public class AnalysisReportDto
{
    public string ReportId { get; set; } = string.Empty;
    public string ReportName { get; set; } = string.Empty;
    public string? SiteName { get; set; }
    public List<Dictionary<string, object>> Items { get; set; } = new();
    public int TotalCount { get; set; }
}

/// <summary>
/// 匯出報表請求 DTO
/// </summary>
public class ExportReportDto
{
    public string Format { get; set; } = "Excel"; // Excel 或 PDF
    public AnalysisReportQueryDto QueryParams { get; set; } = new();
}

/// <summary>
/// 列印報表回應 DTO
/// </summary>
public class PrintReportDto
{
    public byte[] Data { get; set; } = Array.Empty<byte>();
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = "application/pdf";
}
