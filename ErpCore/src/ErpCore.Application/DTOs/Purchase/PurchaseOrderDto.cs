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
    public DateTime? ApplyDate { get; set; }
    public string? ApproveUserId { get; set; }
    public DateTime? ApproveDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalQty { get; set; }
    public string? Memo { get; set; }
    public DateTime? ExpectedDate { get; set; }
    public string? SiteId { get; set; }
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
    public string? Memo { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
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

