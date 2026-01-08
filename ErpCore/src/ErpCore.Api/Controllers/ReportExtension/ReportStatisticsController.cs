using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.ReportExtension;
using ErpCore.Application.Services.ReportExtension;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.ReportExtension;

/// <summary>
/// 報表統計作業控制器 (SYS7C10, SYS7C30)
/// </summary>
[Route("api/v1/reports/statistics")]
public class ReportStatisticsController : BaseController
{
    private readonly IReportStatisticsService _service;

    public ReportStatisticsController(
        IReportStatisticsService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢報表統計記錄
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ReportStatisticDto>>>> GetStatistics([FromQuery] ReportStatisticQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            return await _service.GetStatisticsAsync(query);
        }, "查詢報表統計記錄失敗");
    }

    /// <summary>
    /// 建立報表統計記錄
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ReportStatisticDto>>> CreateStatistic([FromBody] CreateReportStatisticDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            return await _service.CreateStatisticAsync(dto);
        }, "建立報表統計記錄失敗");
    }
}

