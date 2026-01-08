using ErpCore.Application.DTOs.Transfer;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Transfer;

/// <summary>
/// 調撥驗收單服務介面
/// </summary>
public interface ITransferReceiptService
{
    Task<PagedResult<PendingTransferOrderDto>> GetPendingOrdersAsync(PendingTransferOrderQueryDto query);
    Task<PagedResult<TransferReceiptDto>> GetTransferReceiptsAsync(TransferReceiptQueryDto query);
    Task<TransferReceiptDto> GetTransferReceiptByIdAsync(string receiptId);
    Task<TransferReceiptDto> CreateReceiptFromOrderAsync(string transferId);
    Task<string> CreateTransferReceiptAsync(CreateTransferReceiptDto dto);
    Task UpdateTransferReceiptAsync(string receiptId, UpdateTransferReceiptDto dto);
    Task DeleteTransferReceiptAsync(string receiptId);
    Task ConfirmReceiptAsync(string receiptId);
    Task CancelTransferReceiptAsync(string receiptId);
}

