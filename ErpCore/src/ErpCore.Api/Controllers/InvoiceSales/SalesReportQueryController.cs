using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.InvoiceSales;
using ErpCore.Application.Services.InvoiceSales;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.InvoiceSales;

/// <summary>
/// 銷售報表查詢控制器 (SYSG610-SYSG640 - 報表查詢作業)
/// </summary>
[Route("api/v1/sales-reports")]
public class SalesReportQueryController : BaseController
{
    private readonly ISalesReportQueryService _service;

    public SalesReportQueryController(
        ISalesReportQueryService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢報表資料（明細報表）
    /// </summary>
    [HttpGet("detail")]
    public async Task<ActionResult<ApiResponse<PagedResult<SalesReportDetailDto>>>> QueryDetailReport(
        [FromQuery] SalesReportQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.QueryDetailReportAsync(query);
            return result;
        }, "查詢銷售報表明細失敗");
    }

    /// <summary>
    /// 查詢報表資料（彙總報表）
    /// </summary>
    [HttpGet("summary")]
    public async Task<ActionResult<ApiResponse<PagedResult<SalesReportSummaryDto>>>> QuerySummaryReport(
        [FromQuery] SalesReportQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.QuerySummaryReportAsync(query);
            return result;
        }, "查詢銷售報表彙總失敗");
    }
}

