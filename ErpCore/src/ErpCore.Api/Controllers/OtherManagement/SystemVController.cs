using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.OtherManagement;
using ErpCore.Application.Services.OtherManagement;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.OtherManagement;

/// <summary>
/// V系統功能維護控制器 (SYSV000)
/// </summary>
[Route("api/v1/system-v-functions")]
public class SystemVController : BaseController
{
    private readonly ISYSVFunctionService _service;

    public SystemVController(
        ISYSVFunctionService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢V系統功能列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<SYSVFunctionDto>>>> GetSYSVFunctions(
        [FromQuery] SYSVFunctionQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSYSVFunctionsAsync(query);
            return result;
        }, "查詢V系統功能列表失敗");
    }

    /// <summary>
    /// 查詢單筆V系統功能（根據主鍵）
    /// </summary>
    [HttpGet("{tKey}")]
    public async Task<ActionResult<ApiResponse<SYSVFunctionDto>>> GetSYSVFunction(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSYSVFunctionByIdAsync(tKey);
            return result;
        }, $"查詢V系統功能失敗: {tKey}");
    }

    /// <summary>
    /// 查詢單筆V系統功能（根據功能代碼）
    /// </summary>
    [HttpGet("by-id/{functionId}")]
    public async Task<ActionResult<ApiResponse<SYSVFunctionDto>>> GetSYSVFunctionByFunctionId(string functionId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSYSVFunctionByFunctionIdAsync(functionId);
            return result;
        }, $"查詢V系統功能失敗: {functionId}");
    }

    /// <summary>
    /// 新增V系統功能
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<long>>> CreateSYSVFunction(
        [FromBody] CreateSYSVFunctionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateSYSVFunctionAsync(dto);
            return result;
        }, "新增V系統功能失敗");
    }

    /// <summary>
    /// 修改V系統功能
    /// </summary>
    [HttpPut("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateSYSVFunction(
        long tKey,
        [FromBody] UpdateSYSVFunctionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateSYSVFunctionAsync(tKey, dto);
            return new { TKey = tKey };
        }, $"修改V系統功能失敗: {tKey}");
    }

    /// <summary>
    /// 刪除V系統功能
    /// </summary>
    [HttpDelete("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteSYSVFunction(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteSYSVFunctionAsync(tKey);
            return new { TKey = tKey };
        }, $"刪除V系統功能失敗: {tKey}");
    }

    /// <summary>
    /// 啟用/停用V系統功能
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
        }, $"更新V系統功能狀態失敗: {tKey}");
    }
}

