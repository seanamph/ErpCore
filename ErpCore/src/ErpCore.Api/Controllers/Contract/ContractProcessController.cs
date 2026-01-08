using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Contract;
using ErpCore.Application.Services.Contract;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Contract;

/// <summary>
/// 合同處理作業控制器 (SYSF210-SYSF220)
/// </summary>
[ApiController]
[Route("api/v1/contract-processes")]
public class ContractProcessController : BaseController
{
    private readonly IContractProcessService _service;

    public ContractProcessController(
        IContractProcessService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢合同處理列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ContractProcessDto>>>> GetContractProcesses(
        [FromQuery] ContractProcessQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetContractProcessesAsync(query);
            return result;
        }, "查詢合同處理列表失敗");
    }

    /// <summary>
    /// 查詢單筆合同處理
    /// </summary>
    [HttpGet("{processId}")]
    public async Task<ActionResult<ApiResponse<ContractProcessDto>>> GetContractProcess(string processId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetContractProcessByIdAsync(processId);
            return result;
        }, $"查詢合同處理失敗: {processId}");
    }

    /// <summary>
    /// 新增合同處理
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ContractProcessResultDto>>> CreateContractProcess(
        [FromBody] CreateContractProcessDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateContractProcessAsync(dto);
            return result;
        }, "新增合同處理失敗");
    }

    /// <summary>
    /// 修改合同處理
    /// </summary>
    [HttpPut("{processId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateContractProcess(
        string processId,
        [FromBody] UpdateContractProcessDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateContractProcessAsync(processId, dto);
        }, $"修改合同處理失敗: {processId}");
    }

    /// <summary>
    /// 刪除合同處理
    /// </summary>
    [HttpDelete("{processId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteContractProcess(string processId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteContractProcessAsync(processId);
        }, $"刪除合同處理失敗: {processId}");
    }

    /// <summary>
    /// 完成合同處理
    /// </summary>
    [HttpPut("{processId}/complete")]
    public async Task<ActionResult<ApiResponse<object>>> CompleteContractProcess(string processId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.CompleteContractProcessAsync(processId);
        }, $"完成合同處理失敗: {processId}");
    }

    /// <summary>
    /// 取消合同處理
    /// </summary>
    [HttpPut("{processId}/cancel")]
    public async Task<ActionResult<ApiResponse<object>>> CancelContractProcess(string processId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.CancelContractProcessAsync(processId);
        }, $"取消合同處理失敗: {processId}");
    }

    /// <summary>
    /// 檢查合同處理是否存在
    /// </summary>
    [HttpGet("{processId}/exists")]
    public async Task<ActionResult<ApiResponse<bool>>> CheckContractProcessExists(string processId)
    {
        return await ExecuteAsync(async () =>
        {
            var exists = await _service.ExistsAsync(processId);
            return exists;
        }, $"檢查合同處理是否存在失敗: {processId}");
    }
}

