using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.SystemExtension;
using ErpCore.Application.Services.SystemExtension;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.SystemExtension;

/// <summary>
/// 系統擴展資料維護控制器 (SYSX110)
/// </summary>
[Route("api/v1/system-extensions")]
public class SystemExtensionController : BaseController
{
    private readonly ISystemExtensionService _service;

    public SystemExtensionController(
        ISystemExtensionService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢系統擴展列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<SystemExtensionDto>>>> GetSystemExtensions(
        [FromQuery] SystemExtensionQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSystemExtensionsAsync(query);
            return result;
        }, "查詢系統擴展列表失敗");
    }

    /// <summary>
    /// 查詢單筆系統擴展（根據主鍵）
    /// </summary>
    [HttpGet("{tKey}")]
    public async Task<ActionResult<ApiResponse<SystemExtensionDto>>> GetSystemExtension(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSystemExtensionByTKeyAsync(tKey);
            return result;
        }, $"查詢系統擴展失敗: {tKey}");
    }

    /// <summary>
    /// 查詢單筆系統擴展（根據擴展功能代碼）
    /// </summary>
    [HttpGet("by-id/{extensionId}")]
    public async Task<ActionResult<ApiResponse<SystemExtensionDto>>> GetSystemExtensionByExtensionId(string extensionId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSystemExtensionByExtensionIdAsync(extensionId);
            return result;
        }, $"查詢系統擴展失敗: {extensionId}");
    }

    /// <summary>
    /// 新增系統擴展
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<long>>> CreateSystemExtension(
        [FromBody] CreateSystemExtensionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateSystemExtensionAsync(dto);
            return result;
        }, "新增系統擴展失敗");
    }

    /// <summary>
    /// 修改系統擴展
    /// </summary>
    [HttpPut("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateSystemExtension(
        long tKey,
        [FromBody] UpdateSystemExtensionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateSystemExtensionAsync(tKey, dto);
        }, $"修改系統擴展失敗: {tKey}");
    }

    /// <summary>
    /// 刪除系統擴展
    /// </summary>
    [HttpDelete("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteSystemExtension(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteSystemExtensionAsync(tKey);
        }, $"刪除系統擴展失敗: {tKey}");
    }
}

/// <summary>
/// 系統擴展查詢控制器 (SYSX120)
/// </summary>
[Route("api/v1/system-extensions")]
public class SystemExtensionQueryController : BaseController
{
    private readonly ISystemExtensionService _service;

    public SystemExtensionQueryController(
        ISystemExtensionService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢系統擴展統計資訊
    /// </summary>
    [HttpGet("statistics")]
    public async Task<ActionResult<ApiResponse<SystemExtensionStatisticsDto>>> GetStatistics(
        [FromQuery] SystemExtensionStatisticsQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetStatisticsAsync(query);
            return result;
        }, "查詢系統擴展統計失敗");
    }
}

