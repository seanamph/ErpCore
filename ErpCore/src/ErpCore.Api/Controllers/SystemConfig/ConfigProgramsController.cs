using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.SystemConfig;
using ErpCore.Application.Services.SystemConfig;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.SystemConfig;

/// <summary>
/// 系統作業資料維護作業控制器 (CFG0430)
/// </summary>
[Route("api/v1/config/programs")]
public class ConfigProgramsController : BaseController
{
    private readonly IConfigProgramService _service;

    public ConfigProgramsController(
        IConfigProgramService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢作業列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ConfigProgramDto>>>> GetConfigPrograms(
        [FromQuery] ConfigProgramQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetConfigProgramsAsync(query);
            return result;
        }, "查詢作業列表失敗");
    }

    /// <summary>
    /// 查詢單筆作業
    /// </summary>
    [HttpGet("{programId}")]
    public async Task<ActionResult<ApiResponse<ConfigProgramDto>>> GetConfigProgram(string programId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetConfigProgramAsync(programId);
            return result;
        }, $"查詢作業失敗: {programId}");
    }

    /// <summary>
    /// 新增作業
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateConfigProgram(
        [FromBody] CreateConfigProgramDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateConfigProgramAsync(dto);
            return result;
        }, "新增作業失敗");
    }

    /// <summary>
    /// 修改作業
    /// </summary>
    [HttpPut("{programId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateConfigProgram(
        string programId,
        [FromBody] UpdateConfigProgramDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateConfigProgramAsync(programId, dto);
        }, $"修改作業失敗: {programId}");
    }

    /// <summary>
    /// 刪除作業
    /// </summary>
    [HttpDelete("{programId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteConfigProgram(string programId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteConfigProgramAsync(programId);
        }, $"刪除作業失敗: {programId}");
    }

    /// <summary>
    /// 批次刪除作業
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteConfigProgramsBatch(
        [FromBody] BatchDeleteConfigProgramDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteConfigProgramsBatchAsync(dto);
        }, "批次刪除作業失敗");
    }
}

