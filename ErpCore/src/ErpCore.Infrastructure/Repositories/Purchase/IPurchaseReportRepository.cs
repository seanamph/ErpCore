using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.Purchase;

/// <summary>
/// 採購報表查詢 Repository 介面 (SYSP410-SYSP4I0)
/// </summary>
public interface IPurchaseReportRepository
{
    /// <summary>
    /// 查詢採購報表列表
    /// </summary>
    Task<IEnumerable<PurchaseReportResult>> QueryPurchaseReportsAsync(PurchaseReportQuery query);

    /// <summary>
    /// 查詢採購報表總數
    /// </summary>
    Task<int> GetPurchaseReportCountAsync(PurchaseReportQuery query);

    /// <summary>
    /// 查詢採購報表明細列表
    /// </summary>
    Task<IEnumerable<PurchaseReportDetailResult>> QueryPurchaseReportDetailsAsync(PurchaseReportQuery query);

    /// <summary>
    /// 查詢採購報表明細總數
    /// </summary>
    Task<int> GetPurchaseReportDetailCountAsync(PurchaseReportQuery query);
}
