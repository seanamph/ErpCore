using ErpCore.Application.DTOs.Transfer;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Transfer;

/// <summary>
/// 調撥短溢單服務介面
/// </summary>
public interface ITransferShortageService
{
    Task<PagedResult<TransferShortageDto>> GetTransferShortagesAsync(TransferShortageQueryDto query);
    Task<TransferShortageDto> GetTransferShortageByIdAsync(string shortageId);
    Task<TransferShortageDto> CreateShortageFromTransferAsync(string transferId);
    Task<string> CreateTransferShortageAsync(CreateTransferShortageDto dto);
    Task UpdateTransferShortageAsync(string shortageId, UpdateTransferShortageDto dto);
    Task DeleteTransferShortageAsync(string shortageId);
    Task ApproveTransferShortageAsync(string shortageId, ApproveTransferShortageDto dto);
    Task ProcessTransferShortageAsync(string shortageId, ProcessTransferShortageDto dto);
}
