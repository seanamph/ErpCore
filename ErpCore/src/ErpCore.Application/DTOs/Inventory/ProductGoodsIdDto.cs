namespace ErpCore.Application.DTOs.Inventory;

/// <summary>
/// 商品進銷碼 DTO
/// </summary>
public class ProductGoodsIdDto
{
    public string GoodsId { get; set; } = string.Empty;
    public string GoodsName { get; set; } = string.Empty;
    public string? InvPrintName { get; set; }
    public string? GoodsSpace { get; set; }
    public string? ScId { get; set; }
    public string? ScName { get; set; }
    public string Tax { get; set; } = "1";
    public string? TaxName { get; set; }
    public decimal Lprc { get; set; }
    public decimal Mprc { get; set; }
    public string? BarcodeId { get; set; }
    public string? Unit { get; set; }
    public int ConvertRate { get; set; } = 1;
    public int Capacity { get; set; }
    public string? CapacityUnit { get; set; }
    public string Status { get; set; } = "1";
    public string? StatusName { get; set; }
    public string Discount { get; set; } = "N";
    public string AutoOrder { get; set; } = "N";
    public string PriceKind { get; set; } = "1";
    public string CostKind { get; set; } = "1";
    public int SafeDays { get; set; }
    public int ExpirationDays { get; set; }
    public string? National { get; set; }
    public string? Place { get; set; }
    public int GoodsDeep { get; set; }
    public int GoodsWide { get; set; }
    public int GoodsHigh { get; set; }
    public int PackDeep { get; set; }
    public int PackWide { get; set; }
    public int PackHigh { get; set; }
    public int PackWeight { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 建立商品進銷碼 DTO
/// </summary>
public class CreateProductGoodsIdDto
{
    public string GoodsId { get; set; } = string.Empty;
    public string GoodsName { get; set; } = string.Empty;
    public string? InvPrintName { get; set; }
    public string? GoodsSpace { get; set; }
    public string? ScId { get; set; }
    public string Tax { get; set; } = "1";
    public decimal Lprc { get; set; }
    public decimal Mprc { get; set; }
    public string? BarcodeId { get; set; }
    public string? Unit { get; set; }
    public int ConvertRate { get; set; } = 1;
    public int Capacity { get; set; }
    public string? CapacityUnit { get; set; }
    public string Status { get; set; } = "1";
    public string Discount { get; set; } = "N";
    public string AutoOrder { get; set; } = "N";
    public string PriceKind { get; set; } = "1";
    public string CostKind { get; set; } = "1";
    public int SafeDays { get; set; }
    public int ExpirationDays { get; set; }
    public string? National { get; set; }
    public string? Place { get; set; }
    public int GoodsDeep { get; set; }
    public int GoodsWide { get; set; }
    public int GoodsHigh { get; set; }
    public int PackDeep { get; set; }
    public int PackWide { get; set; }
    public int PackHigh { get; set; }
    public int PackWeight { get; set; }
}

/// <summary>
/// 修改商品進銷碼 DTO
/// </summary>
public class UpdateProductGoodsIdDto
{
    public string GoodsName { get; set; } = string.Empty;
    public string? InvPrintName { get; set; }
    public string? GoodsSpace { get; set; }
    public string? ScId { get; set; }
    public string Tax { get; set; } = "1";
    public decimal Lprc { get; set; }
    public decimal Mprc { get; set; }
    public string? BarcodeId { get; set; }
    public string? Unit { get; set; }
    public int ConvertRate { get; set; } = 1;
    public int Capacity { get; set; }
    public string? CapacityUnit { get; set; }
    public string Status { get; set; } = "1";
    public string Discount { get; set; } = "N";
    public string AutoOrder { get; set; } = "N";
    public string PriceKind { get; set; } = "1";
    public string CostKind { get; set; } = "1";
    public int SafeDays { get; set; }
    public int ExpirationDays { get; set; }
    public string? National { get; set; }
    public string? Place { get; set; }
    public int GoodsDeep { get; set; }
    public int GoodsWide { get; set; }
    public int GoodsHigh { get; set; }
    public int PackDeep { get; set; }
    public int PackWide { get; set; }
    public int PackHigh { get; set; }
    public int PackWeight { get; set; }
}

/// <summary>
/// 查詢商品進銷碼 DTO
/// </summary>
public class ProductGoodsIdQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? GoodsId { get; set; }
    public string? GoodsName { get; set; }
    public string? BarcodeId { get; set; }
    public string? ScId { get; set; }
    public string? Status { get; set; }
    public string? ShopId { get; set; }
}

/// <summary>
/// 批次刪除商品進銷碼 DTO
/// </summary>
public class BatchDeleteProductGoodsIdDto
{
    public List<string> GoodsIds { get; set; } = new();
}

/// <summary>
/// 更新狀態 DTO
/// </summary>
public class UpdateProductGoodsIdStatusDto
{
    public string Status { get; set; } = "1";
}

