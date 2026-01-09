using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.StoreFloor;
using ErpCore.Application.Services.StoreFloor;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.StoreFloor;

/// <summary>
/// 樓層查詢控制器 (SYS6381-SYS63A0 - 樓層查詢作業)
/// </summary>
[Route("api/v1/floors/query")]
public class FloorQueryController : BaseController
{
    private readonly IFloorQueryService _service;

    public FloorQueryController(
        IFloorQueryService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢樓層列表（進階查詢）
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<PagedResult<FloorQueryDto>>>> QueryFloors(
        [FromBody] FloorQueryRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.QueryFloorsAsync(request);
            return result;
        }, "查詢樓層列表失敗");
    }

    /// <summary>
    /// 查詢樓層統計資訊
    /// </summary>
    [HttpGet("statistics")]
    public async Task<ActionResult<ApiResponse<FloorStatisticsDto>>> GetFloorStatistics(
        [FromQuery] FloorStatisticsRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetFloorStatisticsAsync(request);
            return result;
        }, "查詢樓層統計資訊失敗");
    }

    /// <summary>
    /// 匯出樓層查詢結果
    /// </summary>
    [HttpPost("export")]
    public async Task<ActionResult> ExportFloors(
        [FromBody] FloorExportDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var bytes = await _service.ExportFloorsAsync(dto);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                $"樓層查詢結果_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }, "匯出樓層查詢結果失敗");
    }
}

