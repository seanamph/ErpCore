using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.BusinessReport;
using ErpCore.Application.Services.BusinessReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.BusinessReport;

/// <summary>
/// 加班發放報表控制器 (SYSL510)
/// </summary>
[Route("api/v1/overtime-payments/report")]
public class OvertimePaymentReportController : BaseController
{
    private readonly IOvertimePaymentReportService _service;

    public OvertimePaymentReportController(
        IOvertimePaymentReportService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢加班發放報表資料
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<OvertimePaymentReportResultDto>>> GetReport(
        [FromQuery] OvertimePaymentReportQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetReportAsync(query);
            return result;
        }, "查詢加班發放報表資料失敗");
    }

    /// <summary>
    /// 列印加班發放報表
    /// </summary>
    [HttpPost("print")]
    public async Task<ActionResult<ApiResponse<BusinessReportPrintResultDto>>> PrintReport(
        [FromBody] BusinessReportPrintRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.PrintReportAsync(request);
            return result;
        }, "列印加班發放報表失敗");
    }

    /// <summary>
    /// 匯出加班發放報表
    /// </summary>
    [HttpPost("export")]
    public async Task<ActionResult<ApiResponse<BusinessReportPrintResultDto>>> ExportReport(
        [FromBody] BusinessReportExportRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.ExportReportAsync(request);
            return result;
        }, "匯出加班發放報表失敗");
    }
}

