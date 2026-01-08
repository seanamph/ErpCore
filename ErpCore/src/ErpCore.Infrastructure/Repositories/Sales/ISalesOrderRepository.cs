using ErpCore.Domain.Entities.Sales;

namespace ErpCore.Infrastructure.Repositories.Sales;

/// <summary>
/// 銷售單 Repository 介面 (SYSD110-SYSD140)
/// </summary>
public interface ISalesOrderRepository
{
    Task<SalesOrder?> GetByIdAsync(string orderId);
    Task<IEnumerable<SalesOrder>> QueryAsync(SalesOrderQuery query);
    Task<int> GetCountAsync(SalesOrderQuery query);
    Task<bool> ExistsAsync(string orderId);
    Task<SalesOrder> CreateAsync(SalesOrder salesOrder);
    Task<SalesOrder> UpdateAsync(SalesOrder salesOrder);
    Task DeleteAsync(string orderId);
}

/// <summary>
/// 銷售單查詢條件
/// </summary>
public class SalesOrderQuery
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

