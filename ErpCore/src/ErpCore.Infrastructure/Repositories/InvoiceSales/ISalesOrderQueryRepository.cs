using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.InvoiceSales;

/// <summary>
/// 銷售查詢 Repository 接口 (SYSG510-SYSG5D0 - 銷售查詢作業)
/// </summary>
public interface ISalesOrderQueryRepository
{
    /// <summary>
    /// 查詢銷售單列表（含關聯資料）
    /// </summary>
    Task<PagedResult<SalesOrderQueryResult>> QueryAsync(SalesOrderQuery query);

    /// <summary>
    /// 查詢銷售單統計資料
    /// </summary>
    Task<SalesOrderStatisticsResult> GetStatisticsAsync(SalesOrderStatisticsQuery query);
}

/// <summary>
/// 銷售單查詢結果
/// </summary>
public class SalesOrderQueryResult
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
/// 銷售單查詢參數
/// </summary>
public class SalesOrderQuery
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
/// 銷售單統計查詢參數
/// </summary>
public class SalesOrderStatisticsQuery
{
    public string? ShopId { get; set; }
    public string? OrderType { get; set; }
    public string? Status { get; set; }
    public DateTime? OrderDateFrom { get; set; }
    public DateTime? OrderDateTo { get; set; }
}

/// <summary>
/// 銷售單統計結果
/// </summary>
public class SalesOrderStatisticsResult
{
    public int OrderCount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalQty { get; set; }
    public decimal AvgAmount { get; set; }
    public List<SalesOrderStatisticsByShop> ByShop { get; set; } = new();
    public List<SalesOrderStatisticsByStatus> ByStatus { get; set; } = new();
}

/// <summary>
/// 按分店統計
/// </summary>
public class SalesOrderStatisticsByShop
{
    public string ShopId { get; set; } = string.Empty;
    public string? ShopName { get; set; }
    public int OrderCount { get; set; }
    public decimal TotalAmount { get; set; }
}

/// <summary>
/// 按狀態統計
/// </summary>
public class SalesOrderStatisticsByStatus
{
    public string Status { get; set; } = string.Empty;
    public string? StatusName { get; set; }
    public int OrderCount { get; set; }
    public decimal TotalAmount { get; set; }
}

