using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.Services.ChartTools;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.ChartTools;

/// <summary>
/// 圖表功能控制器 (SYS1000)
/// </summary>
[Route("api/v1/charts")]
public class ChartController : BaseController
{
    private readonly IChartConfigService _service;

    public ChartController(
        IChartConfigService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢圖表配置列表
    /// </summary>
    [HttpGet("configs")]
    public async Task<ActionResult<ApiResponse<PagedResult<ChartConfigDto>>>> GetChartConfigs(
        [FromQuery] ChartConfigQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetChartConfigsAsync(query);
            return result;
        }, "查詢圖表配置列表失敗");
    }

    /// <summary>
    /// 查詢單筆圖表配置
    /// </summary>
    [HttpGet("configs/{chartConfigId}")]
    public async Task<ActionResult<ApiResponse<ChartConfigDto>>> GetChartConfig(Guid chartConfigId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetChartConfigByIdAsync(chartConfigId);
            return result;
        }, $"查詢圖表配置失敗: {chartConfigId}");
    }

    /// <summary>
    /// 新增圖表配置
    /// </summary>
    [HttpPost("configs")]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateChartConfig(
        [FromBody] CreateChartConfigDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var id = await _service.CreateChartConfigAsync(dto);
            return id;
        }, "新增圖表配置失敗");
    }

    /// <summary>
    /// 修改圖表配置
    /// </summary>
    [HttpPut("configs/{chartConfigId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateChartConfig(
        Guid chartConfigId,
        [FromBody] UpdateChartConfigDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateChartConfigAsync(chartConfigId, dto);
            return new object();
        }, $"修改圖表配置失敗: {chartConfigId}");
    }

    /// <summary>
    /// 刪除圖表配置
    /// </summary>
    [HttpDelete("configs/{chartConfigId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteChartConfig(Guid chartConfigId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteChartConfigAsync(chartConfigId);
            return new object();
        }, $"刪除圖表配置失敗: {chartConfigId}");
    }
}

