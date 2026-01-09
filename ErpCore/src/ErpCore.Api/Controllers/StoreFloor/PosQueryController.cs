using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.StoreFloor;
using ErpCore.Application.Services.StoreFloor;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.StoreFloor;

/// <summary>
/// POS查詢控制器 (SYS6A04-SYS6A19 - POS查詢作業)
/// </summary>
[Route("api/v1/pos-terminals/query")]
public class PosQueryController : BaseController
{
    private readonly IPosTerminalQueryService _service;

    public PosQueryController(
        IPosTerminalQueryService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢POS終端列表（進階查詢）
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<PagedResult<PosTerminalDto>>>> QueryPosTerminals(
        [FromBody] PosTerminalQueryRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.QueryPosTerminalsAsync(request);
            return result;
        }, "查詢POS終端列表失敗");
    }

    /// <summary>
    /// 查詢POS終端統計資訊
    /// </summary>
    [HttpGet("statistics")]
    public async Task<ActionResult<ApiResponse<PosTerminalStatisticsDto>>> GetPosTerminalStatistics(
        [FromQuery] PosTerminalStatisticsRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPosTerminalStatisticsAsync(request);
            return result;
        }, "查詢POS終端統計資訊失敗");
    }
}

