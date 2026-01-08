using ErpCore.Domain.Entities.Sales;

namespace ErpCore.Infrastructure.Repositories.Sales;

/// <summary>
/// 銷售單明細 Repository 介面 (SYSD110-SYSD140)
/// </summary>
public interface ISalesOrderDetailRepository
{
    Task<IEnumerable<SalesOrderDetail>> GetByOrderIdAsync(string orderId);
    Task<SalesOrderDetail?> GetByOrderIdAndLineNumAsync(string orderId, int lineNum);
    Task<SalesOrderDetail> CreateAsync(SalesOrderDetail detail);
    Task<SalesOrderDetail> UpdateAsync(SalesOrderDetail detail);
    Task DeleteAsync(string orderId, int lineNum);
    Task DeleteByOrderIdAsync(string orderId);
}

