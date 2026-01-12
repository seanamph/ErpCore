using ErpCore.Application.DTOs.Transfer;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Transfer;

/// <summary>
/// 調撥驗退單服務介面
/// </summary>
public interface ITransferReturnService
{
    Task<PagedResult<PendingTransferOrderForReturnDto>> GetPendingTransfersAsync(PendingTransferOrderForReturnQueryDto query);
    Task<PagedResult<TransferReturnDto>> GetTransferReturnsAsync(TransferReturnQueryDto query);
    Task<TransferReturnDto> GetTransferReturnByIdAsync(string returnId);
    Task<TransferReturnDto> CreateReturnFromTransferAsync(string transferId);
    Task<string> CreateTransferReturnAsync(CreateTransferReturnDto dto);
    Task UpdateTransferReturnAsync(string returnId, UpdateTransferReturnDto dto);
    Task DeleteTransferReturnAsync(string returnId);
    Task ConfirmReturnAsync(string returnId);
    Task CancelTransferReturnAsync(string returnId);
}
