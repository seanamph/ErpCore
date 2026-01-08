using ErpCore.Domain.Entities.Query;
using ErpCore.Application.DTOs.Query;

namespace ErpCore.Infrastructure.Repositories.Query;

/// <summary>
/// 零用金盤點檔 Repository 接口 (SYSQ241, SYSQ242)
/// </summary>
public interface IPcCashInventoryRepository
{
    Task<PcCashInventory?> GetByIdAsync(long tKey);
    Task<PcCashInventory?> GetByInventoryIdAsync(string inventoryId);
    Task<PagedResult<PcCashInventoryDto>> QueryAsync(PcCashInventoryQueryDto query);
    Task<PcCashInventory> CreateAsync(PcCashInventory entity);
    Task<PcCashInventory> UpdateAsync(PcCashInventory entity);
    Task DeleteAsync(long tKey);
    Task<string> GenerateInventoryIdAsync(string? siteId);
}

