namespace ErpCore.Application.DTOs.Purchase;

/// <summary>
/// 採購報表查詢請求 DTO (SYSP410-SYSP4I0)
/// </summary>
public class PurchaseReportQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; } = "ASC";
    public PurchaseReportFiltersDto? Filters { get; set; }
    public string? ReportType { get; set; } // 報表類型: Detail, Summary, Supplier
}

/// <summary>
/// 採購報表查詢篩選條件 DTO
/// </summary>
public class PurchaseReportFiltersDto
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
/// 採購報表查詢結果 DTO
/// </summary>
public class PurchaseReportResultDto
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
/// 採購報表明細查詢結果 DTO
/// </summary>
public class PurchaseReportDetailResultDto
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

/// <summary>
/// 採購報表匯出請求 DTO
/// </summary>
public class PurchaseReportExportDto
{
    public PurchaseReportQueryDto Query { get; set; } = new();
    public string ExportType { get; set; } = "Excel"; // Excel, PDF, CSV
    public string? FileName { get; set; }
}
