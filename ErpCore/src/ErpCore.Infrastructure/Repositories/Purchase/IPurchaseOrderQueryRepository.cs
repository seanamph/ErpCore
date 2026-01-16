using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.Purchase;

/// <summary>
/// 採購單查詢 Repository 介面 (SYSP310-SYSP330)
/// </summary>
public interface IPurchaseOrderQueryRepository
{
    /// <summary>
    /// 查詢採購單列表（使用視圖）
    /// </summary>
    Task<IEnumerable<PurchaseOrderQueryResult>> QueryPurchaseOrdersAsync(PurchaseOrderQueryRequest request);

    /// <summary>
    /// 查詢採購單總數
    /// </summary>
    Task<int> GetPurchaseOrderCountAsync(PurchaseOrderQueryRequest request);

    /// <summary>
    /// 查詢採購單明細（使用視圖）
    /// </summary>
    Task<IEnumerable<PurchaseOrderDetailQueryItem>> GetPurchaseOrderDetailsAsync(string orderId);

    /// <summary>
    /// 查詢採購單統計資料
    /// </summary>
    Task<PurchaseOrderStatistics> GetPurchaseOrderStatisticsAsync(PurchaseOrderStatisticsRequest request);
}
