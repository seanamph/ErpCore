namespace ErpCore.Application.DTOs.Purchase;

/// <summary>
/// 採購單 DTO
/// </summary>
public class PurchaseOrderDto
{
    public string OrderId { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string OrderType { get; set; } = string.Empty;
    public string ShopId { get; set; } = string.Empty;
    public string? ShopName { get; set; }
    public string SupplierId { get; set; } = string.Empty;
    public string? SupplierName { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ApplyUserId { get; set; }
    public string? ApplyUserName { get; set; }
    public DateTime? ApplyDate { get; set; }
    public string? ApproveUserId { get; set; }
    public string? ApproveUserName { get; set; }
    public DateTime? ApproveDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalQty { get; set; }
    public string? Memo { get; set; }
    public DateTime? ExpectedDate { get; set; }
    public string? SiteId { get; set; }
    public string? OrgId { get; set; }
    public string? CurrencyId { get; set; } = "TWD";
    public decimal? ExchangeRate { get; set; } = 1;
    public string? SourceProgram { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<PurchaseOrderDetailDto> Details { get; set; } = new();
}

/// <summary>
/// 採購單明細 DTO
/// </summary>
public class PurchaseOrderDetailDto
{
    public Guid DetailId { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public int LineNum { get; set; }
    public string GoodsId { get; set; } = string.Empty;
    public string? GoodsName { get; set; }
    public string? BarcodeId { get; set; }
    public decimal OrderQty { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? Amount { get; set; }
    public decimal ReceivedQty { get; set; }
    public decimal ReturnQty { get; set; }
    public string? UnitId { get; set; }
    public decimal? TaxRate { get; set; } = 0;
    public decimal? TaxAmount { get; set; } = 0;
    public string? Memo { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// 建立採購單 DTO
/// </summary>
public class CreatePurchaseOrderDto
{
    public DateTime OrderDate { get; set; }
    public string OrderType { get; set; } = "PO";
    public string ShopId { get; set; } = string.Empty;
    public string SupplierId { get; set; } = string.Empty;
    public string? Memo { get; set; }
    public DateTime? ExpectedDate { get; set; }
    public string? SiteId { get; set; }
    public string? OrgId { get; set; }
    public string? CurrencyId { get; set; } = "TWD";
    public decimal? ExchangeRate { get; set; } = 1;
    public string? SourceProgram { get; set; }
    public List<CreatePurchaseOrderDetailDto> Details { get; set; } = new();
}

/// <summary>
/// 建立採購單明細 DTO
/// </summary>
public class CreatePurchaseOrderDetailDto
{
    public int LineNum { get; set; }
    public string GoodsId { get; set; } = string.Empty;
    public string? BarcodeId { get; set; }
    public decimal OrderQty { get; set; }
    public decimal? UnitPrice { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 修改採購單 DTO
/// </summary>
public class UpdatePurchaseOrderDto
{
    public DateTime OrderDate { get; set; }
    public string OrderType { get; set; } = "PO";
    public string ShopId { get; set; } = string.Empty;
    public string SupplierId { get; set; } = string.Empty;
    public string? Memo { get; set; }
    public DateTime? ExpectedDate { get; set; }
    public string? SiteId { get; set; }
    public string? OrgId { get; set; }
    public string? CurrencyId { get; set; } = "TWD";
    public decimal? ExchangeRate { get; set; } = 1;
    public List<UpdatePurchaseOrderDetailDto> Details { get; set; } = new();
}

/// <summary>
/// 修改採購單明細 DTO
/// </summary>
public class UpdatePurchaseOrderDetailDto
{
    public Guid? DetailId { get; set; }
    public int LineNum { get; set; }
    public string GoodsId { get; set; } = string.Empty;
    public string? BarcodeId { get; set; }
    public decimal OrderQty { get; set; }
    public decimal? UnitPrice { get; set; }
    public string? UnitId { get; set; }
    public decimal? TaxRate { get; set; } = 0;
    public string? Memo { get; set; }
}

/// <summary>
/// 查詢採購單 DTO
/// </summary>
public class PurchaseOrderQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? OrderId { get; set; }
    public string? OrderType { get; set; }
    public string? ShopId { get; set; }
    public string? SupplierId { get; set; }
    public string? Status { get; set; }
    public DateTime? OrderDateFrom { get; set; }
    public DateTime? OrderDateTo { get; set; }
    public string? SourceProgram { get; set; }
}

/// <summary>
/// 採購單詳細資訊 DTO (含明細)
/// </summary>
public class PurchaseOrderFullDto : PurchaseOrderDto
{
    // 繼承 PurchaseOrderDto，已包含 Details
}

// =============================================
// SYSP310-SYSP330 採購單查詢系列 DTO
// =============================================

/// <summary>
/// 採購單查詢結果 DTO (用於查詢列表)
/// </summary>
public class PurchaseOrderQueryResultDto
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
/// 採購單查詢請求 DTO
/// </summary>
public class PurchaseOrderQueryRequestDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; } = "ASC";
    public PurchaseOrderQueryFiltersDto? Filters { get; set; }
}

/// <summary>
/// 採購單查詢篩選條件 DTO
/// </summary>
public class PurchaseOrderQueryFiltersDto
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
/// 採購單明細查詢結果 DTO
/// </summary>
public class PurchaseOrderDetailQueryDto
{
    public string OrderId { get; set; } = string.Empty;
    public List<PurchaseOrderDetailQueryItemDto> Details { get; set; } = new();
}

/// <summary>
/// 採購單明細查詢項目 DTO
/// </summary>
public class PurchaseOrderDetailQueryItemDto
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
/// 採購單統計結果 DTO
/// </summary>
public class PurchaseOrderStatisticsDto
{
    public PurchaseOrderStatisticsSummaryDto Summary { get; set; } = new();
    public List<PurchaseOrderStatisticsDetailDto> Details { get; set; } = new();
}

/// <summary>
/// 採購單統計摘要 DTO
/// </summary>
public class PurchaseOrderStatisticsSummaryDto
{
    public int TotalOrders { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalQty { get; set; }
    public decimal AvgAmount { get; set; }
}

/// <summary>
/// 採購單統計明細 DTO
/// </summary>
public class PurchaseOrderStatisticsDetailDto
{
    public string GroupKey { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public int OrderCount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalQty { get; set; }
}

/// <summary>
/// 採購單統計請求 DTO
/// </summary>
public class PurchaseOrderStatisticsRequestDto
{
    public string GroupBy { get; set; } = "supplier"; // supplier, shop, goods, status, date
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? ShopId { get; set; }
    public string? SupplierId { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 採購單匯出請求 DTO
/// </summary>
public class PurchaseOrderExportRequestDto
{
    public PurchaseOrderQueryFiltersDto? Filters { get; set; }
    public string ExportType { get; set; } = "excel"; // excel, csv, pdf
    public bool IncludeDetails { get; set; } = false;
}

