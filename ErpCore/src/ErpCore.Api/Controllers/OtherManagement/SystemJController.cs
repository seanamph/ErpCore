using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.OtherManagement;
using ErpCore.Application.Services.OtherManagement;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.OtherManagement;

/// <summary>
/// J系統功能維護控制器 (SYSJ000)
/// </summary>
[Route("api/v1/system-j-functions")]
public class SystemJController : BaseController
{
    private readonly ISYSJFunctionService _service;

    public SystemJController(
        ISYSJFunctionService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢J系統功能列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<SYSJFunctionDto>>>> GetSYSJFunctions(
        [FromQuery] SYSJFunctionQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSYSJFunctionsAsync(query);
            return result;
        }, "查詢J系統功能列表失敗");
    }

    /// <summary>
    /// 查詢單筆J系統功能（根據主鍵）
    /// </summary>
    [HttpGet("{tKey}")]
    public async Task<ActionResult<ApiResponse<SYSJFunctionDto>>> GetSYSJFunction(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSYSJFunctionByIdAsync(tKey);
            return result;
        }, $"查詢J系統功能失敗: {tKey}");
    }

    /// <summary>
    /// 查詢單筆J系統功能（根據功能代碼）
    /// </summary>
    [HttpGet("by-id/{functionId}")]
    public async Task<ActionResult<ApiResponse<SYSJFunctionDto>>> GetSYSJFunctionByFunctionId(string functionId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSYSJFunctionByFunctionIdAsync(functionId);
            return result;
        }, $"查詢J系統功能失敗: {functionId}");
    }

    /// <summary>
    /// 新增J系統功能
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<long>>> CreateSYSJFunction(
        [FromBody] CreateSYSJFunctionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateSYSJFunctionAsync(dto);
            return result;
        }, "新增J系統功能失敗");
    }

    /// <summary>
    /// 修改J系統功能
    /// </summary>
    [HttpPut("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateSYSJFunction(
        long tKey,
        [FromBody] UpdateSYSJFunctionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateSYSJFunctionAsync(tKey, dto);
            return new { TKey = tKey };
        }, $"修改J系統功能失敗: {tKey}");
    }

    /// <summary>
    /// 刪除J系統功能
    /// </summary>
    [HttpDelete("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteSYSJFunction(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteSYSJFunctionAsync(tKey);
            return new { TKey = tKey };
        }, $"刪除J系統功能失敗: {tKey}");
    }

    /// <summary>
    /// 啟用/停用J系統功能
    /// </summary>
    [HttpPut("{tKey}/status")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateStatus(
        long tKey,
        [FromBody] UpdateStatusDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateStatusAsync(tKey, dto.Status);
            return new { TKey = tKey, Status = dto.Status };
        }, $"更新J系統功能狀態失敗: {tKey}");
    }
}

