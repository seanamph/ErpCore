using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Extension;
using ErpCore.Application.Services.Extension;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Extension;

/// <summary>
/// 擴展功能維護控制器 (SYS9000)
/// </summary>
[Route("api/v1/extension-functions")]
public class ExtensionController : BaseController
{
    private readonly IExtensionFunctionService _service;

    public ExtensionController(
        IExtensionFunctionService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢擴展功能列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ExtensionFunctionDto>>>> GetExtensionFunctions(
        [FromQuery] ExtensionFunctionQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetExtensionFunctionsAsync(query);
            return result;
        }, "查詢擴展功能列表失敗");
    }

    /// <summary>
    /// 查詢單筆擴展功能
    /// </summary>
    [HttpGet("{tKey}")]
    public async Task<ActionResult<ApiResponse<ExtensionFunctionDto>>> GetExtensionFunction(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetExtensionFunctionByIdAsync(tKey);
            return result;
        }, $"查詢擴展功能失敗: {tKey}");
    }

    /// <summary>
    /// 根據擴展代碼查詢
    /// </summary>
    [HttpGet("by-id/{extensionId}")]
    public async Task<ActionResult<ApiResponse<ExtensionFunctionDto>>> GetExtensionFunctionByExtensionId(string extensionId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetExtensionFunctionByExtensionIdAsync(extensionId);
            return result;
        }, $"查詢擴展功能失敗: {extensionId}");
    }

    /// <summary>
    /// 新增擴展功能
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<long>>> CreateExtensionFunction(
        [FromBody] CreateExtensionFunctionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateExtensionFunctionAsync(dto);
            return result;
        }, "新增擴展功能失敗");
    }

    /// <summary>
    /// 修改擴展功能
    /// </summary>
    [HttpPut("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateExtensionFunction(
        long tKey,
        [FromBody] UpdateExtensionFunctionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateExtensionFunctionAsync(tKey, dto);
            return new { TKey = tKey };
        }, $"修改擴展功能失敗: {tKey}");
    }

    /// <summary>
    /// 刪除擴展功能
    /// </summary>
    [HttpDelete("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteExtensionFunction(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteExtensionFunctionAsync(tKey);
            return new { TKey = tKey };
        }, $"刪除擴展功能失敗: {tKey}");
    }

    /// <summary>
    /// 批次新增擴展功能
    /// </summary>
    [HttpPost("batch")]
    public async Task<ActionResult<ApiResponse<object>>> BatchCreateExtensionFunctions(
        [FromBody] BatchCreateExtensionFunctionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.BatchCreateExtensionFunctionsAsync(dto);
            return new { Count = dto.Items.Count };
        }, "批次新增擴展功能失敗");
    }

    /// <summary>
    /// 啟用/停用擴展功能
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
        }, $"更新擴展功能狀態失敗: {tKey}");
    }
}

