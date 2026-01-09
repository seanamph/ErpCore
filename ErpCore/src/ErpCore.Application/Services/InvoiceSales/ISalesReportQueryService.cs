using ErpCore.Application.DTOs.InvoiceSales;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.InvoiceSales;

/// <summary>
/// 銷售報表查詢服務接口 (SYSG610-SYSG640 - 報表查詢作業)
/// </summary>
public interface ISalesReportQueryService
{
    /// <summary>
    /// 查詢報表資料（明細報表）
    /// </summary>
    Task<PagedResult<SalesReportDetailDto>> QueryDetailReportAsync(SalesReportQueryDto query);

    /// <summary>
    /// 查詢報表資料（彙總報表）
    /// </summary>
    Task<PagedResult<SalesReportSummaryDto>> QuerySummaryReportAsync(SalesReportQueryDto query);
}

