using ErpCore.Application.DTOs.Query;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Query;

/// <summary>
/// 零用金拋轉檔服務接口 (SYSQ230)
/// </summary>
public interface IPcCashTransferService
{
    Task<PagedResult<PcCashTransferDto>> QueryAsync(PcCashTransferQueryDto query);
    Task<PcCashTransferDto> GetByIdAsync(long tKey);
    Task<PcCashTransferDto> GetByTransferIdAsync(string transferId);
    Task<PcCashTransferDto> CreateAsync(CreatePcCashTransferDto dto);
    Task<PcCashTransferDto> UpdateAsync(long tKey, UpdatePcCashTransferDto dto);
    Task DeleteAsync(long tKey);
}

