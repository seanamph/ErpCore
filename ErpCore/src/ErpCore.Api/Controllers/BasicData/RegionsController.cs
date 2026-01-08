using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.BasicData;
using ErpCore.Application.Services.BasicData;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.BasicData;

/// <summary>
/// 地區設定控制器 (SYSBC30)
/// </summary>
[Route("api/v1/regions")]
public class RegionsController : BaseController
{
    private readonly IRegionService _service;

    public RegionsController(
        IRegionService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢地區列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<RegionDto>>>> GetRegions(
        [FromQuery] RegionQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetRegionsAsync(query);
            return result;
        }, "查詢地區列表失敗");
    }

    /// <summary>
    /// 查詢單筆地區
    /// </summary>
    [HttpGet("{regionId}")]
    public async Task<ActionResult<ApiResponse<RegionDto>>> GetRegion(string regionId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetRegionByIdAsync(regionId);
            return result;
        }, $"查詢地區失敗: {regionId}");
    }

    /// <summary>
    /// 新增地區
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateRegion(
        [FromBody] CreateRegionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateRegionAsync(dto);
            return result;
        }, "新增地區失敗");
    }

    /// <summary>
    /// 修改地區
    /// </summary>
    [HttpPut("{regionId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateRegion(
        string regionId,
        [FromBody] UpdateRegionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateRegionAsync(regionId, dto);
        }, $"修改地區失敗: {regionId}");
    }

    /// <summary>
    /// 刪除地區
    /// </summary>
    [HttpDelete("{regionId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteRegion(string regionId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteRegionAsync(regionId);
        }, $"刪除地區失敗: {regionId}");
    }

    /// <summary>
    /// 批次刪除地區
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteRegionsBatch(
        [FromBody] BatchDeleteRegionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteRegionsBatchAsync(dto);
        }, "批次刪除地區失敗");
    }
}

