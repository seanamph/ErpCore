using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.InvoiceSalesB2B;

/// <summary>
/// B2B銷售查詢 Repository 接口 (SYSG000_B2B - B2B銷售查詢作業)
/// </summary>
public interface IB2BSalesOrderQueryRepository
{
    /// <summary>
    /// 查詢B2B銷售單列表
    /// </summary>
    Task<PagedResult<B2BSalesOrderQueryResult>> QueryAsync(B2BSalesOrderQuery query);

    /// <summary>
    /// 查詢B2B銷售單統計
    /// </summary>
    Task<B2BSalesOrderStatisticsResult> GetStatisticsAsync(B2BSalesOrderStatisticsQuery query);
}

/// <summary>
/// B2B銷售單查詢結果
/// </summary>
public class B2BSalesOrderQueryResult
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
/// B2B銷售單統計結果
/// </summary>
public class B2BSalesOrderStatisticsResult
{
    public int OrderCount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalQty { get; set; }
    public decimal AvgAmount { get; set; }
    public List<B2BSalesOrderStatisticsByShop> ByShop { get; set; } = new();
    public List<B2BSalesOrderStatisticsByStatus> ByStatus { get; set; } = new();
}

/// <summary>
/// B2B銷售單按分店統計
/// </summary>
public class B2BSalesOrderStatisticsByShop
{
    public string ShopId { get; set; } = string.Empty;
    public string? ShopName { get; set; }
    public int OrderCount { get; set; }
    public decimal TotalAmount { get; set; }
}

/// <summary>
/// B2B銷售單按狀態統計
/// </summary>
public class B2BSalesOrderStatisticsByStatus
{
    public string Status { get; set; } = string.Empty;
    public string StatusName { get; set; } = string.Empty;
    public int OrderCount { get; set; }
    public decimal TotalAmount { get; set; }
}

/// <summary>
/// B2B銷售單統計查詢參數
/// </summary>
public class B2BSalesOrderStatisticsQuery
{
    public string? ShopId { get; set; }
    public string? OrderType { get; set; }
    public string? Status { get; set; }
    public DateTime? OrderDateFrom { get; set; }
    public DateTime? OrderDateTo { get; set; }
    public string? B2BFlag { get; set; } = "Y";
}

