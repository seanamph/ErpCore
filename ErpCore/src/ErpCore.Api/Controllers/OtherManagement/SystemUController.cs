using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.OtherManagement;
using ErpCore.Application.Services.OtherManagement;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.OtherManagement;

/// <summary>
/// U系統功能維護控制器 (SYSU000)
/// </summary>
[Route("api/v1/system-u-functions")]
public class SystemUController : BaseController
{
    private readonly ISYSUFunctionService _service;

    public SystemUController(
        ISYSUFunctionService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢U系統功能列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<SYSUFunctionDto>>>> GetSYSUFunctions(
        [FromQuery] SYSUFunctionQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSYSUFunctionsAsync(query);
            return result;
        }, "查詢U系統功能列表失敗");
    }

    /// <summary>
    /// 查詢單筆U系統功能（根據主鍵）
    /// </summary>
    [HttpGet("{tKey}")]
    public async Task<ActionResult<ApiResponse<SYSUFunctionDto>>> GetSYSUFunction(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSYSUFunctionByIdAsync(tKey);
            return result;
        }, $"查詢U系統功能失敗: {tKey}");
    }

    /// <summary>
    /// 查詢單筆U系統功能（根據功能代碼）
    /// </summary>
    [HttpGet("by-id/{functionId}")]
    public async Task<ActionResult<ApiResponse<SYSUFunctionDto>>> GetSYSUFunctionByFunctionId(string functionId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSYSUFunctionByFunctionIdAsync(functionId);
            return result;
        }, $"查詢U系統功能失敗: {functionId}");
    }

    /// <summary>
    /// 新增U系統功能
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<long>>> CreateSYSUFunction(
        [FromBody] CreateSYSUFunctionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateSYSUFunctionAsync(dto);
            return result;
        }, "新增U系統功能失敗");
    }

    /// <summary>
    /// 修改U系統功能
    /// </summary>
    [HttpPut("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateSYSUFunction(
        long tKey,
        [FromBody] UpdateSYSUFunctionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateSYSUFunctionAsync(tKey, dto);
            return new { TKey = tKey };
        }, $"修改U系統功能失敗: {tKey}");
    }

    /// <summary>
    /// 刪除U系統功能
    /// </summary>
    [HttpDelete("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteSYSUFunction(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteSYSUFunctionAsync(tKey);
            return new { TKey = tKey };
        }, $"刪除U系統功能失敗: {tKey}");
    }

    /// <summary>
    /// 啟用/停用U系統功能
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
        }, $"更新U系統功能狀態失敗: {tKey}");
    }
}

