using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.ReportExtension;
using ErpCore.Application.Services.ReportExtension;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.ReportExtension;

/// <summary>
/// 報表列印作業控制器 (SYS7B10-SYS7B40)
/// </summary>
[Route("api/v1/reports/print")]
public class ReportPrintController : BaseController
{
    private readonly IReportPrintService _service;

    public ReportPrintController(
        IReportPrintService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 列印報表
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ReportPrintLogDto>>> PrintReport([FromBody] ReportPrintRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            return await _service.PrintReportAsync(request);
        }, "列印報表失敗");
    }

    /// <summary>
    /// 查詢報表列印記錄
    /// </summary>
    [HttpGet("logs")]
    public async Task<ActionResult<ApiResponse<PagedResult<ReportPrintLogDto>>>> GetPrintLogs([FromQuery] ReportPrintLogQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            return await _service.GetPrintLogsAsync(query);
        }, "查詢報表列印記錄失敗");
    }
}

