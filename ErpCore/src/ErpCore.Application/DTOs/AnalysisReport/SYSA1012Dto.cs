using ErpCore.Shared.Common;

namespace ErpCore.Application.DTOs.AnalysisReport;

/// <summary>
/// 進銷存月報表 DTO (SYSA1012)
/// </summary>
public class SYSA1012ReportDto
{
    public string SiteId { get; set; } = string.Empty;
    public string? SiteName { get; set; }
    public string GoodsId { get; set; } = string.Empty;
    public string GoodsName { get; set; } = string.Empty;
    public string ReportMonth { get; set; } = string.Empty; // YYYYMM
    public decimal BeginQty { get; set; }
    public decimal BeginAmt { get; set; }
    public decimal InQty { get; set; }
    public decimal InAmt { get; set; }
    public decimal OutQty { get; set; }
    public decimal OutAmt { get; set; }
    public decimal EndQty { get; set; }
    public decimal EndAmt { get; set; }
}

/// <summary>
/// 進銷存月報表查詢 DTO (SYSA1012)
/// </summary>
public class SYSA1012QueryDto : PagedQuery
{
    public string? SiteId { get; set; }
    public string? GoodsId { get; set; }
    public string? BId { get; set; }
    public string? MId { get; set; }
    public string? SId { get; set; }
    public string? ReportMonth { get; set; } // YYYYMM
}
