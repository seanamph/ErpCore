using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.AnalysisReport;
using ErpCore.Application.Services.AnalysisReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.AnalysisReport;

/// <summary>
/// 耗材標籤列印作業控制器 (SYSA254)
/// </summary>
[Route("api/v1/consumables/print")]
public class ConsumablePrintController : BaseController
{
    private readonly IConsumablePrintService _service;

    public ConsumablePrintController(
        IConsumablePrintService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢耗材列表（用於列印）
    /// </summary>
    [HttpGet("list")]
    public async Task<ActionResult<ApiResponse<ConsumablePrintListDto>>> GetPrintList(
        [FromQuery] ConsumablePrintQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPrintListAsync(query);
            return result;
        }, "查詢耗材列表失敗");
    }

    /// <summary>
    /// 批次列印耗材標籤
    /// </summary>
    [HttpPost("batch")]
    public async Task<ActionResult<ApiResponse<BatchPrintResponseDto>>> BatchPrint(
        [FromBody] BatchPrintDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            var result = await _service.BatchPrintAsync(dto, userId);
            return result;
        }, "批次列印失敗");
    }

    /// <summary>
    /// 查詢列印記錄列表
    /// </summary>
    [HttpGet("logs")]
    public async Task<ActionResult<ApiResponse<PagedResult<ConsumablePrintLogDto>>>> GetPrintLogs(
        [FromQuery] ConsumablePrintLogQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPrintLogsAsync(query);
            return result;
        }, "查詢列印記錄列表失敗");
    }
}
