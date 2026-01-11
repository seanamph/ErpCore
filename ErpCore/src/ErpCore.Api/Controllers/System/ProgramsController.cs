using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.System;

/// <summary>
/// 系統作業資料維護作業控制器 (SYS0430)
/// </summary>
[Route("api/v1/programs")]
public class ProgramsController : BaseController
{
    private readonly IProgramService _service;

    public ProgramsController(
        IProgramService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢作業列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ProgramDto>>>> GetPrograms(
        [FromQuery] ProgramQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetProgramsAsync(query);
            return result;
        }, "查詢作業列表失敗");
    }

    /// <summary>
    /// 查詢單筆作業
    /// </summary>
    [HttpGet("{programId}")]
    public async Task<ActionResult<ApiResponse<ProgramDto>>> GetProgram(string programId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetProgramAsync(programId);
            return result;
        }, $"查詢作業失敗: {programId}");
    }

    /// <summary>
    /// 新增作業
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateProgram(
        [FromBody] CreateProgramDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateProgramAsync(dto);
            return result;
        }, "新增作業失敗");
    }

    /// <summary>
    /// 修改作業
    /// </summary>
    [HttpPut("{programId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateProgram(
        string programId,
        [FromBody] UpdateProgramDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateProgramAsync(programId, dto);
        }, $"修改作業失敗: {programId}");
    }

    /// <summary>
    /// 刪除作業
    /// </summary>
    [HttpDelete("{programId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteProgram(string programId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteProgramAsync(programId);
        }, $"刪除作業失敗: {programId}");
    }

    /// <summary>
    /// 批次刪除作業
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteProgramsBatch(
        [FromBody] BatchDeleteProgramsDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteProgramsBatchAsync(dto);
        }, "批次刪除作業失敗");
    }
}
