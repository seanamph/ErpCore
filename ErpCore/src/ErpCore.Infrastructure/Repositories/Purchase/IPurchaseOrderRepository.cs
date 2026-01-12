using ErpCore.Domain.Entities.Purchase;

namespace ErpCore.Infrastructure.Repositories.Purchase;

/// <summary>
/// 採購單 Repository 介面
/// </summary>
public interface IPurchaseOrderRepository
{
    Task<PurchaseOrder?> GetByIdAsync(string orderId);
    Task<PurchaseOrderDetail?> GetDetailByIdAsync(Guid detailId);
    Task<IEnumerable<PurchaseOrderDetail>> GetDetailsByOrderIdAsync(string orderId);
    Task<IEnumerable<PurchaseOrder>> QueryAsync(PurchaseOrderQuery query);
    Task<int> GetCountAsync(PurchaseOrderQuery query);
    Task<string> CreateAsync(PurchaseOrder entity, List<PurchaseOrderDetail> details);
    Task UpdateAsync(PurchaseOrder entity, List<PurchaseOrderDetail> details);
    Task DeleteAsync(string orderId);
    Task UpdateStatusAsync(string orderId, string status, string? userId = null, System.Data.IDbTransaction? transaction = null);
    Task<string> GenerateOrderIdAsync();
    Task UpdateReceiptQtyAsync(Guid orderDetailId, decimal receiptQty, System.Data.IDbTransaction? transaction = null);
}

/// <summary>
/// 查詢條件
/// </summary>
public class PurchaseOrderQuery
{
    public string? OrderId { get; set; }
    public string? OrderType { get; set; }
    public string? ShopId { get; set; }
    public string? SupplierId { get; set; }
    public string? Status { get; set; }
    public DateTime? OrderDateFrom { get; set; }
    public DateTime? OrderDateTo { get; set; }
    public string? SourceProgram { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

