using ErpCore.Shared.Common;

namespace ErpCore.Application.DTOs.AnalysisReport;

/// <summary>
/// 商品分析報表 DTO (SYSA1019)
/// </summary>
public class SYSA1019ReportDto
{
    public string SiteId { get; set; } = string.Empty;
    public string? SiteName { get; set; }
    public string OrgId { get; set; } = string.Empty;
    public string? OrgName { get; set; }
    public string ReportName { get; set; } = "商品分析報表";
    public string YearMonth { get; set; } = string.Empty;
    public string? FilterType { get; set; }
    public string SeqNo { get; set; } = string.Empty;
    public string GoodsId { get; set; } = string.Empty;
    public string GoodsName { get; set; } = string.Empty;
}

/// <summary>
/// 商品分析報表查詢 DTO (SYSA1019)
/// </summary>
public class SYSA1019QueryDto : PagedQuery
{
    public string? OrgId { get; set; }
    public string? SiteId { get; set; }
    public string? YearMonth { get; set; } // YYYY-MM
    public string? FilterType { get; set; } // 篩選類型 (全部、特定條件等)
}
