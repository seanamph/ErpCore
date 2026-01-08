using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Procurement;
using ErpCore.Application.Services.Procurement;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Procurement;

/// <summary>
/// 採購其他功能控制器 (SYSP510-SYSP530)
/// </summary>
[ApiController]
[Route("api/v1/purchase-others")]
public class ProcurementOtherController : BaseController
{
    private readonly IProcurementOtherService _service;

    public ProcurementOtherController(
        IProcurementOtherService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢採購其他功能列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ProcurementOtherDto>>>> GetProcurementOthers(
        [FromQuery] ProcurementOtherQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetProcurementOthersAsync(query);
            return result;
        }, "查詢採購其他功能列表失敗");
    }

    /// <summary>
    /// 查詢單筆採購其他功能（根據主鍵）
    /// </summary>
    [HttpGet("{tKey}")]
    public async Task<ActionResult<ApiResponse<ProcurementOtherDto>>> GetProcurementOther(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetProcurementOtherByTKeyAsync(tKey);
            return result;
        }, $"查詢採購其他功能失敗: {tKey}");
    }

    /// <summary>
    /// 查詢單筆採購其他功能（根據功能代碼）
    /// </summary>
    [HttpGet("by-id/{functionId}")]
    public async Task<ActionResult<ApiResponse<ProcurementOtherDto>>> GetProcurementOtherByFunctionId(string functionId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetProcurementOtherByFunctionIdAsync(functionId);
            return result;
        }, $"查詢採購其他功能失敗: {functionId}");
    }

    /// <summary>
    /// 新增採購其他功能
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<long>>> CreateProcurementOther(
        [FromBody] CreateProcurementOtherDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var tKey = await _service.CreateProcurementOtherAsync(dto);
            return tKey;
        }, "新增採購其他功能失敗");
    }

    /// <summary>
    /// 修改採購其他功能
    /// </summary>
    [HttpPut("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateProcurementOther(
        long tKey,
        [FromBody] UpdateProcurementOtherDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateProcurementOtherAsync(tKey, dto);
        }, $"修改採購其他功能失敗: {tKey}");
    }

    /// <summary>
    /// 刪除採購其他功能
    /// </summary>
    [HttpDelete("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteProcurementOther(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteProcurementOtherAsync(tKey);
        }, $"刪除採購其他功能失敗: {tKey}");
    }

    /// <summary>
    /// 檢查採購其他功能是否存在
    /// </summary>
    [HttpGet("{functionId}/exists")]
    public async Task<ActionResult<ApiResponse<bool>>> CheckProcurementOtherExists(string functionId)
    {
        return await ExecuteAsync(async () =>
        {
            var exists = await _service.ExistsAsync(functionId);
            return exists;
        }, $"檢查採購其他功能是否存在失敗: {functionId}");
    }
}

