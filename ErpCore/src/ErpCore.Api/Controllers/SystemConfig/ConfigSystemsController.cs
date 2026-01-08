using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.SystemConfig;
using ErpCore.Application.Services.SystemConfig;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.SystemConfig;

/// <summary>
/// 主系統項目資料維護作業控制器 (CFG0410)
/// </summary>
[Route("api/v1/config/systems")]
public class ConfigSystemsController : BaseController
{
    private readonly IConfigSystemService _service;

    public ConfigSystemsController(
        IConfigSystemService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢主系統列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ConfigSystemDto>>>> GetConfigSystems(
        [FromQuery] ConfigSystemQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetConfigSystemsAsync(query);
            return result;
        }, "查詢主系統列表失敗");
    }

    /// <summary>
    /// 查詢單筆主系統
    /// </summary>
    [HttpGet("{systemId}")]
    public async Task<ActionResult<ApiResponse<ConfigSystemDto>>> GetConfigSystem(string systemId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetConfigSystemAsync(systemId);
            return result;
        }, $"查詢主系統失敗: {systemId}");
    }

    /// <summary>
    /// 新增主系統
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateConfigSystem(
        [FromBody] CreateConfigSystemDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateConfigSystemAsync(dto);
            return result;
        }, "新增主系統失敗");
    }

    /// <summary>
    /// 修改主系統
    /// </summary>
    [HttpPut("{systemId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateConfigSystem(
        string systemId,
        [FromBody] UpdateConfigSystemDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateConfigSystemAsync(systemId, dto);
        }, $"修改主系統失敗: {systemId}");
    }

    /// <summary>
    /// 刪除主系統
    /// </summary>
    [HttpDelete("{systemId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteConfigSystem(string systemId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteConfigSystemAsync(systemId);
        }, $"刪除主系統失敗: {systemId}");
    }

    /// <summary>
    /// 批次刪除主系統
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteConfigSystemsBatch(
        [FromBody] BatchDeleteConfigSystemDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteConfigSystemsBatchAsync(dto);
        }, "批次刪除主系統失敗");
    }
}

