using System.Data;
using ErpCore.Domain.Entities.Transfer;

namespace ErpCore.Infrastructure.Repositories.Transfer;

/// <summary>
/// 調撥驗收單 Repository 介面
/// </summary>
public interface ITransferReceiptRepository
{
    Task<TransferReceipt?> GetByIdAsync(string receiptId);
    Task<TransferReceiptDetail?> GetDetailByIdAsync(Guid detailId);
    Task<IEnumerable<TransferReceiptDetail>> GetDetailsByReceiptIdAsync(string receiptId);
    Task<IEnumerable<TransferReceipt>> QueryAsync(TransferReceiptQuery query);
    Task<int> GetCountAsync(TransferReceiptQuery query);
    Task<string> CreateAsync(TransferReceipt entity, List<TransferReceiptDetail> details);
    Task UpdateAsync(TransferReceipt entity, List<TransferReceiptDetail> details);
    Task DeleteAsync(string receiptId);
    Task<IEnumerable<PendingTransferOrderForReceipt>> GetPendingOrdersAsync(PendingTransferOrderQuery query);
    Task<int> GetPendingOrdersCountAsync(PendingTransferOrderQuery query);
    Task UpdateStatusAsync(string receiptId, string status, global::System.Data.IDbTransaction? transaction = null);
    Task<string> GenerateReceiptIdAsync();
}

/// <summary>
/// 查詢條件
/// </summary>
public class TransferReceiptQuery
{
    public string? ReceiptId { get; set; }
    public string? TransferId { get; set; }
    public string? FromShopId { get; set; }
    public string? ToShopId { get; set; }
    public string? Status { get; set; }
    public DateTime? ReceiptDateFrom { get; set; }
    public DateTime? ReceiptDateTo { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// 待驗收調撥單查詢條件
/// </summary>
public class PendingTransferOrderQuery
{
    public string? TransferId { get; set; }
    public string? FromShopId { get; set; }
    public string? ToShopId { get; set; }
    public DateTime? TransferDateFrom { get; set; }
    public DateTime? TransferDateTo { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// 待驗收調撥單
/// </summary>
public class PendingTransferOrderForReceipt
{
    public string TransferId { get; set; } = string.Empty;
    public DateTime TransferDate { get; set; }
    public string FromShopId { get; set; } = string.Empty;
    public string ToShopId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal TotalQty { get; set; }
    public decimal ReceiptQty { get; set; }
    public decimal PendingReceiptQty { get; set; }
}

