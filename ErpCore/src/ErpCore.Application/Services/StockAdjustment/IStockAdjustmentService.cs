using ErpCore.Application.DTOs.StockAdjustment;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.StockAdjustment;

/// <summary>
/// 庫存調整單服務介面
/// </summary>
public interface IStockAdjustmentService
{
    Task<PagedResult<InventoryAdjustmentDto>> GetInventoryAdjustmentsAsync(InventoryAdjustmentQueryDto query);
    Task<InventoryAdjustmentDto> GetInventoryAdjustmentByIdAsync(string adjustmentId);
    Task<string> CreateInventoryAdjustmentAsync(CreateInventoryAdjustmentDto dto);
    Task UpdateInventoryAdjustmentAsync(string adjustmentId, UpdateInventoryAdjustmentDto dto);
    Task DeleteInventoryAdjustmentAsync(string adjustmentId);
    Task ConfirmAdjustmentAsync(string adjustmentId);
    Task CancelAdjustmentAsync(string adjustmentId);
    Task<IEnumerable<AdjustmentReasonDto>> GetAdjustmentReasonsAsync();
}

