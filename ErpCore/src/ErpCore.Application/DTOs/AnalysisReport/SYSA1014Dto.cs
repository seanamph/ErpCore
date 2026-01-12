using ErpCore.Shared.Common;

namespace ErpCore.Application.DTOs.AnalysisReport;

/// <summary>
/// 商品分析報表 DTO (SYSA1014)
/// </summary>
public class SYSA1014ReportDto
{
    public string SiteId { get; set; } = string.Empty;
    public string? SiteName { get; set; }
    public string ReportName { get; set; } = "商品分析報表";
    public string SelectDate { get; set; } = string.Empty;
    public string SelectType { get; set; } = "全部";
    public string SeqNo { get; set; } = string.Empty;
    public string? BId { get; set; }
    public string? MId { get; set; }
    public string? SId { get; set; }
    public string GoodsId { get; set; } = string.Empty;
    public string GoodsName { get; set; } = string.Empty;
    public string? PackUnit { get; set; }
    public string? Unit { get; set; }
    public decimal PurchaseQty { get; set; }
    public decimal SalesQty { get; set; }
    public decimal StockQty { get; set; }
    public DateTime BeginDate { get; set; }
    public DateTime EndDate { get; set; }
}

/// <summary>
/// 商品分析報表查詢 DTO (SYSA1014)
/// </summary>
public class SYSA1014QueryDto : PagedQuery
{
    public string? SiteId { get; set; }
    public string? BId { get; set; }
    public string? MId { get; set; }
    public string? SId { get; set; }
    public string? OrgId { get; set; }
    public string? GoodsId { get; set; }
    public string? BeginDate { get; set; }
    public string? EndDate { get; set; }
}
