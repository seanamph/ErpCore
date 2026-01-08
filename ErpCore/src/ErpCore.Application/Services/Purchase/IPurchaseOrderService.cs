using ErpCore.Application.DTOs.Purchase;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Purchase;

/// <summary>
/// 採購單服務介面
/// </summary>
public interface IPurchaseOrderService
{
    Task<PagedResult<PurchaseOrderDto>> GetPurchaseOrdersAsync(PurchaseOrderQueryDto query);
    Task<PurchaseOrderFullDto> GetPurchaseOrderByIdAsync(string orderId);
    Task<string> CreatePurchaseOrderAsync(CreatePurchaseOrderDto dto);
    Task UpdatePurchaseOrderAsync(string orderId, UpdatePurchaseOrderDto dto);
    Task DeletePurchaseOrderAsync(string orderId);
    Task SubmitPurchaseOrderAsync(string orderId);
    Task ApprovePurchaseOrderAsync(string orderId);
    Task CancelPurchaseOrderAsync(string orderId);
    Task ClosePurchaseOrderAsync(string orderId);
}

