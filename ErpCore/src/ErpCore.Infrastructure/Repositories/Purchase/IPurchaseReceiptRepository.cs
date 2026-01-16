using ErpCore.Domain.Entities.Purchase;

namespace ErpCore.Infrastructure.Repositories.Purchase;

/// <summary>
/// 採購驗收單 Repository 介面
/// </summary>
public interface IPurchaseReceiptRepository
{
    Task<PurchaseReceipt?> GetByIdAsync(string receiptId);
    Task<PurchaseReceiptDetail?> GetDetailByIdAsync(Guid detailId);
    Task<IEnumerable<PurchaseReceiptDetail>> GetDetailsByReceiptIdAsync(string receiptId);
    Task<IEnumerable<PurchaseReceipt>> QueryAsync(PurchaseReceiptQuery query);
    Task<int> GetCountAsync(PurchaseReceiptQuery query);
    Task<string> CreateAsync(PurchaseReceipt entity, List<PurchaseReceiptDetail> details);
    Task UpdateAsync(PurchaseReceipt entity, List<PurchaseReceiptDetail> details);
    Task DeleteAsync(string receiptId);
    Task<IEnumerable<PendingPurchaseOrderForReceipt>> GetPendingOrdersAsync(PendingPurchaseOrderQuery query);
    Task<int> GetPendingOrdersCountAsync(PendingPurchaseOrderQuery query);
    Task UpdateStatusAsync(string receiptId, string status, global::System.Data.IDbTransaction? transaction = null);
    Task<string> GenerateReceiptIdAsync();

    // SYSW333 - 已日結採購單驗收調整作業
    Task<IEnumerable<PurchaseReceipt>> QuerySettledAdjustmentsAsync(PurchaseReceiptQuery query);
    Task<int> GetSettledAdjustmentsCountAsync(PurchaseReceiptQuery query);
    Task<IEnumerable<PurchaseOrder>> GetSettledOrdersAsync(SettledOrderQuery query);
    Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(string orderId);

    // SYSW530 - 已日結退貨單驗退調整作業
    Task<IEnumerable<PurchaseReceipt>> QueryClosedReturnAdjustmentsAsync(PurchaseReceiptQuery query);
    Task<int> GetClosedReturnAdjustmentsCountAsync(PurchaseReceiptQuery query);
    Task<IEnumerable<PurchaseOrder>> GetClosedReturnOrdersAsync(ClosedReturnOrderQuery query);
}

/// <summary>
/// 查詢條件
/// </summary>
public class PurchaseReceiptQuery
{
    public string? ReceiptId { get; set; }
    public string? OrderId { get; set; }
    public string? ShopId { get; set; }
    public string? SupplierId { get; set; }
    public string? Status { get; set; }
    public DateTime? ReceiptDateFrom { get; set; }
    public DateTime? ReceiptDateTo { get; set; }
    public string? SourceProgram { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// 待驗收採購單查詢條件
/// </summary>
public class PendingPurchaseOrderQuery
{
    public string? OrderId { get; set; }
    public string? ShopId { get; set; }
    public string? SupplierId { get; set; }
    public DateTime? OrderDateFrom { get; set; }
    public DateTime? OrderDateTo { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// 待驗收採購單
/// </summary>
public class PendingPurchaseOrderForReceipt
{
    public string OrderId { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string ShopId { get; set; } = string.Empty;
    public string SupplierId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal TotalQty { get; set; }
    public decimal ReceiptQty { get; set; }
    public decimal PendingReceiptQty { get; set; }
}

/// <summary>
/// 已日結採購單查詢條件 (SYSW333)
/// </summary>
public class SettledOrderQuery
{
    public string? ShopId { get; set; }
    public string? SupplierId { get; set; }
    public DateTime? OrderDateFrom { get; set; }
    public DateTime? OrderDateTo { get; set; }
}

/// <summary>
/// 已日結退貨單查詢條件 (SYSW530)
/// </summary>
public class ClosedReturnOrderQuery
{
    public string? SupplierId { get; set; }
    public string? WarehouseId { get; set; }
    public string? ShopId { get; set; }
    public string? Status { get; set; }
}

