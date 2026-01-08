using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Contract;
using ErpCore.Application.Services.Contract;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Contract;

/// <summary>
/// 合同擴展維護控制器 (SYSF350-SYSF540)
/// </summary>
[ApiController]
[Route("api/v1/contracts/extensions")]
public class ContractExtensionController : BaseController
{
    private readonly IContractExtensionService _service;

    public ContractExtensionController(
        IContractExtensionService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢合同擴展列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ContractExtensionDto>>>> GetContractExtensions(
        [FromQuery] ContractExtensionQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetContractExtensionsAsync(query);
            return result;
        }, "查詢合同擴展列表失敗");
    }

    /// <summary>
    /// 查詢單筆合同擴展
    /// </summary>
    [HttpGet("{tKey}")]
    public async Task<ActionResult<ApiResponse<ContractExtensionDto>>> GetContractExtension(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetContractExtensionByIdAsync(tKey);
            return result;
        }, $"查詢合同擴展失敗: {tKey}");
    }

    /// <summary>
    /// 新增合同擴展
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ContractExtensionResultDto>>> CreateContractExtension(
        [FromBody] CreateContractExtensionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateContractExtensionAsync(dto);
            return result;
        }, "新增合同擴展失敗");
    }

    /// <summary>
    /// 修改合同擴展
    /// </summary>
    [HttpPut("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateContractExtension(
        long tKey,
        [FromBody] UpdateContractExtensionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateContractExtensionAsync(tKey, dto);
        }, $"修改合同擴展失敗: {tKey}");
    }

    /// <summary>
    /// 刪除合同擴展
    /// </summary>
    [HttpDelete("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteContractExtension(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteContractExtensionAsync(tKey);
        }, $"刪除合同擴展失敗: {tKey}");
    }

    /// <summary>
    /// 批次刪除合同擴展
    /// </summary>
    [HttpPost("batch-delete")]
    public async Task<ActionResult<ApiResponse<int>>> BatchDeleteContractExtensions(
        [FromBody] BatchDeleteDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.BatchDeleteContractExtensionsAsync(dto.TKeys);
            return result;
        }, "批次刪除合同擴展失敗");
    }

    /// <summary>
    /// 檢查合同擴展是否存在
    /// </summary>
    [HttpGet("{tKey}/exists")]
    public async Task<ActionResult<ApiResponse<bool>>> CheckContractExtensionExists(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var exists = await _service.ExistsAsync(tKey);
            return exists;
        }, $"檢查合同擴展是否存在失敗: {tKey}");
    }
}

