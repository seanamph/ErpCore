using ErpCore.Application.DTOs.Lease;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Lease;

/// <summary>
/// 費用項目主檔服務介面 (SYSE310-SYSE430)
/// </summary>
public interface ILeaseFeeItemService
{
    Task<PagedResult<LeaseFeeItemDto>> GetLeaseFeeItemsAsync(LeaseFeeItemQueryDto query);
    Task<LeaseFeeItemDto> GetLeaseFeeItemByIdAsync(string feeItemId);
    Task<LeaseFeeItemDto> CreateLeaseFeeItemAsync(CreateLeaseFeeItemDto dto);
    Task UpdateLeaseFeeItemAsync(string feeItemId, UpdateLeaseFeeItemDto dto);
    Task DeleteLeaseFeeItemAsync(string feeItemId);
    Task UpdateLeaseFeeItemStatusAsync(string feeItemId, string status);
    Task<bool> ExistsAsync(string feeItemId);
}

