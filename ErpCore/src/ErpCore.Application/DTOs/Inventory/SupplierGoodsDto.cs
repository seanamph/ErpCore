namespace ErpCore.Application.DTOs.Inventory;

/// <summary>
/// 供應商商品 DTO
/// </summary>
public class SupplierGoodsDto
{
    public string SupplierId { get; set; } = string.Empty;
    public string? SupplierName { get; set; }
    public string BarcodeId { get; set; } = string.Empty;
    public string? BarcodeName { get; set; }
    public string ShopId { get; set; } = string.Empty;
    public string? ShopName { get; set; }
    public decimal Lprc { get; set; }
    public decimal Mprc { get; set; }
    public string Tax { get; set; } = "1";
    public decimal MinQty { get; set; }
    public decimal MaxQty { get; set; }
    public string? Unit { get; set; }
    public decimal Rate { get; set; } = 1;
    public string Status { get; set; } = "0";
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal Slprc { get; set; }
    public int ArrivalDays { get; set; }
    public string OrdDay1 { get; set; } = "Y";
    public string OrdDay2 { get; set; } = "Y";
    public string OrdDay3 { get; set; } = "Y";
    public string OrdDay4 { get; set; } = "Y";
    public string OrdDay5 { get; set; } = "Y";
    public string OrdDay6 { get; set; } = "Y";
    public string OrdDay7 { get; set; } = "Y";
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 建立供應商商品 DTO
/// </summary>
public class CreateSupplierGoodsDto
{
    public string SupplierId { get; set; } = string.Empty;
    public string BarcodeId { get; set; } = string.Empty;
    public string ShopId { get; set; } = string.Empty;
    public decimal Lprc { get; set; }
    public decimal Mprc { get; set; }
    public string Tax { get; set; } = "1";
    public decimal MinQty { get; set; }
    public decimal MaxQty { get; set; }
    public string? Unit { get; set; }
    public decimal Rate { get; set; } = 1;
    public string Status { get; set; } = "0";
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal Slprc { get; set; }
    public int ArrivalDays { get; set; }
    public string OrdDay1 { get; set; } = "Y";
    public string OrdDay2 { get; set; } = "Y";
    public string OrdDay3 { get; set; } = "Y";
    public string OrdDay4 { get; set; } = "Y";
    public string OrdDay5 { get; set; } = "Y";
    public string OrdDay6 { get; set; } = "Y";
    public string OrdDay7 { get; set; } = "Y";
}

/// <summary>
/// 修改供應商商品 DTO
/// </summary>
public class UpdateSupplierGoodsDto
{
    public decimal Lprc { get; set; }
    public decimal Mprc { get; set; }
    public string Tax { get; set; } = "1";
    public decimal MinQty { get; set; }
    public decimal MaxQty { get; set; }
    public string? Unit { get; set; }
    public decimal Rate { get; set; } = 1;
    public string Status { get; set; } = "0";
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal Slprc { get; set; }
    public int ArrivalDays { get; set; }
    public string OrdDay1 { get; set; } = "Y";
    public string OrdDay2 { get; set; } = "Y";
    public string OrdDay3 { get; set; } = "Y";
    public string OrdDay4 { get; set; } = "Y";
    public string OrdDay5 { get; set; } = "Y";
    public string OrdDay6 { get; set; } = "Y";
    public string OrdDay7 { get; set; } = "Y";
}

/// <summary>
/// 查詢供應商商品 DTO
/// </summary>
public class SupplierGoodsQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? SupplierId { get; set; }
    public string? BarcodeId { get; set; }
    public string? ShopId { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 批次刪除供應商商品 DTO
/// </summary>
public class BatchDeleteSupplierGoodsDto
{
    public List<SupplierGoodsKeyDto> Items { get; set; } = new();
}

/// <summary>
/// 供應商商品主鍵 DTO
/// </summary>
public class SupplierGoodsKeyDto
{
    public string SupplierId { get; set; } = string.Empty;
    public string BarcodeId { get; set; } = string.Empty;
    public string ShopId { get; set; } = string.Empty;
}

/// <summary>
/// 更新狀態 DTO
/// </summary>
public class UpdateSupplierGoodsStatusDto
{
    public string Status { get; set; } = "0";
}

