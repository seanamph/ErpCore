using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.OtherManagement;
using ErpCore.Application.Services.OtherManagement;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.OtherManagement;

/// <summary>
/// S系統功能維護控制器 (SYSS000)
/// </summary>
[Route("api/v1/system-s-functions")]
public class SystemSController : BaseController
{
    private readonly ISYSSFunctionService _service;

    public SystemSController(
        ISYSSFunctionService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢S系統功能列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<SYSSFunctionDto>>>> GetSYSSFunctions(
        [FromQuery] SYSSFunctionQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSYSSFunctionsAsync(query);
            return result;
        }, "查詢S系統功能列表失敗");
    }

    /// <summary>
    /// 查詢單筆S系統功能（根據主鍵）
    /// </summary>
    [HttpGet("{tKey}")]
    public async Task<ActionResult<ApiResponse<SYSSFunctionDto>>> GetSYSSFunction(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSYSSFunctionByIdAsync(tKey);
            return result;
        }, $"查詢S系統功能失敗: {tKey}");
    }

    /// <summary>
    /// 查詢單筆S系統功能（根據功能代碼）
    /// </summary>
    [HttpGet("by-id/{functionId}")]
    public async Task<ActionResult<ApiResponse<SYSSFunctionDto>>> GetSYSSFunctionByFunctionId(string functionId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSYSSFunctionByFunctionIdAsync(functionId);
            return result;
        }, $"查詢S系統功能失敗: {functionId}");
    }

    /// <summary>
    /// 新增S系統功能
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<long>>> CreateSYSSFunction(
        [FromBody] CreateSYSSFunctionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateSYSSFunctionAsync(dto);
            return result;
        }, "新增S系統功能失敗");
    }

    /// <summary>
    /// 修改S系統功能
    /// </summary>
    [HttpPut("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateSYSSFunction(
        long tKey,
        [FromBody] UpdateSYSSFunctionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateSYSSFunctionAsync(tKey, dto);
            return new { TKey = tKey };
        }, $"修改S系統功能失敗: {tKey}");
    }

    /// <summary>
    /// 刪除S系統功能
    /// </summary>
    [HttpDelete("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteSYSSFunction(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteSYSSFunctionAsync(tKey);
            return new { TKey = tKey };
        }, $"刪除S系統功能失敗: {tKey}");
    }

    /// <summary>
    /// 啟用/停用S系統功能
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
        }, $"更新S系統功能狀態失敗: {tKey}");
    }
}

