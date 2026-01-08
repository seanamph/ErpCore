using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Recruitment;
using ErpCore.Application.Services.Recruitment;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Recruitment;

/// <summary>
/// 潛客維護作業控制器 (SYSC180)
/// </summary>
[Route("api/v1/prospects")]
public class ProspectController : BaseController
{
    private readonly IProspectService _service;

    public ProspectController(
        IProspectService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢潛客列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ProspectDto>>>> GetProspects(
        [FromQuery] ProspectQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetProspectsAsync(query);
            return result;
        }, "查詢潛客列表失敗");
    }

    /// <summary>
    /// 查詢單筆潛客
    /// </summary>
    [HttpGet("{prospectId}")]
    public async Task<ActionResult<ApiResponse<ProspectDto>>> GetProspect(string prospectId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetProspectByIdAsync(prospectId);
            return result;
        }, $"查詢潛客失敗: {prospectId}");
    }

    /// <summary>
    /// 新增潛客
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateProspect(
        [FromBody] CreateProspectDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateProspectAsync(dto);
            return result;
        }, "新增潛客失敗");
    }

    /// <summary>
    /// 修改潛客
    /// </summary>
    [HttpPut("{prospectId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateProspect(
        string prospectId,
        [FromBody] UpdateProspectDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateProspectAsync(prospectId, dto);
        }, $"修改潛客失敗: {prospectId}");
    }

    /// <summary>
    /// 刪除潛客
    /// </summary>
    [HttpDelete("{prospectId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteProspect(string prospectId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteProspectAsync(prospectId);
        }, $"刪除潛客失敗: {prospectId}");
    }

    /// <summary>
    /// 批次刪除潛客
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ActionResult<ApiResponse<object>>> BatchDeleteProspects(
        [FromBody] BatchDeleteProspectDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.BatchDeleteProspectsAsync(dto);
        }, "批次刪除潛客失敗");
    }

    /// <summary>
    /// 更新潛客狀態
    /// </summary>
    [HttpPatch("{prospectId}/status")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateProspectStatus(
        string prospectId,
        [FromBody] UpdateProspectStatusDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateProspectStatusAsync(prospectId, dto);
        }, $"更新潛客狀態失敗: {prospectId}");
    }
}

