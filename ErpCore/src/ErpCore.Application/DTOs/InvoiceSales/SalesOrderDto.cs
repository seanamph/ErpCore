namespace ErpCore.Application.DTOs.InvoiceSales;

/// <summary>
/// 銷售單 DTO (SYSG410-SYSG460 - 銷售資料維護)
/// </summary>
public class SalesOrderDto
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
    public decimal? TotalAmount { get; set; } = 0;
    public decimal? TotalQty { get; set; } = 0;
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
    public string? CustomerId { get; set; }
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
    public decimal? TaxRate { get; set; } = 0;
    public string? Memo { get; set; }
}

/// <summary>
/// 修改銷售單 DTO
/// </summary>
public class UpdateSalesOrderDto
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
    public string? CurrencyId { get; set; } = "TWD";
    public decimal? ExchangeRate { get; set; } = 1;
    public List<UpdateSalesOrderDetailDto> Details { get; set; } = new();
}

/// <summary>
/// 修改銷售單明細 DTO
/// </summary>
public class UpdateSalesOrderDetailDto
{
    public long? TKey { get; set; }
    public int LineNum { get; set; }
    public string GoodsId { get; set; } = string.Empty;
    public string? BarcodeId { get; set; }
    public decimal OrderQty { get; set; } = 0;
    public decimal? UnitPrice { get; set; }
    public string? UnitId { get; set; }
    public decimal? TaxRate { get; set; } = 0;
    public string? Memo { get; set; }
}

/// <summary>
/// 銷售單查詢 DTO
/// </summary>
public class SalesOrderQueryDto
{
    public long TKey { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string OrderType { get; set; } = string.Empty;
    public string ShopId { get; set; } = string.Empty;
    public string? ShopName { get; set; }
    public string? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal? TotalAmount { get; set; }
    public decimal? TotalQty { get; set; }
    public string? ApplyUserId { get; set; }
    public string? ApplyUserName { get; set; }
    public DateTime? ApplyDate { get; set; }
    public string? ApproveUserId { get; set; }
    public string? ApproveUserName { get; set; }
    public DateTime? ApproveDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 銷售單查詢條件 DTO
/// </summary>
public class SalesOrderQueryConditionDto
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
    public string? ApplyUserId { get; set; }
    public string? ApproveUserId { get; set; }
}

/// <summary>
/// 銷售單統計查詢條件 DTO
/// </summary>
public class SalesOrderStatisticsQueryDto
{
    public string? ShopId { get; set; }
    public string? OrderType { get; set; }
    public string? Status { get; set; }
    public DateTime? OrderDateFrom { get; set; }
    public DateTime? OrderDateTo { get; set; }
}

/// <summary>
/// 銷售單統計結果 DTO
/// </summary>
public class SalesOrderStatisticsDto
{
    public int OrderCount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalQty { get; set; }
    public decimal AvgAmount { get; set; }
    public List<SalesOrderStatisticsByShopDto> ByShop { get; set; } = new();
    public List<SalesOrderStatisticsByStatusDto> ByStatus { get; set; } = new();
}

/// <summary>
/// 按分店統計 DTO
/// </summary>
public class SalesOrderStatisticsByShopDto
{
    public string ShopId { get; set; } = string.Empty;
    public string? ShopName { get; set; }
    public int OrderCount { get; set; }
    public decimal TotalAmount { get; set; }
}

/// <summary>
/// 按狀態統計 DTO
/// </summary>
public class SalesOrderStatisticsByStatusDto
{
    public string Status { get; set; } = string.Empty;
    public string? StatusName { get; set; }
    public int OrderCount { get; set; }
    public decimal TotalAmount { get; set; }
}

