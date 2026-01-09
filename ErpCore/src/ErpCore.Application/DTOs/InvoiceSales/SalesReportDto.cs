namespace ErpCore.Application.DTOs.InvoiceSales;

/// <summary>
/// 銷售報表查詢條件 DTO (SYSG610-SYSG640 - 報表查詢作業)
/// </summary>
public class SalesReportQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? OrderId { get; set; }
    public string? OrderType { get; set; }
    public string? ShopId { get; set; }
    public string? CustomerId { get; set; }
    public string? GoodsId { get; set; }
    public string? Status { get; set; }
    public DateTime? OrderDateFrom { get; set; }
    public DateTime? OrderDateTo { get; set; }
}

/// <summary>
/// 銷售報表明細 DTO
/// </summary>
public class SalesReportDetailDto
{
    public string OrderId { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string OrderType { get; set; } = string.Empty;
    public string ShopId { get; set; } = string.Empty;
    public string? ShopName { get; set; }
    public string? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string GoodsId { get; set; } = string.Empty;
    public string? GoodsName { get; set; }
    public decimal OrderQty { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? Amount { get; set; }
    public decimal? ShippedQty { get; set; }
    public decimal? ReturnQty { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? CurrencyId { get; set; }
    public decimal? ExchangeRate { get; set; }
    public string? ApplyUserId { get; set; }
    public string? ApplyUserName { get; set; }
    public DateTime? ApplyDate { get; set; }
    public string? ApproveUserId { get; set; }
    public string? ApproveUserName { get; set; }
    public DateTime? ApproveDate { get; set; }
}

/// <summary>
/// 銷售報表彙總 DTO
/// </summary>
public class SalesReportSummaryDto
{
    public string ShopId { get; set; } = string.Empty;
    public string? ShopName { get; set; }
    public string? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string GoodsId { get; set; } = string.Empty;
    public string? GoodsName { get; set; }
    public int OrderCount { get; set; }
    public decimal TotalQty { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AvgUnitPrice { get; set; }
    public decimal? ShippedQty { get; set; }
    public decimal? ReturnQty { get; set; }
}

