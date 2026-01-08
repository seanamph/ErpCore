namespace ErpCore.Application.DTOs.Sales;

/// <summary>
/// 銷售單 DTO (SYSD110-SYSD140)
/// </summary>
public class SalesOrderDto
{
    public long TKey { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string OrderType { get; set; } = string.Empty;
    public string ShopId { get; set; } = string.Empty;
    public string? ShopName { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public string? CustomerName { get; set; }
    public string Status { get; set; } = "D";
    public string? ApplyUserId { get; set; }
    public DateTime? ApplyDate { get; set; }
    public string? ApproveUserId { get; set; }
    public DateTime? ApproveDate { get; set; }
    public DateTime? ShipDate { get; set; }
    public decimal? TotalAmount { get; set; } = 0;
    public decimal? TotalQty { get; set; } = 0;
    public decimal? DiscountAmount { get; set; } = 0;
    public decimal? TaxAmount { get; set; } = 0;
    public string? Memo { get; set; }
    public DateTime? ExpectedDate { get; set; }
    public string? SiteId { get; set; }
    public string? OrgId { get; set; }
    public string? CurrencyId { get; set; } = "TWD";
    public decimal? ExchangeRate { get; set; } = 1;
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<SalesOrderDetailDto>? Details { get; set; }
}

/// <summary>
/// 銷售單明細 DTO
/// </summary>
public class SalesOrderDetailDto
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
    public decimal? ShippedQty { get; set; } = 0;
    public decimal? ReturnQty { get; set; } = 0;
    public string? UnitId { get; set; }
    public decimal? DiscountRate { get; set; } = 0;
    public decimal? DiscountAmount { get; set; } = 0;
    public decimal? TaxRate { get; set; } = 0;
    public decimal? TaxAmount { get; set; } = 0;
    public string? Memo { get; set; }
}

/// <summary>
/// 建立銷售單 DTO
/// </summary>
public class CreateSalesOrderDto
{
    public string OrderId { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string OrderType { get; set; } = string.Empty;
    public string ShopId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string Status { get; set; } = "D";
    public DateTime? ExpectedDate { get; set; }
    public string? Memo { get; set; }
    public string? SiteId { get; set; }
    public string? OrgId { get; set; }
    public string? CurrencyId { get; set; } = "TWD";
    public decimal? ExchangeRate { get; set; } = 1;
    public List<CreateSalesOrderDetailDto> Details { get; set; } = new();
}

/// <summary>
/// 建立銷售單明細 DTO
/// </summary>
public class CreateSalesOrderDetailDto
{
    public int LineNum { get; set; }
    public string GoodsId { get; set; } = string.Empty;
    public string? BarcodeId { get; set; }
    public decimal OrderQty { get; set; } = 0;
    public decimal? UnitPrice { get; set; }
    public string? UnitId { get; set; }
    public decimal? DiscountRate { get; set; } = 0;
    public decimal? TaxRate { get; set; } = 0;
    public string? Memo { get; set; }
}

/// <summary>
/// 修改銷售單 DTO
/// </summary>
public class UpdateSalesOrderDto
{
    public DateTime OrderDate { get; set; }
    public string OrderType { get; set; } = string.Empty;
    public string ShopId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string Status { get; set; } = "D";
    public DateTime? ExpectedDate { get; set; }
    public string? Memo { get; set; }
    public string? SiteId { get; set; }
    public string? OrgId { get; set; }
    public string? CurrencyId { get; set; } = "TWD";
    public decimal? ExchangeRate { get; set; } = 1;
    public List<UpdateSalesOrderDetailDto> Details { get; set; } = new();
}

/// <summary>
/// 修改銷售單明細 DTO
/// </summary>
public class UpdateSalesOrderDetailDto
{
    public int LineNum { get; set; }
    public string GoodsId { get; set; } = string.Empty;
    public string? BarcodeId { get; set; }
    public decimal OrderQty { get; set; } = 0;
    public decimal? UnitPrice { get; set; }
    public string? UnitId { get; set; }
    public decimal? DiscountRate { get; set; } = 0;
    public decimal? TaxRate { get; set; } = 0;
    public string? Memo { get; set; }
}

/// <summary>
/// 查詢銷售單 DTO
/// </summary>
public class SalesOrderQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? OrderId { get; set; }
    public string? OrderType { get; set; }
    public string? ShopId { get; set; }
    public string? CustomerId { get; set; }
    public string? Status { get; set; }
    public DateTime? OrderDateFrom { get; set; }
    public DateTime? OrderDateTo { get; set; }
}

/// <summary>
/// 審核銷售單 DTO (SYSD210-SYSD230)
/// </summary>
public class ApproveSalesOrderDto
{
    public string ApproveUserId { get; set; } = string.Empty;
    public string? Memo { get; set; }
}

/// <summary>
/// 出貨銷售單 DTO (SYSD210-SYSD230)
/// </summary>
public class ShipSalesOrderDto
{
    public DateTime ShipDate { get; set; }
    public List<ShipSalesOrderDetailDto> Details { get; set; } = new();
    public string? Memo { get; set; }
}

/// <summary>
/// 出貨銷售單明細 DTO
/// </summary>
public class ShipSalesOrderDetailDto
{
    public int LineNum { get; set; }
    public decimal ShippedQty { get; set; } = 0;
}

/// <summary>
/// 取消銷售單 DTO (SYSD210-SYSD230)
/// </summary>
public class CancelSalesOrderDto
{
    public string? Memo { get; set; }
}

/// <summary>
/// 銷售報表查詢 DTO (SYSD310-SYSD430)
/// </summary>
public class SalesReportQueryDto
{
    public string ReportType { get; set; } = string.Empty;
    public DateTime? OrderDateFrom { get; set; }
    public DateTime? OrderDateTo { get; set; }
    public string? OrderType { get; set; }
    public string? ShopId { get; set; }
    public string? CustomerId { get; set; }
    public string? Status { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

