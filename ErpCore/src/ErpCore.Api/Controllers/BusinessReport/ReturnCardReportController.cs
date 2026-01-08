using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.BusinessReport;
using ErpCore.Application.Services.BusinessReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.BusinessReport;

/// <summary>
/// 銷退卡報表控制器 (SYSL310)
/// </summary>
[Route("api/v1/return-cards/report")]
public class ReturnCardReportController : BaseController
{
    private readonly IReturnCardReportService _service;

    public ReturnCardReportController(
        IReturnCardReportService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢銷退卡報表資料
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<ReturnCardReportResultDto>>> GetReport(
        [FromQuery] ReturnCardReportQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetReportAsync(query);
            return result;
        }, "查詢銷退卡報表資料失敗");
    }

    /// <summary>
    /// 列印銷退卡報表
    /// </summary>
    [HttpPost("print")]
    public async Task<ActionResult<ApiResponse<BusinessReportPrintResultDto>>> PrintReport(
        [FromBody] BusinessReportPrintRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.PrintReportAsync(request);
            return result;
        }, "列印銷退卡報表失敗");
    }

    /// <summary>
    /// 匯出銷退卡報表
    /// </summary>
    [HttpPost("export")]
    public async Task<ActionResult<ApiResponse<BusinessReportPrintResultDto>>> ExportReport(
        [FromBody] BusinessReportExportRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.ExportReportAsync(request);
            return result;
        }, "匯出銷退卡報表失敗");
    }
}

