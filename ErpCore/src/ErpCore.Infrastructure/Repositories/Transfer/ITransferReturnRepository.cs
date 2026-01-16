using System.Data;
using ErpCore.Domain.Entities.Transfer;

namespace ErpCore.Infrastructure.Repositories.Transfer;

/// <summary>
/// 調撥驗退單 Repository 介面
/// </summary>
public interface ITransferReturnRepository
{
    Task<TransferReturn?> GetByIdAsync(string returnId);
    Task<TransferReturnDetail?> GetDetailByIdAsync(Guid detailId);
    Task<IEnumerable<TransferReturnDetail>> GetDetailsByReturnIdAsync(string returnId);
    Task<IEnumerable<TransferReturn>> QueryAsync(TransferReturnQuery query);
    Task<int> GetCountAsync(TransferReturnQuery query);
    Task<string> CreateAsync(TransferReturn entity, List<TransferReturnDetail> details);
    Task UpdateAsync(TransferReturn entity, List<TransferReturnDetail> details);
    Task DeleteAsync(string returnId);
    Task<IEnumerable<PendingTransferOrderForReturn>> GetPendingOrdersAsync(PendingTransferOrderForReturnQuery query);
    Task<int> GetPendingOrdersCountAsync(PendingTransferOrderForReturnQuery query);
    Task UpdateStatusAsync(string returnId, string status, IDbTransaction? transaction = null);
    Task<string> GenerateReturnIdAsync();
}

/// <summary>
/// 查詢條件
/// </summary>
public class TransferReturnQuery
{
    public string? ReturnId { get; set; }
    public string? TransferId { get; set; }
    public string? ReceiptId { get; set; }
    public string? FromShopId { get; set; }
    public string? ToShopId { get; set; }
    public string? Status { get; set; }
    public DateTime? ReturnDateFrom { get; set; }
    public DateTime? ReturnDateTo { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// 待驗退調撥單查詢條件
/// </summary>
public class PendingTransferOrderForReturnQuery
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
/// 待驗退調撥單
/// </summary>
public class PendingTransferOrderForReturn
{
    public string TransferId { get; set; } = string.Empty;
    public DateTime TransferDate { get; set; }
    public string FromShopId { get; set; } = string.Empty;
    public string ToShopId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal TotalQty { get; set; }
    public decimal ReceiptQty { get; set; }
    public decimal ReturnQty { get; set; }
    public decimal PendingReturnQty { get; set; }
}
