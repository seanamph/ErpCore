using ErpCore.Shared.Common;

namespace ErpCore.Application.DTOs.AnalysisReport;

/// <summary>
/// 商品分析報表 DTO (SYSA1011)
/// </summary>
public class SYSA1011ReportDto
{
    public string SiteId { get; set; } = string.Empty;
    public string? SiteName { get; set; }
    public string ReportName { get; set; } = "商品分析報表";
    public string SeqNo { get; set; } = string.Empty;
    public string? BId { get; set; }
    public string? MId { get; set; }
    public string? SId { get; set; }
    public string GoodsId { get; set; } = string.Empty;
    public string GoodsName { get; set; } = string.Empty;
    public string? PackUnit { get; set; }
    public string? Unit { get; set; }
    public decimal Qty { get; set; }
    public decimal SafeQty { get; set; }
    public string SelectType { get; set; } = "全部";
}

/// <summary>
/// 商品分析報表查詢 DTO (SYSA1011)
/// </summary>
public class SYSA1011QueryDto : PagedQuery
{
    public string? SiteId { get; set; }
    public string? BId { get; set; }
    public string? MId { get; set; }
    public string? SId { get; set; }
    public string? GoodsId { get; set; }
    public string? FilterType { get; set; } // 篩選類型 (全部、低於安全庫存量等)
}
