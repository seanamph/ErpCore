using ErpCore.Application.DTOs.Query;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Query;

/// <summary>
/// 零用金盤點檔服務接口 (SYSQ241, SYSQ242)
/// </summary>
public interface IPcCashInventoryService
{
    Task<PagedResult<PcCashInventoryDto>> QueryAsync(PcCashInventoryQueryDto query);
    Task<PcCashInventoryDto> GetByIdAsync(long tKey);
    Task<PcCashInventoryDto> GetByInventoryIdAsync(string inventoryId);
    Task<PcCashInventoryDto> CreateAsync(CreatePcCashInventoryDto dto);
    Task<PcCashInventoryDto> UpdateAsync(long tKey, UpdatePcCashInventoryDto dto);
    Task DeleteAsync(long tKey);
}

