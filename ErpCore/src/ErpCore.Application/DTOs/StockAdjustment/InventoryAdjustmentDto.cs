namespace ErpCore.Application.DTOs.StockAdjustment;

/// <summary>
/// 庫存調整單 DTO
/// </summary>
public class InventoryAdjustmentDto
{
    public string AdjustmentId { get; set; } = string.Empty;
    public DateTime AdjustmentDate { get; set; }
    public string ShopId { get; set; } = string.Empty;
    public string? ShopName { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? StatusName { get; set; }
    public string? AdjustmentType { get; set; }
    public string? AdjustmentUser { get; set; }
    public string? UserName { get; set; }
    public string? Memo { get; set; }
    public string? Memo2 { get; set; }
    public string? SourceNo { get; set; }
    public string? SourceNum { get; set; }
    public DateTime? SourceCheckDate { get; set; }
    public string? SourceSuppId { get; set; }
    public string? SiteId { get; set; }
    public decimal TotalQty { get; set; }
    public decimal TotalCost { get; set; }
    public decimal TotalAmount { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<InventoryAdjustmentDetailDto> Details { get; set; } = new();
}

/// <summary>
/// 庫存調整單明細 DTO
/// </summary>
public class InventoryAdjustmentDetailDto
{
    public Guid DetailId { get; set; }
    public string AdjustmentId { get; set; } = string.Empty;
    public int LineNum { get; set; }
    public string GoodsId { get; set; } = string.Empty;
    public string? GoodsName { get; set; }
    public string? BarcodeId { get; set; }
    public decimal AdjustmentQty { get; set; }
    public decimal? BeforeQty { get; set; }
    public decimal? AfterQty { get; set; }
    public decimal? UnitCost { get; set; }
    public decimal? AdjustmentCost { get; set; }
    public decimal? AdjustmentAmount { get; set; }
    public string? Reason { get; set; }
    public string? ReasonName { get; set; }
    public string? Memo { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 建立庫存調整單 DTO
/// </summary>
public class CreateInventoryAdjustmentDto
{
    public DateTime AdjustmentDate { get; set; }
    public string ShopId { get; set; } = string.Empty;
    public string? AdjustmentType { get; set; }
    public string? Memo { get; set; }
    public string? Memo2 { get; set; }
    public string? SourceNo { get; set; }
    public string? SourceNum { get; set; }
    public DateTime? SourceCheckDate { get; set; }
    public string? SourceSuppId { get; set; }
    public string? SiteId { get; set; }
    public string? AdjustmentUser { get; set; }
    public List<CreateInventoryAdjustmentDetailDto> Details { get; set; } = new();
}

/// <summary>
/// 建立庫存調整單明細 DTO
/// </summary>
public class CreateInventoryAdjustmentDetailDto
{
    public string GoodsId { get; set; } = string.Empty;
    public string? BarcodeId { get; set; }
    public decimal AdjustmentQty { get; set; }
    public decimal? UnitCost { get; set; }
    public string? Reason { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 修改庫存調整單 DTO
/// </summary>
public class UpdateInventoryAdjustmentDto
{
    public DateTime AdjustmentDate { get; set; }
    public string? AdjustmentType { get; set; }
    public string? Memo { get; set; }
    public string? Memo2 { get; set; }
    public string? SourceNo { get; set; }
    public string? SourceNum { get; set; }
    public DateTime? SourceCheckDate { get; set; }
    public string? SourceSuppId { get; set; }
    public string? SiteId { get; set; }
    public string? AdjustmentUser { get; set; }
    public List<UpdateInventoryAdjustmentDetailDto> Details { get; set; } = new();
}

/// <summary>
/// 修改庫存調整單明細 DTO
/// </summary>
public class UpdateInventoryAdjustmentDetailDto
{
    public Guid? DetailId { get; set; }
    public string GoodsId { get; set; } = string.Empty;
    public string? BarcodeId { get; set; }
    public decimal AdjustmentQty { get; set; }
    public decimal? UnitCost { get; set; }
    public string? Reason { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 查詢庫存調整單 DTO
/// </summary>
public class InventoryAdjustmentQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? AdjustmentId { get; set; }
    public string? ShopId { get; set; }
    public string? Status { get; set; }
    public DateTime? AdjustmentDateFrom { get; set; }
    public DateTime? AdjustmentDateTo { get; set; }
    public string? AdjustmentUser { get; set; }
}

/// <summary>
/// 調整原因 DTO
/// </summary>
public class AdjustmentReasonDto
{
    public string ReasonId { get; set; } = string.Empty;
    public string ReasonName { get; set; } = string.Empty;
    public string? ReasonType { get; set; }
    public string Status { get; set; } = string.Empty;
}

