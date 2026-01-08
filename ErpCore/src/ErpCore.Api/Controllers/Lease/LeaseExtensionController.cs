using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Lease;
using ErpCore.Application.Services.Lease;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Lease;

/// <summary>
/// 租賃擴展維護控制器 (SYS8A10-SYS8A45)
/// </summary>
[ApiController]
[Route("api/v1/lease-extensions")]
public class LeaseExtensionController : BaseController
{
    private readonly ILeaseExtensionService _service;

    public LeaseExtensionController(
        ILeaseExtensionService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢租賃擴展列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<LeaseExtensionDto>>>> GetLeaseExtensions(
        [FromQuery] LeaseExtensionQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetLeaseExtensionsAsync(query);
            return result;
        }, "查詢租賃擴展列表失敗");
    }

    /// <summary>
    /// 查詢單筆租賃擴展
    /// </summary>
    [HttpGet("{extensionId}")]
    public async Task<ActionResult<ApiResponse<LeaseExtensionDto>>> GetLeaseExtension(
        string extensionId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetLeaseExtensionByIdAsync(extensionId);
            return result;
        }, $"查詢租賃擴展失敗: {extensionId}");
    }

    /// <summary>
    /// 根據租賃編號查詢擴展列表
    /// </summary>
    [HttpGet("by-lease/{leaseId}")]
    public async Task<ActionResult<ApiResponse<List<LeaseExtensionDto>>>> GetLeaseExtensionsByLeaseId(
        string leaseId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetLeaseExtensionsByLeaseIdAsync(leaseId);
            return result;
        }, $"根據租賃編號查詢擴展列表失敗: {leaseId}");
    }

    /// <summary>
    /// 新增租賃擴展
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateLeaseExtension(
        [FromBody] CreateLeaseExtensionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateLeaseExtensionAsync(dto);
            return result;
        }, "新增租賃擴展失敗");
    }

    /// <summary>
    /// 修改租賃擴展
    /// </summary>
    [HttpPut("{extensionId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateLeaseExtension(
        string extensionId,
        [FromBody] UpdateLeaseExtensionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateLeaseExtensionAsync(extensionId, dto);
        }, $"修改租賃擴展失敗: {extensionId}");
    }

    /// <summary>
    /// 刪除租賃擴展
    /// </summary>
    [HttpDelete("{extensionId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteLeaseExtension(
        string extensionId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteLeaseExtensionAsync(extensionId);
        }, $"刪除租賃擴展失敗: {extensionId}");
    }

    /// <summary>
    /// 批次刪除租賃擴展
    /// </summary>
    [HttpPost("batch-delete")]
    public async Task<ActionResult<ApiResponse<object>>> BatchDeleteLeaseExtensions(
        [FromBody] BatchDeleteLeaseExtensionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.BatchDeleteLeaseExtensionsAsync(dto);
        }, "批次刪除租賃擴展失敗");
    }

    /// <summary>
    /// 更新租賃擴展狀態
    /// </summary>
    [HttpPut("{extensionId}/status")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateLeaseExtensionStatus(
        string extensionId,
        [FromBody] UpdateLeaseExtensionStatusDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateLeaseExtensionStatusAsync(extensionId, dto);
        }, $"更新租賃擴展狀態失敗: {extensionId}");
    }

    /// <summary>
    /// 檢查租賃擴展是否存在
    /// </summary>
    [HttpGet("{extensionId}/exists")]
    public async Task<ActionResult<ApiResponse<bool>>> CheckLeaseExtensionExists(
        string extensionId)
    {
        return await ExecuteAsync(async () =>
        {
            var exists = await _service.ExistsAsync(extensionId);
            return exists;
        }, $"檢查租賃擴展是否存在失敗: {extensionId}");
    }
}

