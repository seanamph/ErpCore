using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Lease;
using ErpCore.Application.Services.Lease;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Lease;

/// <summary>
/// 租賃處理作業控制器 (SYS8B50-SYS8B90)
/// </summary>
[ApiController]
[Route("api/v1/lease-processes")]
public class LeaseProcessController : BaseController
{
    private readonly ILeaseProcessService _service;

    public LeaseProcessController(
        ILeaseProcessService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢租賃處理列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<LeaseProcessDto>>>> GetLeaseProcesses(
        [FromQuery] LeaseProcessQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetLeaseProcessesAsync(query);
            return result;
        }, "查詢租賃處理列表失敗");
    }

    /// <summary>
    /// 查詢單筆租賃處理
    /// </summary>
    [HttpGet("{processId}")]
    public async Task<ActionResult<ApiResponse<LeaseProcessDto>>> GetLeaseProcess(
        string processId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetLeaseProcessByIdAsync(processId);
            return result;
        }, $"查詢租賃處理失敗: {processId}");
    }

    /// <summary>
    /// 根據租賃編號查詢處理列表
    /// </summary>
    [HttpGet("by-lease/{leaseId}")]
    public async Task<ActionResult<ApiResponse<List<LeaseProcessDto>>>> GetLeaseProcessesByLeaseId(
        string leaseId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetLeaseProcessesByLeaseIdAsync(leaseId);
            return result;
        }, $"根據租賃編號查詢處理列表失敗: {leaseId}");
    }

    /// <summary>
    /// 新增租賃處理
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateLeaseProcess(
        [FromBody] CreateLeaseProcessDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateLeaseProcessAsync(dto);
            return result;
        }, "新增租賃處理失敗");
    }

    /// <summary>
    /// 修改租賃處理
    /// </summary>
    [HttpPut("{processId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateLeaseProcess(
        string processId,
        [FromBody] UpdateLeaseProcessDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateLeaseProcessAsync(processId, dto);
        }, $"修改租賃處理失敗: {processId}");
    }

    /// <summary>
    /// 刪除租賃處理
    /// </summary>
    [HttpDelete("{processId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteLeaseProcess(
        string processId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteLeaseProcessAsync(processId);
        }, $"刪除租賃處理失敗: {processId}");
    }

    /// <summary>
    /// 更新租賃處理狀態
    /// </summary>
    [HttpPut("{processId}/status")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateLeaseProcessStatus(
        string processId,
        [FromBody] UpdateLeaseProcessStatusDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateLeaseProcessStatusAsync(processId, dto);
        }, $"更新租賃處理狀態失敗: {processId}");
    }

    /// <summary>
    /// 執行租賃處理
    /// </summary>
    [HttpPost("{processId}/execute")]
    public async Task<ActionResult<ApiResponse<object>>> ExecuteLeaseProcess(
        string processId,
        [FromBody] ExecuteLeaseProcessDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.ExecuteLeaseProcessAsync(processId, dto);
        }, $"執行租賃處理失敗: {processId}");
    }

    /// <summary>
    /// 審核租賃處理
    /// </summary>
    [HttpPost("{processId}/approve")]
    public async Task<ActionResult<ApiResponse<object>>> ApproveLeaseProcess(
        string processId,
        [FromBody] ApproveLeaseProcessDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.ApproveLeaseProcessAsync(processId, dto);
        }, $"審核租賃處理失敗: {processId}");
    }

    /// <summary>
    /// 檢查租賃處理是否存在
    /// </summary>
    [HttpGet("{processId}/exists")]
    public async Task<ActionResult<ApiResponse<bool>>> CheckLeaseProcessExists(
        string processId)
    {
        return await ExecuteAsync(async () =>
        {
            var exists = await _service.ExistsAsync(processId);
            return exists;
        }, $"檢查租賃處理是否存在失敗: {processId}");
    }
}

