namespace ErpCore.Shared.Common;

/// <summary>
/// 採購單查詢結果模型 (用於 Repository 層)
/// </summary>
public class PurchaseOrderQueryResult
{
    public string OrderId { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string OrderType { get; set; } = string.Empty;
    public string OrderTypeName { get; set; } = string.Empty;
    public string ShopId { get; set; } = string.Empty;
    public string ShopName { get; set; } = string.Empty;
    public string SupplierId { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string StatusName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal TotalQty { get; set; }
    public string? ApplyUserId { get; set; }
    public string ApplyUserName { get; set; } = string.Empty;
    public DateTime? ApplyDate { get; set; }
    public string? ApproveUserId { get; set; }
    public string ApproveUserName { get; set; } = string.Empty;
    public DateTime? ApproveDate { get; set; }
    public DateTime? ExpectedDate { get; set; }
    public int DetailCount { get; set; }
    public decimal TotalReceivedQty { get; set; }
    public decimal TotalReturnQty { get; set; }
    public string? Memo { get; set; }
    public string? SiteId { get; set; }
    public string? OrgId { get; set; }
    public string? CurrencyId { get; set; }
    public decimal? ExchangeRate { get; set; }
    public string? SourceProgram { get; set; }
}

/// <summary>
/// 採購單查詢請求模型
/// </summary>
public class PurchaseOrderQueryRequest
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; } = "ASC";
    public PurchaseOrderQueryFilters? Filters { get; set; }
}

/// <summary>
/// 採購單查詢篩選條件模型
/// </summary>
public class PurchaseOrderQueryFilters
{
    public string? OrderId { get; set; }
    public string? OrderType { get; set; }
    public string? ShopId { get; set; }
    public string? SupplierId { get; set; }
    public string? Status { get; set; }
    public DateTime? OrderDateFrom { get; set; }
    public DateTime? OrderDateTo { get; set; }
    public string? ApplyUserId { get; set; }
    public string? ApproveUserId { get; set; }
    public DateTime? ExpectedDateFrom { get; set; }
    public DateTime? ExpectedDateTo { get; set; }
    public decimal? MinTotalAmount { get; set; }
    public decimal? MaxTotalAmount { get; set; }
}

/// <summary>
/// 採購單明細查詢項目模型
/// </summary>
public class PurchaseOrderDetailQueryItem
{
    public Guid DetailId { get; set; }
    public int LineNum { get; set; }
    public string GoodsId { get; set; } = string.Empty;
    public string GoodsName { get; set; } = string.Empty;
    public string? BarcodeId { get; set; }
    public decimal OrderQty { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? Amount { get; set; }
    public decimal ReceivedQty { get; set; }
    public decimal ReturnQty { get; set; }
    public decimal PendingQty { get; set; }
    public string? UnitId { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 採購單統計結果模型
/// </summary>
public class PurchaseOrderStatistics
{
    public PurchaseOrderStatisticsSummary Summary { get; set; } = new();
    public List<PurchaseOrderStatisticsDetail> Details { get; set; } = new();
}

/// <summary>
/// 採購單統計摘要模型
/// </summary>
public class PurchaseOrderStatisticsSummary
{
    public int TotalOrders { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalQty { get; set; }
    public decimal AvgAmount { get; set; }
}

/// <summary>
/// 採購單統計明細模型
/// </summary>
public class PurchaseOrderStatisticsDetail
{
    public string GroupKey { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public int OrderCount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalQty { get; set; }
}

/// <summary>
/// 採購單統計請求模型
/// </summary>
public class PurchaseOrderStatisticsRequest
{
    public string GroupBy { get; set; } = "supplier"; // supplier, shop, goods, status, date
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? ShopId { get; set; }
    public string? SupplierId { get; set; }
    public string? Status { get; set; }
}

// =============================================
// 採購報表查詢模型 (用於 Repository 層)
// =============================================

/// <summary>
/// 採購報表查詢請求模型
/// </summary>
public class PurchaseReportQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; } = "ASC";
    public PurchaseReportFilters? Filters { get; set; }
    public string? ReportType { get; set; } // 報表類型: Detail, Summary, Supplier
}

/// <summary>
/// 採購報表查詢篩選條件模型
/// </summary>
public class PurchaseReportFilters
{
    public string? OrderId { get; set; }
    public string? OrderType { get; set; }
    public string? ShopId { get; set; }
    public string? SupplierId { get; set; }
    public string? Status { get; set; }
    public DateTime? OrderDateFrom { get; set; }
    public DateTime? OrderDateTo { get; set; }
    public string? GoodsId { get; set; }
    public string? ApplyUserId { get; set; }
    public string? ApproveUserId { get; set; }
}

/// <summary>
/// 採購報表查詢結果模型
/// </summary>
public class PurchaseReportResult
{
    public string OrderId { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string OrderType { get; set; } = string.Empty;
    public string? OrderTypeName { get; set; }
    public string ShopId { get; set; } = string.Empty;
    public string? ShopName { get; set; }
    public string SupplierId { get; set; } = string.Empty;
    public string? SupplierName { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? StatusName { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalQty { get; set; }
    public string? ApplyUserId { get; set; }
    public string? ApplyUserName { get; set; }
    public DateTime? ApplyDate { get; set; }
    public string? ApproveUserId { get; set; }
    public string? ApproveUserName { get; set; }
    public DateTime? ApproveDate { get; set; }
    public DateTime? ExpectedDate { get; set; }
    public string? Memo { get; set; }
    public string? SiteId { get; set; }
    public string? OrgId { get; set; }
    public string? CurrencyId { get; set; }
    public decimal? ExchangeRate { get; set; }
    public int DetailCount { get; set; }
    public decimal TotalReceivedQty { get; set; }
    public decimal TotalReturnQty { get; set; }
    public decimal TotalDetailAmount { get; set; }
}

/// <summary>
/// 採購報表明細查詢結果模型
/// </summary>
public class PurchaseReportDetailResult
{
    public Guid DetailId { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string OrderType { get; set; } = string.Empty;
    public string? OrderTypeName { get; set; }
    public string ShopId { get; set; } = string.Empty;
    public string? ShopName { get; set; }
    public string SupplierId { get; set; } = string.Empty;
    public string? SupplierName { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? StatusName { get; set; }
    public int LineNum { get; set; }
    public string GoodsId { get; set; } = string.Empty;
    public string? GoodsName { get; set; }
    public string? BarcodeId { get; set; }
    public decimal OrderQty { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? Amount { get; set; }
    public decimal ReceivedQty { get; set; }
    public decimal ReturnQty { get; set; }
    public decimal PendingQty { get; set; }
    public string? UnitId { get; set; }
    public decimal? TaxRate { get; set; }
    public decimal? TaxAmount { get; set; }
    public string? Memo { get; set; }
    public decimal? OrderTotalAmount { get; set; }
    public string? CurrencyId { get; set; }
    public decimal? ExchangeRate { get; set; }
}
