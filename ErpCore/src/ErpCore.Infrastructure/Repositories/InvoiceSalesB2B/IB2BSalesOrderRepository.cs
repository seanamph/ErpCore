using ErpCore.Domain.Entities.InvoiceSalesB2B;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.InvoiceSalesB2B;

/// <summary>
/// B2B銷售單 Repository 接口 (SYSG000_B2B - B2B銷售資料維護)
/// </summary>
public interface IB2BSalesOrderRepository
{
    Task<B2BSalesOrder?> GetByIdAsync(long tKey);
    Task<B2BSalesOrder?> GetByOrderIdAsync(string orderId);
    Task<PagedResult<B2BSalesOrder>> QueryAsync(B2BSalesOrderQuery query);
    Task<long> CreateAsync(B2BSalesOrder salesOrder);
    Task<int> UpdateAsync(B2BSalesOrder salesOrder);
    Task<int> DeleteAsync(long tKey);
    Task<bool> ExistsByOrderIdAsync(string orderId, long? excludeTKey = null);
}

/// <summary>
/// B2B銷售單明細 Repository 接口
/// </summary>
public interface IB2BSalesOrderDetailRepository
{
    Task<List<B2BSalesOrderDetail>> GetByOrderIdAsync(string orderId);
    Task<long> CreateAsync(B2BSalesOrderDetail detail);
    Task<int> UpdateAsync(B2BSalesOrderDetail detail);
    Task<int> DeleteAsync(long tKey);
    Task<int> DeleteByOrderIdAsync(string orderId);
}

/// <summary>
/// B2B銷售單查詢參數
/// </summary>
public class B2BSalesOrderQuery
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

