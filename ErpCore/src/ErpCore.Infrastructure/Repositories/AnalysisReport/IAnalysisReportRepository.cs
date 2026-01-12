using ErpCore.Domain.Entities.AnalysisReport;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.AnalysisReport;

/// <summary>
/// 分析報表 Repository 介面 (SYSA1000)
/// </summary>
public interface IAnalysisReportRepository
{
    /// <summary>
    /// 查詢商品分析報表 (SYSA1011)
    /// </summary>
    Task<PagedResult<SYSA1011ReportItem>> GetSYSA1011ReportAsync(SYSA1011Query query);

    /// <summary>
    /// 查詢商品分類列表
    /// </summary>
    Task<List<GoodsCategory>> GetGoodsCategoriesAsync(string categoryType, string? parentId = null);

    /// <summary>
    /// 查詢進銷存月報表 (SYSA1012)
    /// </summary>
    Task<PagedResult<SYSA1012ReportItem>> GetSYSA1012ReportAsync(SYSA1012Query query);

    /// <summary>
    /// 查詢進銷存分析報表 (SYSA1000) - 通用查詢方法
    /// </summary>
    Task<PagedResult<Dictionary<string, object>>> GetAnalysisReportAsync(string reportId, AnalysisReportQuery query);
}

/// <summary>
/// SYSA1011 查詢條件
/// </summary>
public class SYSA1011Query : PagedQuery
{
    public string? SiteId { get; set; }
    public string? BId { get; set; }
    public string? MId { get; set; }
    public string? SId { get; set; }
    public string? GoodsId { get; set; }
    public string? FilterType { get; set; } // 篩選類型 (全部、低於安全庫存量等)
}

/// <summary>
/// SYSA1011 報表項目
/// </summary>
public class SYSA1011ReportItem
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
/// SYSA1012 查詢條件
/// </summary>
public class SYSA1012Query : PagedQuery
{
    public string? SiteId { get; set; }
    public string? GoodsId { get; set; }
    public string? BId { get; set; }
    public string? MId { get; set; }
    public string? SId { get; set; }
    public string? ReportMonth { get; set; } // YYYYMM
}

/// <summary>
/// SYSA1012 報表項目
/// </summary>
public class SYSA1012ReportItem
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
/// 進銷存分析報表查詢條件 (SYSA1000)
/// </summary>
public class AnalysisReportQuery : PagedQuery
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
