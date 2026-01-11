using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.System;

/// <summary>
/// 主系統項目資料維護作業控制器 (SYS0410)
/// </summary>
[Route("api/v1/systems")]
public class SystemsController : BaseController
{
    private readonly ISystemService _service;

    public SystemsController(
        ISystemService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢主系統列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<SystemDto>>>> GetSystems(
        [FromQuery] SystemQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSystemsAsync(query);
            return result;
        }, "查詢主系統列表失敗");
    }

    /// <summary>
    /// 查詢單筆主系統
    /// </summary>
    [HttpGet("{systemId}")]
    public async Task<ActionResult<ApiResponse<SystemDto>>> GetSystem(string systemId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSystemAsync(systemId);
            return result;
        }, $"查詢主系統失敗: {systemId}");
    }

    /// <summary>
    /// 新增主系統
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateSystem(
        [FromBody] CreateSystemDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateSystemAsync(dto);
            return result;
        }, "新增主系統失敗");
    }

    /// <summary>
    /// 修改主系統
    /// </summary>
    [HttpPut("{systemId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateSystem(
        string systemId,
        [FromBody] UpdateSystemDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateSystemAsync(systemId, dto);
        }, $"修改主系統失敗: {systemId}");
    }

    /// <summary>
    /// 刪除主系統
    /// </summary>
    [HttpDelete("{systemId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteSystem(string systemId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteSystemAsync(systemId);
        }, $"刪除主系統失敗: {systemId}");
    }

    /// <summary>
    /// 批次刪除主系統
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteSystemsBatch(
        [FromBody] BatchDeleteSystemsDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteSystemsBatchAsync(dto);
        }, "批次刪除主系統失敗");
    }

    /// <summary>
    /// 取得系統型態選項
    /// </summary>
    [HttpGet("system-types")]
    public async Task<ActionResult<ApiResponse<List<SystemTypeOptionDto>>>> GetSystemTypes()
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSystemTypesAsync();
            return result;
        }, "取得系統型態選項失敗");
    }
}
