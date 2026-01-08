using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Lease;
using ErpCore.Application.Services.Lease;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Lease;

/// <summary>
/// 租賃資料維護控制器 (SYS8110-SYS8220)
/// </summary>
[ApiController]
[Route("api/v1/leases")]
public class LeaseDataController : BaseController
{
    private readonly ILeaseService _service;

    public LeaseDataController(
        ILeaseService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢租賃列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<LeaseDto>>>> GetLeases(
        [FromQuery] LeaseQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetLeasesAsync(query);
            return result;
        }, "查詢租賃列表失敗");
    }

    /// <summary>
    /// 查詢單筆租賃
    /// </summary>
    [HttpGet("{leaseId}")]
    public async Task<ActionResult<ApiResponse<LeaseDto>>> GetLease(
        string leaseId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetLeaseByIdAsync(leaseId);
            return result;
        }, $"查詢租賃失敗: {leaseId}");
    }

    /// <summary>
    /// 新增租賃
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<LeaseResultDto>>> CreateLease(
        [FromBody] CreateLeaseDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateLeaseAsync(dto);
            return result;
        }, "新增租賃失敗");
    }

    /// <summary>
    /// 修改租賃
    /// </summary>
    [HttpPut("{leaseId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateLease(
        string leaseId,
        [FromBody] UpdateLeaseDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateLeaseAsync(leaseId, dto);
        }, $"修改租賃失敗: {leaseId}");
    }

    /// <summary>
    /// 刪除租賃
    /// </summary>
    [HttpDelete("{leaseId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteLease(
        string leaseId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteLeaseAsync(leaseId);
        }, $"刪除租賃失敗: {leaseId}");
    }

    /// <summary>
    /// 批次刪除租賃
    /// </summary>
    [HttpPost("batch-delete")]
    public async Task<ActionResult<ApiResponse<object>>> BatchDeleteLeases(
        [FromBody] BatchDeleteLeaseDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.BatchDeleteLeasesAsync(dto);
        }, "批次刪除租賃失敗");
    }

    /// <summary>
    /// 更新租賃狀態
    /// </summary>
    [HttpPut("{leaseId}/status")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateLeaseStatus(
        string leaseId,
        [FromBody] UpdateLeaseStatusDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateLeaseStatusAsync(leaseId, dto);
        }, $"更新租賃狀態失敗: {leaseId}");
    }

    /// <summary>
    /// 檢查租賃是否存在
    /// </summary>
    [HttpGet("{leaseId}/exists")]
    public async Task<ActionResult<ApiResponse<bool>>> CheckLeaseExists(
        string leaseId)
    {
        return await ExecuteAsync(async () =>
        {
            var exists = await _service.ExistsAsync(leaseId);
            return exists;
        }, $"檢查租賃是否存在失敗: {leaseId}");
    }
}

