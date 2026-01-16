using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.BasicData;
using ErpCore.Application.Services.BasicData;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.BasicData;

/// <summary>
/// 區域基本資料維護控制器 (SYSB450)
/// </summary>
[Route("api/v1/areas")]
public class AreasController : BaseController
{
    private readonly IAreaService _service;

    public AreasController(
        IAreaService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢區域列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<AreaDto>>>> GetAreas(
        [FromQuery] AreaQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetAreasAsync(query);
            return result;
        }, "查詢區域列表失敗");
    }

    /// <summary>
    /// 查詢單筆區域
    /// </summary>
    [HttpGet("{areaId}")]
    public async Task<ActionResult<ApiResponse<AreaDto>>> GetArea(string areaId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetAreaByIdAsync(areaId);
            return result;
        }, $"查詢區域失敗: {areaId}");
    }

    /// <summary>
    /// 新增區域
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateArea(
        [FromBody] CreateAreaDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateAreaAsync(dto);
            return result;
        }, "新增區域失敗");
    }

    /// <summary>
    /// 修改區域
    /// </summary>
    [HttpPut("{areaId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateArea(
        string areaId,
        [FromBody] UpdateAreaDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateAreaAsync(areaId, dto);
        }, $"修改區域失敗: {areaId}");
    }

    /// <summary>
    /// 刪除區域
    /// </summary>
    [HttpDelete("{areaId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteArea(string areaId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteAreaAsync(areaId);
        }, $"刪除區域失敗: {areaId}");
    }

    /// <summary>
    /// 批次刪除區域
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteAreasBatch(
        [FromBody] BatchDeleteAreaDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteAreasBatchAsync(dto);
        }, "批次刪除區域失敗");
    }

    /// <summary>
    /// 更新區域狀態
    /// </summary>
    [HttpPut("{areaId}/status")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateAreaStatus(
        string areaId,
        [FromBody] UpdateAreaStatusDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateStatusAsync(areaId, dto);
        }, $"更新區域狀態失敗: {areaId}");
    }
}
