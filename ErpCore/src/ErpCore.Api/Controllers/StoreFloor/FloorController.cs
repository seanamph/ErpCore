using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.StoreFloor;
using ErpCore.Application.Services.StoreFloor;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.StoreFloor;

/// <summary>
/// 樓層資料維護控制器 (SYS6310-SYS6370)
/// </summary>
[Route("api/v1/floors")]
public class FloorController : BaseController
{
    private readonly IFloorService _service;

    public FloorController(
        IFloorService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢樓層列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<FloorDto>>>> GetFloors(
        [FromQuery] FloorQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetFloorsAsync(query);
            return result;
        }, "查詢樓層列表失敗");
    }

    /// <summary>
    /// 查詢單筆樓層
    /// </summary>
    [HttpGet("{floorId}")]
    public async Task<ActionResult<ApiResponse<FloorDto>>> GetFloor(string floorId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetFloorByIdAsync(floorId);
            return result;
        }, $"查詢樓層失敗: {floorId}");
    }

    /// <summary>
    /// 新增樓層
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateFloor(
        [FromBody] CreateFloorDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateFloorAsync(dto);
            return result;
        }, "新增樓層失敗");
    }

    /// <summary>
    /// 修改樓層
    /// </summary>
    [HttpPut("{floorId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateFloor(
        string floorId,
        [FromBody] UpdateFloorDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateFloorAsync(floorId, dto);
        }, $"修改樓層失敗: {floorId}");
    }

    /// <summary>
    /// 刪除樓層
    /// </summary>
    [HttpDelete("{floorId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteFloor(string floorId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteFloorAsync(floorId);
        }, $"刪除樓層失敗: {floorId}");
    }

    /// <summary>
    /// 檢查樓層代碼是否存在
    /// </summary>
    [HttpGet("check/{floorId}")]
    public async Task<ActionResult<ApiResponse<bool>>> CheckFloorExists(string floorId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.ExistsAsync(floorId);
            return result;
        }, $"檢查樓層代碼是否存在失敗: {floorId}");
    }

    /// <summary>
    /// 更新樓層狀態
    /// </summary>
    [HttpPut("{floorId}/status")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateFloorStatus(
        string floorId,
        [FromBody] UpdateFloorStatusDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateStatusAsync(floorId, dto.Status);
        }, $"更新樓層狀態失敗: {floorId}");
    }
}

