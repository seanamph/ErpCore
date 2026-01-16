using ErpCore.Shared.Common;

namespace ErpCore.Application.DTOs.AnalysisReport;

/// <summary>
/// 月成本報表 DTO (SYSA1021)
/// </summary>
public class SYSA1021ReportDto
{
    public string SiteId { get; set; } = string.Empty;
    public string? SiteName { get; set; }
    public string ReportName { get; set; } = "月成本報表";
    public string YearMonth { get; set; } = string.Empty; // YYYYMM
    public string? BId { get; set; }
    public string? MId { get; set; }
    public string? SId { get; set; }
    public string GoodsId { get; set; } = string.Empty;
    public string GoodsName { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public decimal CostAmount { get; set; }
    public decimal AvgCost { get; set; }
}

/// <summary>
/// 月成本報表查詢 DTO (SYSA1021)
/// </summary>
public class SYSA1021QueryDto : PagedQuery
{
    public string? SiteId { get; set; }
    public string? BId { get; set; }
    public string? MId { get; set; }
    public string? SId { get; set; }
    public string? GoodsId { get; set; }
    public string? YearMonth { get; set; } // YYYYMM
    public string? FilterType { get; set; } // 篩選類型 (全部、有成本、無成本)
}
