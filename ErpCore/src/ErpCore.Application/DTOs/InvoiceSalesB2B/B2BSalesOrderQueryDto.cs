namespace ErpCore.Application.DTOs.InvoiceSalesB2B;

/// <summary>
/// B2B銷售單查詢 DTO (SYSG000_B2B - B2B銷售查詢作業)
/// </summary>
public class B2BSalesOrderQueryDto
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
    public decimal TotalAmount { get; set; } = 0;
    public decimal TotalQty { get; set; } = 0;
    public string? ApplyUserId { get; set; }
    public string? ApplyUserName { get; set; }
    public DateTime? ApplyDate { get; set; }
    public string? ApproveUserId { get; set; }
    public string? ApproveUserName { get; set; }
    public DateTime? ApproveDate { get; set; }
    public string? TransferType { get; set; }
    public string? TransferStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// B2B銷售單查詢條件 DTO
/// </summary>
public class B2BSalesOrderQueryConditionDto
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

/// <summary>
/// B2B銷售單統計 DTO
/// </summary>
public class B2BSalesOrderStatisticsDto
{
    public int OrderCount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalQty { get; set; }
    public decimal AvgAmount { get; set; }
    public List<B2BSalesOrderStatisticsByShopDto> ByShop { get; set; } = new();
    public List<B2BSalesOrderStatisticsByStatusDto> ByStatus { get; set; } = new();
}

/// <summary>
/// B2B銷售單按分店統計 DTO
/// </summary>
public class B2BSalesOrderStatisticsByShopDto
{
    public string ShopId { get; set; } = string.Empty;
    public string? ShopName { get; set; }
    public int OrderCount { get; set; }
    public decimal TotalAmount { get; set; }
}

/// <summary>
/// B2B銷售單按狀態統計 DTO
/// </summary>
public class B2BSalesOrderStatisticsByStatusDto
{
    public string Status { get; set; } = string.Empty;
    public string StatusName { get; set; } = string.Empty;
    public int OrderCount { get; set; }
    public decimal TotalAmount { get; set; }
}

/// <summary>
/// B2B銷售單統計查詢條件 DTO
/// </summary>
public class B2BSalesOrderStatisticsQueryDto
{
    public string? ShopId { get; set; }
    public string? OrderType { get; set; }
    public string? Status { get; set; }
    public DateTime? OrderDateFrom { get; set; }
    public DateTime? OrderDateTo { get; set; }
    public string? B2BFlag { get; set; } = "Y";
}

