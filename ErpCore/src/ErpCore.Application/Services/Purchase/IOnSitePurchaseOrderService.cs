using ErpCore.Application.DTOs.Purchase;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Purchase;

/// <summary>
/// 現場打單作業服務介面 (SYSW322)
/// </summary>
public interface IOnSitePurchaseOrderService
{
    /// <summary>
    /// 查詢現場打單申請單列表
    /// </summary>
    Task<PagedResult<PurchaseOrderDto>> GetOnSitePurchaseOrdersAsync(PurchaseOrderQueryDto query);

    /// <summary>
    /// 查詢單筆現場打單申請單
    /// </summary>
    Task<PurchaseOrderFullDto> GetOnSitePurchaseOrderByIdAsync(string orderId);

    /// <summary>
    /// 新增現場打單申請單
    /// </summary>
    Task<string> CreateOnSitePurchaseOrderAsync(CreatePurchaseOrderDto dto);

    /// <summary>
    /// 修改現場打單申請單
    /// </summary>
    Task UpdateOnSitePurchaseOrderAsync(string orderId, UpdatePurchaseOrderDto dto);

    /// <summary>
    /// 刪除現場打單申請單
    /// </summary>
    Task DeleteOnSitePurchaseOrderAsync(string orderId);

    /// <summary>
    /// 送出現場打單申請單
    /// </summary>
    Task SubmitOnSitePurchaseOrderAsync(string orderId);

    /// <summary>
    /// 審核現場打單申請單
    /// </summary>
    Task ApproveOnSitePurchaseOrderAsync(string orderId);

    /// <summary>
    /// 取消現場打單申請單
    /// </summary>
    Task CancelOnSitePurchaseOrderAsync(string orderId);

    /// <summary>
    /// 根據條碼查詢商品資訊（現場打單專用）
    /// </summary>
    Task<GoodsByBarcodeDto> GetGoodsByBarcodeAsync(string barcode);
}
