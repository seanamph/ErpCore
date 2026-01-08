using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Recruitment;
using ErpCore.Application.Services.Recruitment;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Recruitment;

/// <summary>
/// 潛客主檔維護作業控制器 (SYSC165)
/// </summary>
[Route("api/v1/prospect-masters")]
public class ProspectMasterController : BaseController
{
    private readonly IProspectMasterService _service;

    public ProspectMasterController(
        IProspectMasterService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢潛客主檔列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ProspectMasterDto>>>> GetProspectMasters(
        [FromQuery] ProspectMasterQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetProspectMastersAsync(query);
            return result;
        }, "查詢潛客主檔列表失敗");
    }

    /// <summary>
    /// 查詢單筆潛客主檔
    /// </summary>
    [HttpGet("{masterId}")]
    public async Task<ActionResult<ApiResponse<ProspectMasterDto>>> GetProspectMaster(string masterId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetProspectMasterByIdAsync(masterId);
            return result;
        }, $"查詢潛客主檔失敗: {masterId}");
    }

    /// <summary>
    /// 新增潛客主檔
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateProspectMaster(
        [FromBody] CreateProspectMasterDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateProspectMasterAsync(dto);
            return result;
        }, "新增潛客主檔失敗");
    }

    /// <summary>
    /// 修改潛客主檔
    /// </summary>
    [HttpPut("{masterId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateProspectMaster(
        string masterId,
        [FromBody] UpdateProspectMasterDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateProspectMasterAsync(masterId, dto);
        }, $"修改潛客主檔失敗: {masterId}");
    }

    /// <summary>
    /// 刪除潛客主檔
    /// </summary>
    [HttpDelete("{masterId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteProspectMaster(string masterId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteProspectMasterAsync(masterId);
        }, $"刪除潛客主檔失敗: {masterId}");
    }

    /// <summary>
    /// 批次刪除潛客主檔
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ActionResult<ApiResponse<object>>> BatchDeleteProspectMasters(
        [FromBody] BatchDeleteProspectMasterDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.BatchDeleteProspectMastersAsync(dto);
        }, "批次刪除潛客主檔失敗");
    }

    /// <summary>
    /// 更新潛客主檔狀態
    /// </summary>
    [HttpPatch("{masterId}/status")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateProspectMasterStatus(
        string masterId,
        [FromBody] UpdateProspectMasterStatusDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateProspectMasterStatusAsync(masterId, dto);
        }, $"更新潛客主檔狀態失敗: {masterId}");
    }
}

