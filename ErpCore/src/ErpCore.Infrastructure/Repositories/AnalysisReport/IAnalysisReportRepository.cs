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
    /// 查詢耗材出庫明細表 (SYSA1013)
    /// </summary>
    Task<PagedResult<SYSA1013ReportItem>> GetSYSA1013ReportAsync(SYSA1013Query query);

    /// <summary>
    /// 查詢商品分析報表 (SYSA1014)
    /// </summary>
    Task<PagedResult<SYSA1014ReportItem>> GetSYSA1014ReportAsync(SYSA1014Query query);

    /// <summary>
    /// 查詢商品分析報表 (SYSA1015)
    /// </summary>
    Task<PagedResult<SYSA1015ReportItem>> GetSYSA1015ReportAsync(SYSA1015Query query);

    /// <summary>
    /// 查詢商品分析報表 (SYSA1016)
    /// </summary>
    Task<PagedResult<SYSA1016ReportItem>> GetSYSA1016ReportAsync(SYSA1016Query query);

    /// <summary>
    /// 查詢商品分析報表 (SYSA1017)
    /// </summary>
    Task<PagedResult<SYSA1017ReportItem>> GetSYSA1017ReportAsync(SYSA1017Query query);

    /// <summary>
    /// 查詢工務維修件數統計報表 (SYSA1018)
    /// </summary>
    Task<PagedResult<SYSA1018ReportItem>> GetSYSA1018ReportAsync(SYSA1018Query query);

    /// <summary>
    /// 查詢商品分析報表 (SYSA1019)
    /// </summary>
    Task<PagedResult<SYSA1019ReportItem>> GetSYSA1019ReportAsync(SYSA1019Query query);

    /// <summary>
    /// 查詢商品分析報表 (SYSA1020)
    /// </summary>
    Task<PagedResult<SYSA1020ReportItem>> GetSYSA1020ReportAsync(SYSA1020Query query);

    /// <summary>
    /// 查詢月成本報表 (SYSA1021)
    /// </summary>
    Task<PagedResult<SYSA1021ReportItem>> GetSYSA1021ReportAsync(SYSA1021Query query);

    /// <summary>
    /// 查詢工務維修統計報表 (SYSA1022)
    /// </summary>
    Task<PagedResult<SYSA1022ReportItem>> GetSYSA1022ReportAsync(SYSA1022Query query);

    /// <summary>
    /// 查詢工務維修統計報表(報表類型) (SYSA1023)
    /// </summary>
    Task<PagedResult<SYSA1023ReportItem>> GetSYSA1023ReportAsync(SYSA1023Query query);

    /// <summary>
    /// 查詢工務維修統計報表(其他) (SYSA1024)
    /// </summary>
    Task<PagedResult<SYSA1024ReportItem>> GetSYSA1024ReportAsync(SYSA1024Query query);

    /// <summary>
    /// 查詢庫存分析報表 (SYSWC10)
    /// </summary>
    Task<PagedResult<SYSWC10ReportItem>> GetSYSWC10ReportAsync(SYSWC10Query query);

    /// <summary>
    /// 查詢銷售分析報表
    /// </summary>
    Task<PagedResult<SalesAnalysisReportItem>> GetSalesAnalysisReportAsync(SalesAnalysisQuery query);

    /// <summary>
    /// 查詢銷售分析報表彙總
    /// </summary>
    Task<SalesAnalysisSummaryItem> GetSalesAnalysisSummaryAsync(SalesAnalysisQuery query);

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
/// SYSA1013 查詢條件
/// </summary>
public class SYSA1013Query : PagedQuery
{
    public string? SiteId { get; set; }
    public string? BId { get; set; }
    public string? MId { get; set; }
    public string? SId { get; set; }
    public string? OrgId { get; set; }
    public string? GoodsId { get; set; }
    public string? BeginDate { get; set; }
    public string? EndDate { get; set; }
    public string? SupplierId { get; set; }
    public string? Use { get; set; }
    public string? FilterType { get; set; } // 篩選類型 (全部、特定狀態等)
}

/// <summary>
/// SYSA1013 報表項目
/// </summary>
public class SYSA1013ReportItem
{
    public string TxnNo { get; set; } = string.Empty; // 出庫單號
    public DateTime TxnDate { get; set; } // 出庫日期
    public string? BId { get; set; } // 大分類
    public string? MId { get; set; } // 中分類
    public string? SId { get; set; } // 小分類
    public string GoodsId { get; set; } = string.Empty; // 商品代碼
    public string GoodsName { get; set; } = string.Empty; // 商品名稱
    public string? PackUnit { get; set; } // 包裝單位
    public string? Unit { get; set; } // 單位
    public decimal Amt { get; set; } // 單價
    public decimal ApplyQty { get; set; } // 申請數量
    public decimal Qty { get; set; } // 數量
    public decimal NAmt { get; set; } // 未稅金額
    public string? Use { get; set; } // 用途
    public string? Vendor { get; set; } // 廠商
    public string? StocksType { get; set; } // 庫存類型
    public string? OrgId { get; set; } // 單位
    public string? OrgAllocation { get; set; } // 單位分攤
}

/// <summary>
/// SYSA1014 查詢條件
/// </summary>
public class SYSA1014Query : PagedQuery
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

/// <summary>
/// SYSA1014 報表項目
/// </summary>
public class SYSA1014ReportItem
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
/// SYSA1015 查詢條件
/// </summary>
public class SYSA1015Query : PagedQuery
{
    public string? SiteId { get; set; }
    public string? BId { get; set; }
    public string? MId { get; set; }
    public string? SId { get; set; }
    public string? GoodsId { get; set; }
    public string? YearMonth { get; set; } // YYYY-MM
    public string? FilterType { get; set; } // 篩選類型 (全部、低於安全庫存量等)
}

/// <summary>
/// SYSA1015 報表項目
/// </summary>
public class SYSA1015ReportItem
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
    public string YearMonth { get; set; } = string.Empty;
}

/// <summary>
/// SYSA1016 查詢條件
/// </summary>
public class SYSA1016Query : PagedQuery
{
    public string? OrgId { get; set; }
    public string? SiteId { get; set; }
    public string? BId { get; set; }
    public string? MId { get; set; }
    public string? SId { get; set; }
    public string? GoodsId { get; set; }
    public string? YearMonth { get; set; } // YYYY-MM
    public string? FilterType { get; set; } // 篩選類型 (全部、低於安全庫存量等)
}

/// <summary>
/// SYSA1016 報表項目
/// </summary>
public class SYSA1016ReportItem
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
    public string YearMonth { get; set; } = string.Empty;
}

/// <summary>
/// SYSA1017 查詢條件
/// </summary>
public class SYSA1017Query : PagedQuery
{
    public string? OrgId { get; set; }
    public string? SiteId { get; set; }
    public string? BId { get; set; }
    public string? MId { get; set; }
    public string? SId { get; set; }
    public string? GoodsId { get; set; }
    public string? YearMonth { get; set; } // YYYYMM
    public string? FilterType { get; set; } // 篩選類型 (全部、低於安全庫存量等)
}

/// <summary>
/// SYSA1017 報表項目
/// </summary>
public class SYSA1017ReportItem
{
    public string SiteId { get; set; } = string.Empty;
    public string? SiteName { get; set; }
    public string ReportName { get; set; } = "商品分析報表";
    public int SeqNo { get; set; }
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
    public string YearMonth { get; set; } = string.Empty;
    public string? OrgId { get; set; }
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
    public string? OtherCondition1 { get; set; } // 其他查詢條件1 (用於 SYSA1024)
    public string? OtherCondition2 { get; set; } // 其他查詢條件2 (用於 SYSA1024)
}

/// <summary>
/// SYSA1018 查詢條件
/// </summary>
public class SYSA1018Query : PagedQuery
{
    public string? OrgId { get; set; }
    public string? YearMonth { get; set; } // YYYY-MM
    public string? FilterType { get; set; } // 篩選類型 (全部、待處理、處理中、已完成、已取消)
}

/// <summary>
/// SYSA1018 報表項目
/// </summary>
public class SYSA1018ReportItem
{
    public string OrgId { get; set; } = string.Empty;
    public string? OrgName { get; set; }
    public string ReportName { get; set; } = "工務維修件數統計表";
    public string YearMonth { get; set; } = string.Empty;
    public string? MaintenanceType { get; set; }
    public string MaintenanceStatus { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public int TotalCount { get; set; }
}

/// <summary>
/// SYSA1019 查詢條件
/// </summary>
public class SYSA1019Query : PagedQuery
{
    public string? OrgId { get; set; }
    public string? SiteId { get; set; }
    public string? YearMonth { get; set; } // YYYY-MM
    public string? FilterType { get; set; } // 篩選類型 (全部、特定條件等)
}

/// <summary>
/// SYSA1019 報表項目
/// </summary>
public class SYSA1019ReportItem
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
/// SYSA1020 查詢條件
/// </summary>
public class SYSA1020Query : PagedQuery
{
    public string? SiteId { get; set; }
    public string? PlanId { get; set; }
    public string? ShowType { get; set; }
    public string? FilterType { get; set; } // 篩選類型 (全部、特定條件等)
}

/// <summary>
/// SYSA1020 報表項目
/// </summary>
public class SYSA1020ReportItem
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
/// SYSA1021 查詢條件
/// </summary>
public class SYSA1021Query : PagedQuery
{
    public string? SiteId { get; set; }
    public string? BId { get; set; }
    public string? MId { get; set; }
    public string? SId { get; set; }
    public string? GoodsId { get; set; }
    public string? YearMonth { get; set; } // YYYYMM
    public string? FilterType { get; set; } // 篩選類型 (全部、有成本、無成本)
}

/// <summary>
/// SYSA1021 報表項目
/// </summary>
public class SYSA1021ReportItem
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
/// SYSA1022 查詢條件
/// </summary>
public class SYSA1022Query : PagedQuery
{
    public string? SiteId { get; set; }
    public string? BelongStatus { get; set; } // 費用負擔
    public string? ApplyDateB { get; set; } // 日統計表起 (YYYY-MM-DD)
    public string? ApplyDateE { get; set; } // 日統計表迄 (YYYY-MM-DD)
    public string? BelongOrg { get; set; } // 費用歸屬單位
    public string? MaintainEmp { get; set; } // 維保人員
    public string? ApplyType { get; set; } // 請修類別
}

/// <summary>
/// SYSA1022 報表項目
/// </summary>
public class SYSA1022ReportItem
{
    public string SiteId { get; set; } = string.Empty;
    public string? SiteName { get; set; }
    public string ReportName { get; set; } = "工務維修統計報表";
    public string? BelongStatus { get; set; } // 費用負擔
    public string ApplyDateB { get; set; } = string.Empty; // 日統計表起
    public string ApplyDateE { get; set; } = string.Empty; // 日統計表迄
    public string? BelongOrg { get; set; } // 費用歸屬單位
    public string? MaintainEmp { get; set; } // 維保人員
    public string? ApplyType { get; set; } // 請修類別
    public int RequestCount { get; set; } // 申請件數
    public decimal TotalAmount { get; set; } // 總金額
}

/// <summary>
/// SYSA1023 查詢條件
/// </summary>
public class SYSA1023Query : PagedQuery
{
    public string? ReportType { get; set; } // 報表類型
    public string? SiteId { get; set; }
    public string? BelongStatus { get; set; } // 費用負擔
    public string? ApplyDateB { get; set; } // 日統計表起 (YYYY-MM-DD)
    public string? ApplyDateE { get; set; } // 日統計表迄 (YYYY-MM-DD)
    public string? BelongOrg { get; set; } // 費用歸屬單位
    public string? MaintainEmp { get; set; } // 維保人員
    public string? ApplyType { get; set; } // 請修類別
}

/// <summary>
/// SYSA1023 報表項目
/// </summary>
public class SYSA1023ReportItem
{
    public string SiteId { get; set; } = string.Empty;
    public string? SiteName { get; set; }
    public string ReportName { get; set; } = "工務維修統計報表(報表類型)";
    public string? ReportType { get; set; } // 報表類型
    public string? BelongStatus { get; set; } // 費用負擔
    public string ApplyDateB { get; set; } = string.Empty; // 日統計表起
    public string ApplyDateE { get; set; } = string.Empty; // 日統計表迄
    public string? BelongOrg { get; set; } // 費用歸屬單位
    public string? MaintainEmp { get; set; } // 維保人員
    public string? ApplyType { get; set; } // 請修類別
    public int RequestCount { get; set; } // 申請件數
    public decimal TotalAmount { get; set; } // 總金額
}

/// <summary>
/// SYSA1024 查詢條件
/// </summary>
public class SYSA1024Query : PagedQuery
{
    public string? SiteId { get; set; }
    public string? BelongStatus { get; set; } // 費用負擔
    public string? ApplyDateB { get; set; } // 日統計表起 (YYYY-MM-DD)
    public string? ApplyDateE { get; set; } // 日統計表迄 (YYYY-MM-DD)
    public string? BelongOrg { get; set; } // 費用歸屬單位
    public string? MaintainEmp { get; set; } // 維保人員
    public string? ApplyType { get; set; } // 請修類別
    public string? OtherCondition1 { get; set; } // 其他查詢條件1
    public string? OtherCondition2 { get; set; } // 其他查詢條件2
}

/// <summary>
/// SYSA1024 報表項目
/// </summary>
public class SYSA1024ReportItem
{
    public string SiteId { get; set; } = string.Empty;
    public string? SiteName { get; set; }
    public string ReportName { get; set; } = "工務維修統計報表(其他)";
    public string? BelongStatus { get; set; } // 費用負擔
    public string ApplyDateB { get; set; } = string.Empty; // 日統計表起
    public string ApplyDateE { get; set; } = string.Empty; // 日統計表迄
    public string? BelongOrg { get; set; } // 費用歸屬單位
    public string? MaintainEmp { get; set; } // 維保人員
    public string? ApplyType { get; set; } // 請修類別
    public string? OtherCondition1 { get; set; } // 其他查詢條件1
    public string? OtherCondition2 { get; set; } // 其他查詢條件2
    public int RequestCount { get; set; } // 申請件數
    public decimal TotalAmount { get; set; } // 總金額
}

/// <summary>
/// SYSWC10 查詢條件
/// </summary>
public class SYSWC10Query : PagedQuery
{
    public string? GoodsIdFrom { get; set; } // 商品代碼起
    public string? GoodsIdTo { get; set; } // 商品代碼迄
    public string? GoodsName { get; set; } // 商品名稱
    public List<string>? SiteIds { get; set; } // 店別列表
    public List<string>? WarehouseIds { get; set; } // 庫別列表
    public List<string>? CategoryIds { get; set; } // 分類列表
    public string? DateFrom { get; set; } // 日期起
    public string? DateTo { get; set; } // 日期迄
    public decimal? MinQty { get; set; } // 最小數量
    public decimal? MaxQty { get; set; } // 最大數量
    public string? Status { get; set; } // 狀態
    public string? BId { get; set; } // 大分類
    public string? MId { get; set; } // 中分類
    public string? SId { get; set; } // 小分類
}

/// <summary>
/// SYSWC10 報表項目
/// </summary>
public class SYSWC10ReportItem
{
    public string SiteId { get; set; } = string.Empty;
    public string? SiteName { get; set; }
    public string GoodsId { get; set; } = string.Empty;
    public string GoodsName { get; set; } = string.Empty;
    public string? BigCategoryId { get; set; }
    public string? BigCategoryName { get; set; }
    public string? MidCategoryId { get; set; }
    public string? MidCategoryName { get; set; }
    public string? SmallCategoryId { get; set; }
    public string? SmallCategoryName { get; set; }
    public string? WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public decimal InQty { get; set; } // 入庫數量
    public decimal OutQty { get; set; } // 出庫數量
    public decimal CurrentQty { get; set; } // 當前庫存數量
    public decimal CurrentAmt { get; set; } // 當前庫存金額
    public DateTime? LastStockDate { get; set; } // 最後庫存異動日期
    public decimal SafeQty { get; set; } // 安全庫存量
    public bool IsLowStock { get; set; } // 是否低庫存
    public bool IsOverStock { get; set; } // 是否過量庫存
}

/// <summary>
/// 銷售分析報表查詢條件
/// </summary>
public class SalesAnalysisQuery : PagedQuery
{
    public string? SiteId { get; set; } // 店別代碼
    public string? DateFrom { get; set; } // 日期起
    public string? DateTo { get; set; } // 日期迄
    public string? BigClassId { get; set; } // 大分類代碼
    public string? MidClassId { get; set; } // 中分類代碼
    public string? SmallClassId { get; set; } // 小分類代碼
    public string? ProductId { get; set; } // 商品代碼
    public string? VendorId { get; set; } // 廠商代碼
    public string? SalesPersonId { get; set; } // 銷售人員代碼
    public string? CustomerId { get; set; } // 客戶代碼
    public string? ReportType { get; set; } // 報表類型 (daily, monthly, yearly, custom)
    public string? GroupBy { get; set; } // 群組方式 (product, category, site, vendor, salesperson)
}

/// <summary>
/// 銷售分析報表項目
/// </summary>
public class SalesAnalysisReportItem
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? BigClassId { get; set; }
    public string? BigClassName { get; set; }
    public string? MidClassId { get; set; }
    public string? MidClassName { get; set; }
    public string? SmallClassId { get; set; }
    public string? SmallClassName { get; set; }
    public string? VendorId { get; set; }
    public string? VendorName { get; set; }
    public string SiteId { get; set; } = string.Empty;
    public string? SiteName { get; set; }
    public string? SalesPersonId { get; set; }
    public string? SalesPersonName { get; set; }
    public decimal TotalQuantity { get; set; } // 總數量
    public decimal TotalAmount { get; set; } // 總金額
    public decimal TotalCost { get; set; } // 總成本
    public decimal TotalProfit { get; set; } // 總毛利
    public decimal ProfitRate { get; set; } // 毛利率
    public int OrderCount { get; set; } // 訂單筆數
    public decimal AvgUnitPrice { get; set; } // 平均單價
    public decimal AvgQuantity { get; set; } // 平均數量
}

/// <summary>
/// 銷售分析報表彙總項目
/// </summary>
public class SalesAnalysisSummaryItem
{
    public decimal TotalQuantity { get; set; } // 總數量
    public decimal TotalAmount { get; set; } // 總金額
    public decimal TotalCost { get; set; } // 總成本
    public decimal TotalProfit { get; set; } // 總毛利
    public decimal AvgProfitRate { get; set; } // 平均毛利率
    public int TotalOrderCount { get; set; } // 總訂單筆數
}
