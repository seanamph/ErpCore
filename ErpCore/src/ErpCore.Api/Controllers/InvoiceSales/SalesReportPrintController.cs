using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.InvoiceSales;
using ErpCore.Application.Services.InvoiceSales;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.InvoiceSales;

/// <summary>
/// 銷售報表列印控制器 (SYSG710-SYSG7I0 - 報表列印作業)
/// </summary>
[Route("api/v1/reports")]
public class SalesReportPrintController : BaseController
{
    private readonly IReportPrintService _service;

    public SalesReportPrintController(
        IReportPrintService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 列印報表
    /// </summary>
    [HttpPost("print")]
    public async Task<ActionResult<ApiResponse<ReportPrintResultDto>>> PrintReport(
        [FromBody] ReportPrintDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.PrintReportAsync(dto);
            return result;
        }, "列印報表失敗");
    }

    /// <summary>
    /// 預覽報表
    /// </summary>
    [HttpPost("preview")]
    public async Task<ActionResult<ApiResponse<string>>> PreviewReport(
        [FromBody] ReportPrintDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.PreviewReportAsync(dto);
            return result;
        }, "預覽報表失敗");
    }

    /// <summary>
    /// 查詢報表模板列表
    /// </summary>
    [HttpGet("templates")]
    public async Task<ActionResult<ApiResponse<List<ReportTemplateDto>>>> GetTemplates(
        [FromQuery] string reportType,
        [FromQuery] string? status = null)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetTemplatesAsync(reportType, status);
            return result;
        }, "查詢報表模板列表失敗");
    }
}

