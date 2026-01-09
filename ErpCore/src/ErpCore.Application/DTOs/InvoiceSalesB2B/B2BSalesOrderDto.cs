namespace ErpCore.Application.DTOs.InvoiceSalesB2B;

/// <summary>
/// B2B銷售單 DTO (SYSG000_B2B - B2B銷售資料維護)
/// </summary>
public class B2BSalesOrderDto
{
    public long TKey { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string OrderType { get; set; } = string.Empty;
    public string ShopId { get; set; } = string.Empty;
    public string? ShopName { get; set; }
    public string? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string Status { get; set; } = "D";
    public string? ApplyUserId { get; set; }
    public string? ApplyUserName { get; set; }
    public DateTime? ApplyDate { get; set; }
    public string? ApproveUserId { get; set; }
    public string? ApproveUserName { get; set; }
    public DateTime? ApproveDate { get; set; }
    public decimal TotalAmount { get; set; } = 0;
    public decimal TotalQty { get; set; } = 0;
    public string? Memo { get; set; }
    public DateTime? ExpectedDate { get; set; }
    public string? SiteId { get; set; }
    public string? OrgId { get; set; }
    public string CurrencyId { get; set; } = "TWD";
    public decimal ExchangeRate { get; set; } = 1;
    public string B2BFlag { get; set; } = "Y";
    public string? TransferType { get; set; }
    public string? TransferStatus { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<B2BSalesOrderDetailDto>? Details { get; set; }
}

/// <summary>
/// B2B銷售單明細 DTO
/// </summary>
public class B2BSalesOrderDetailDto
{
    public long TKey { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public int LineNum { get; set; }
    public string GoodsId { get; set; } = string.Empty;
    public string? GoodsName { get; set; }
    public string? BarcodeId { get; set; }
    public decimal OrderQty { get; set; } = 0;
    public decimal? UnitPrice { get; set; }
    public decimal? Amount { get; set; }
    public decimal ShippedQty { get; set; } = 0;
    public decimal ReturnQty { get; set; } = 0;
    public string? UnitId { get; set; }
    public decimal TaxRate { get; set; } = 0;
    public decimal TaxAmount { get; set; } = 0;
    public string? Memo { get; set; }
}

/// <summary>
/// 建立B2B銷售單 DTO
/// </summary>
public class CreateB2BSalesOrderDto
{
    public string OrderId { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string OrderType { get; set; } = string.Empty;
    public string ShopId { get; set; } = string.Empty;
    public string? CustomerId { get; set; }
    public string Status { get; set; } = "D";
    public DateTime? ExpectedDate { get; set; }
    public string? Memo { get; set; }
    public string? SiteId { get; set; }
    public string? OrgId { get; set; }
    public string CurrencyId { get; set; } = "TWD";
    public decimal ExchangeRate { get; set; } = 1;
    public string? TransferType { get; set; }
    public List<CreateB2BSalesOrderDetailDto> Details { get; set; } = new();
}

/// <summary>
/// 建立B2B銷售單明細 DTO
/// </summary>
public class CreateB2BSalesOrderDetailDto
{
    public int LineNum { get; set; }
    public string GoodsId { get; set; } = string.Empty;
    public string? BarcodeId { get; set; }
    public decimal OrderQty { get; set; } = 0;
    public decimal? UnitPrice { get; set; }
    public string? UnitId { get; set; }
    public decimal TaxRate { get; set; } = 0;
    public string? Memo { get; set; }
}

/// <summary>
/// 修改B2B銷售單 DTO
/// </summary>
public class UpdateB2BSalesOrderDto
{
    public long TKey { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string OrderType { get; set; } = string.Empty;
    public string ShopId { get; set; } = string.Empty;
    public string? CustomerId { get; set; }
    public string Status { get; set; } = "D";
    public DateTime? ExpectedDate { get; set; }
    public string? Memo { get; set; }
    public string? SiteId { get; set; }
    public string? OrgId { get; set; }
    public string CurrencyId { get; set; } = "TWD";
    public decimal ExchangeRate { get; set; } = 1;
    public string? TransferType { get; set; }
    public List<UpdateB2BSalesOrderDetailDto> Details { get; set; } = new();
}

/// <summary>
/// 修改B2B銷售單明細 DTO
/// </summary>
public class UpdateB2BSalesOrderDetailDto
{
    public long? TKey { get; set; }
    public int LineNum { get; set; }
    public string GoodsId { get; set; } = string.Empty;
    public string? BarcodeId { get; set; }
    public decimal OrderQty { get; set; } = 0;
    public decimal? UnitPrice { get; set; }
    public string? UnitId { get; set; }
    public decimal TaxRate { get; set; } = 0;
    public string? Memo { get; set; }
}

/// <summary>
/// B2B銷售單查詢 DTO
/// </summary>
public class B2BSalesOrderQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? OrderId { get; set; }
    public string? OrderType { get; set; }
    public string? ShopId { get; set; }
    public string? CustomerId { get; set; }
    public string? Status { get; set; }
    public DateTime? OrderDateFrom { get; set; }
    public DateTime? OrderDateTo { get; set; }
    public string? B2BFlag { get; set; } = "Y";
}

