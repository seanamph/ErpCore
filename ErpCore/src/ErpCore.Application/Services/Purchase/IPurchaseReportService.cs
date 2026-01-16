using ErpCore.Application.DTOs.Purchase;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Purchase;

/// <summary>
/// 採購報表查詢服務介面 (SYSP410-SYSP4I0)
/// </summary>
public interface IPurchaseReportService
{
    /// <summary>
    /// 查詢採購報表列表
    /// </summary>
    Task<PagedResult<PurchaseReportResultDto>> QueryPurchaseReportsAsync(PurchaseReportQueryDto query);

    /// <summary>
    /// 查詢採購報表明細列表
    /// </summary>
    Task<PagedResult<PurchaseReportDetailResultDto>> QueryPurchaseReportDetailsAsync(PurchaseReportQueryDto query);

    /// <summary>
    /// 匯出採購報表
    /// </summary>
    Task<byte[]> ExportPurchaseReportAsync(PurchaseReportExportDto exportDto);
}
