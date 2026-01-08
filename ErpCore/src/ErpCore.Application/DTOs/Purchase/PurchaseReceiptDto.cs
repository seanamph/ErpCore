namespace ErpCore.Application.DTOs.Purchase;

/// <summary>
/// 採購驗收單 DTO
/// </summary>
public class PurchaseReceiptDto
{
    public string ReceiptId { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public DateTime ReceiptDate { get; set; }
    public string ShopId { get; set; } = string.Empty;
    public string SupplierId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ReceiptUserId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalQty { get; set; }
    public string? Memo { get; set; }
    public bool IsSettled { get; set; }
    public DateTime? SettledDate { get; set; }
    public string PurchaseOrderType { get; set; } = "1";
    public bool IsSettledAdjustment { get; set; }
    public string? OriginalReceiptId { get; set; }
    public string? AdjustmentReason { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<PurchaseReceiptDetailDto> Details { get; set; } = new();
}

/// <summary>
/// 採購驗收單明細 DTO
/// </summary>
public class PurchaseReceiptDetailDto
{
    public Guid DetailId { get; set; }
    public string ReceiptId { get; set; } = string.Empty;
    public Guid? OrderDetailId { get; set; }
    public int LineNum { get; set; }
    public string GoodsId { get; set; } = string.Empty;
    public string? BarcodeId { get; set; }
    public decimal OrderQty { get; set; }
    public decimal ReceiptQty { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? Amount { get; set; }
    public decimal? OriginalReceiptQty { get; set; }
    public decimal? AdjustmentQty { get; set; }
    public decimal? OriginalUnitPrice { get; set; }
    public decimal? AdjustmentPrice { get; set; }
    public string? Memo { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 建立採購驗收單 DTO
/// </summary>
public class CreatePurchaseReceiptDto
{
    public string OrderId { get; set; } = string.Empty;
    public DateTime ReceiptDate { get; set; }
    public string? ReceiptUserId { get; set; }
    public string? Memo { get; set; }
    public List<CreatePurchaseReceiptDetailDto> Details { get; set; } = new();
}

/// <summary>
/// 建立採購驗收單明細 DTO
/// </summary>
public class CreatePurchaseReceiptDetailDto
{
    public Guid? OrderDetailId { get; set; }
    public int LineNum { get; set; }
    public string GoodsId { get; set; } = string.Empty;
    public string? BarcodeId { get; set; }
    public decimal ReceiptQty { get; set; }
    public decimal? UnitPrice { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 修改採購驗收單 DTO
/// </summary>
public class UpdatePurchaseReceiptDto
{
    public DateTime ReceiptDate { get; set; }
    public string? ReceiptUserId { get; set; }
    public string? Memo { get; set; }
    public List<UpdatePurchaseReceiptDetailDto> Details { get; set; } = new();
}

/// <summary>
/// 修改採購驗收單明細 DTO
/// </summary>
public class UpdatePurchaseReceiptDetailDto
{
    public Guid? DetailId { get; set; }
    public decimal ReceiptQty { get; set; }
    public decimal? UnitPrice { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 查詢採購驗收單 DTO
/// </summary>
public class PurchaseReceiptQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ReceiptId { get; set; }
    public string? OrderId { get; set; }
    public string? ShopId { get; set; }
    public string? SupplierId { get; set; }
    public string? Status { get; set; }
    public DateTime? ReceiptDateFrom { get; set; }
    public DateTime? ReceiptDateTo { get; set; }
}

/// <summary>
/// 待驗收採購單查詢 DTO
/// </summary>
public class PendingPurchaseOrderQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? OrderId { get; set; }
    public string? ShopId { get; set; }
    public string? SupplierId { get; set; }
    public DateTime? OrderDateFrom { get; set; }
    public DateTime? OrderDateTo { get; set; }
}

/// <summary>
/// 待驗收採購單 DTO
/// </summary>
public class PendingPurchaseOrderDto
{
    public string OrderId { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string ShopId { get; set; } = string.Empty;
    public string SupplierId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal TotalQty { get; set; }
    public decimal ReceiptQty { get; set; }
    public decimal PendingReceiptQty { get; set; }
}

/// <summary>
/// 採購驗收單完整資料 DTO（含明細）
/// </summary>
public class PurchaseReceiptFullDto
{
    public PurchaseReceiptDto Receipt { get; set; } = new();
}

/// <summary>
/// 已日結採購單驗收調整查詢 DTO (SYSW333)
/// </summary>
public class SettledPurchaseReceiptAdjustmentQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ReceiptId { get; set; }
    public string? OrderId { get; set; }
    public string? ShopId { get; set; }
    public string? SupplierId { get; set; }
    public string? Status { get; set; }
    public DateTime? ReceiptDateFrom { get; set; }
    public DateTime? ReceiptDateTo { get; set; }
}

/// <summary>
/// 已日結退貨單驗退調整查詢 DTO (SYSW530)
/// </summary>
public class ClosedReturnAdjustmentQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ReceiptId { get; set; }
    public string? PurchaseOrderId { get; set; }
    public string? SupplierId { get; set; }
    public string? ShopId { get; set; }
    public string? WarehouseId { get; set; }
    public string? Status { get; set; }
    public DateTime? ApplyDateFrom { get; set; }
    public DateTime? ApplyDateTo { get; set; }
    public DateTime? CheckDateFrom { get; set; }
    public DateTime? CheckDateTo { get; set; }
}

/// <summary>
/// 已日結採購單查詢 DTO (SYSW333)
/// </summary>
public class SettledOrderQueryDto
{
    public string? ShopId { get; set; }
    public string? SupplierId { get; set; }
    public DateTime? OrderDateFrom { get; set; }
    public DateTime? OrderDateTo { get; set; }
}

/// <summary>
/// 已日結退貨單查詢 DTO (SYSW530)
/// </summary>
public class ClosedReturnOrderQueryDto
{
    public string? SupplierId { get; set; }
    public string? WarehouseId { get; set; }
    public string? ShopId { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 審核已日結調整 DTO
/// </summary>
public class ApproveSettledAdjustmentDto
{
    public string ApproveUserId { get; set; } = string.Empty;
    public DateTime ApproveDate { get; set; }
    public string? Notes { get; set; }
}

