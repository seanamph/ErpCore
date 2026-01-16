using ErpCore.Application.DTOs.Purchase;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Purchase;

/// <summary>
/// 採購單查詢服務介面 (SYSP310-SYSP330)
/// </summary>
public interface IPurchaseOrderQueryService
{
    /// <summary>
    /// 查詢採購單列表
    /// </summary>
    Task<PagedResult<PurchaseOrderQueryResultDto>> QueryPurchaseOrdersAsync(PurchaseOrderQueryRequestDto request);

    /// <summary>
    /// 查詢採購單明細
    /// </summary>
    Task<PurchaseOrderDetailQueryDto> GetPurchaseOrderDetailsAsync(string orderId);

    /// <summary>
    /// 查詢採購單統計資料
    /// </summary>
    Task<PurchaseOrderStatisticsDto> GetPurchaseOrderStatisticsAsync(PurchaseOrderStatisticsRequestDto request);

    /// <summary>
    /// 匯出採購單查詢結果
    /// </summary>
    Task<byte[]> ExportPurchaseOrdersAsync(PurchaseOrderExportRequestDto request);

    /// <summary>
    /// 列印採購單
    /// </summary>
    Task<byte[]> PrintPurchaseOrderAsync(string orderId);
}
