using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.SystemConfig;
using ErpCore.Application.Services.SystemConfig;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.SystemConfig;

/// <summary>
/// 子系統項目資料維護作業控制器 (CFG0420)
/// </summary>
[Route("api/v1/config/subsystems")]
public class ConfigSubSystemsController : BaseController
{
    private readonly IConfigSubSystemService _service;

    public ConfigSubSystemsController(
        IConfigSubSystemService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢子系統列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ConfigSubSystemDto>>>> GetConfigSubSystems(
        [FromQuery] ConfigSubSystemQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetConfigSubSystemsAsync(query);
            return result;
        }, "查詢子系統列表失敗");
    }

    /// <summary>
    /// 查詢單筆子系統
    /// </summary>
    [HttpGet("{subSystemId}")]
    public async Task<ActionResult<ApiResponse<ConfigSubSystemDto>>> GetConfigSubSystem(string subSystemId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetConfigSubSystemAsync(subSystemId);
            return result;
        }, $"查詢子系統失敗: {subSystemId}");
    }

    /// <summary>
    /// 新增子系統
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateConfigSubSystem(
        [FromBody] CreateConfigSubSystemDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateConfigSubSystemAsync(dto);
            return result;
        }, "新增子系統失敗");
    }

    /// <summary>
    /// 修改子系統
    /// </summary>
    [HttpPut("{subSystemId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateConfigSubSystem(
        string subSystemId,
        [FromBody] UpdateConfigSubSystemDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateConfigSubSystemAsync(subSystemId, dto);
        }, $"修改子系統失敗: {subSystemId}");
    }

    /// <summary>
    /// 刪除子系統
    /// </summary>
    [HttpDelete("{subSystemId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteConfigSubSystem(string subSystemId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteConfigSubSystemAsync(subSystemId);
        }, $"刪除子系統失敗: {subSystemId}");
    }

    /// <summary>
    /// 批次刪除子系統
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteConfigSubSystemsBatch(
        [FromBody] BatchDeleteConfigSubSystemDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteConfigSubSystemsBatchAsync(dto);
        }, "批次刪除子系統失敗");
    }
}

