using ErpCore.Shared.Common;

namespace ErpCore.Application.DTOs.AnalysisReport;

/// <summary>
/// 商品分析報表 DTO (SYSA1020)
/// </summary>
public class SYSA1020ReportDto
{
    public string SiteId { get; set; } = string.Empty;
    public string? SiteName { get; set; }
    public string PlanId { get; set; } = string.Empty;
    public string? PlanName { get; set; }
    public string? ShowType { get; set; }
    public string? FilterType { get; set; }
    public string SeqNo { get; set; } = string.Empty;
    public string GoodsId { get; set; } = string.Empty;
    public string GoodsName { get; set; } = string.Empty;
}

/// <summary>
/// 商品分析報表查詢 DTO (SYSA1020)
/// </summary>
public class SYSA1020QueryDto : PagedQuery
{
    public string? SiteId { get; set; }
    public string? PlanId { get; set; }
    public string? ShowType { get; set; }
    public string? FilterType { get; set; } // 篩選類型 (全部、特定條件等)
}
