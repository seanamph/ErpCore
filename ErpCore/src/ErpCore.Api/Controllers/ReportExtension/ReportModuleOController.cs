using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.ReportExtension;
using ErpCore.Application.Services.ReportExtension;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.ReportExtension;

/// <summary>
/// 報表模組O控制器
/// </summary>
[Route("api/v1/reports/module-o")]
public class ReportModuleOController : BaseController
{
    private readonly IReportModule7Service _service;

    public ReportModuleOController(
        IReportModule7Service service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢報表列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ReportQueryDto>>>> GetReports([FromQuery] ReportQueryListDto query)
    {
        return await ExecuteAsync(async () =>
        {
            return await _service.GetReportsAsync(query);
        }, "查詢報表列表失敗");
    }

    /// <summary>
    /// 執行報表查詢
    /// </summary>
    [HttpPost("{reportCode}/query")]
    public async Task<ActionResult<ApiResponse<ReportQueryResultDto>>> QueryReport(string reportCode, [FromBody] ReportQueryRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            request.ReportCode = reportCode;
            return await _service.QueryReportAsync(request);
        }, "執行報表查詢失敗");
    }
}

