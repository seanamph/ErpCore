using ErpCore.Application.DTOs.Purchase;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Purchase;

/// <summary>
/// 採購驗收單服務介面
/// </summary>
public interface IPurchaseReceiptService
{
    Task<PagedResult<PendingPurchaseOrderDto>> GetPendingOrdersAsync(PendingPurchaseOrderQueryDto query);
    Task<PagedResult<PurchaseReceiptDto>> GetPurchaseReceiptsAsync(PurchaseReceiptQueryDto query);
    Task<PurchaseReceiptFullDto> GetPurchaseReceiptByIdAsync(string receiptId);
    Task<PurchaseReceiptFullDto> CreateReceiptFromOrderAsync(string orderId);
    Task<string> CreatePurchaseReceiptAsync(CreatePurchaseReceiptDto dto);
    Task UpdatePurchaseReceiptAsync(string receiptId, UpdatePurchaseReceiptDto dto);
    Task DeletePurchaseReceiptAsync(string receiptId);
    Task ConfirmReceiptAsync(string receiptId);
    Task CancelPurchaseReceiptAsync(string receiptId);

    // SYSW333 - 已日結採購單驗收調整作業
    Task<PagedResult<PurchaseReceiptDto>> GetSettledAdjustmentsAsync(SettledPurchaseReceiptAdjustmentQueryDto query);
    Task<PurchaseReceiptFullDto> GetSettledAdjustmentByIdAsync(string receiptId);
    Task<List<PurchaseOrderDto>> GetSettledOrdersAsync(SettledOrderQueryDto query);
    Task<string> CreateSettledAdjustmentAsync(CreatePurchaseReceiptDto dto);
    Task UpdateSettledAdjustmentAsync(string receiptId, UpdatePurchaseReceiptDto dto);
    Task DeleteSettledAdjustmentAsync(string receiptId);
    Task ApproveSettledAdjustmentAsync(string receiptId, ApproveSettledAdjustmentDto dto);

    // SYSW530 - 已日結退貨單驗退調整作業
    Task<PagedResult<PurchaseReceiptDto>> GetClosedReturnAdjustmentsAsync(ClosedReturnAdjustmentQueryDto query);
    Task<PurchaseReceiptFullDto> GetClosedReturnAdjustmentByIdAsync(string receiptId);
    Task<List<PurchaseOrderDto>> GetClosedReturnOrdersAsync(ClosedReturnOrderQueryDto query);
    Task<string> CreateClosedReturnAdjustmentAsync(CreatePurchaseReceiptDto dto);
    Task UpdateClosedReturnAdjustmentAsync(string receiptId, UpdatePurchaseReceiptDto dto);
    Task DeleteClosedReturnAdjustmentAsync(string receiptId);
    Task ApproveClosedReturnAdjustmentAsync(string receiptId, ApproveSettledAdjustmentDto dto);

    // SYSW337 - 已日結退貨單驗退調整作業
    Task<PagedResult<PurchaseReceiptDto>> GetClosedReturnAdjustmentsV2Async(ClosedReturnAdjustmentQueryDto query);
    Task<PurchaseReceiptFullDto> GetClosedReturnAdjustmentV2ByIdAsync(string receiptId);
    Task<List<PurchaseOrderDto>> GetClosedReturnOrdersV2Async(ClosedReturnOrderQueryDto query);
    Task<string> CreateClosedReturnAdjustmentV2Async(CreatePurchaseReceiptDto dto);
    Task UpdateClosedReturnAdjustmentV2Async(string receiptId, UpdatePurchaseReceiptDto dto);
    Task DeleteClosedReturnAdjustmentV2Async(string receiptId);
    Task ApproveClosedReturnAdjustmentV2Async(string receiptId, ApproveSettledAdjustmentDto dto);
}

