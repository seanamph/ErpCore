using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Contract;
using ErpCore.Application.Services.Contract;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Contract;

/// <summary>
/// 合同資料維護控制器 (SYSF110-SYSF140)
/// </summary>
[ApiController]
[Route("api/v1/contracts")]
public class ContractDataController : BaseController
{
    private readonly IContractService _service;

    public ContractDataController(
        IContractService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢合同列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ContractDto>>>> GetContracts(
        [FromQuery] ContractQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetContractsAsync(query);
            return result;
        }, "查詢合同列表失敗");
    }

    /// <summary>
    /// 查詢單筆合同
    /// </summary>
    [HttpGet("{contractId}/{version}")]
    public async Task<ActionResult<ApiResponse<ContractDto>>> GetContract(
        string contractId, 
        int version)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetContractByIdAsync(contractId, version);
            return result;
        }, $"查詢合同失敗: {contractId}, Version: {version}");
    }

    /// <summary>
    /// 新增合同
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ContractResultDto>>> CreateContract(
        [FromBody] CreateContractDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateContractAsync(dto);
            return result;
        }, "新增合同失敗");
    }

    /// <summary>
    /// 修改合同
    /// </summary>
    [HttpPut("{contractId}/{version}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateContract(
        string contractId,
        int version,
        [FromBody] UpdateContractDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateContractAsync(contractId, version, dto);
        }, $"修改合同失敗: {contractId}, Version: {version}");
    }

    /// <summary>
    /// 刪除合同
    /// </summary>
    [HttpDelete("{contractId}/{version}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteContract(
        string contractId, 
        int version)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteContractAsync(contractId, version);
        }, $"刪除合同失敗: {contractId}, Version: {version}");
    }

    /// <summary>
    /// 審核合同
    /// </summary>
    [HttpPut("{contractId}/{version}/approve")]
    public async Task<ActionResult<ApiResponse<object>>> ApproveContract(
        string contractId,
        int version,
        [FromBody] ApproveContractDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.ApproveContractAsync(contractId, version, dto);
        }, $"審核合同失敗: {contractId}, Version: {version}");
    }

    /// <summary>
    /// 產生新版本
    /// </summary>
    [HttpPost("{contractId}/{version}/new-version")]
    public async Task<ActionResult<ApiResponse<ContractResultDto>>> CreateNewVersion(
        string contractId,
        int version,
        [FromBody] NewVersionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateNewVersionAsync(contractId, version, dto);
            return result;
        }, $"產生合同新版本失敗: {contractId}, Version: {version}");
    }

    /// <summary>
    /// 檢查合同是否存在
    /// </summary>
    [HttpGet("{contractId}/{version}/exists")]
    public async Task<ActionResult<ApiResponse<bool>>> CheckContractExists(
        string contractId, 
        int version)
    {
        return await ExecuteAsync(async () =>
        {
            var exists = await _service.ExistsAsync(contractId, version);
            return exists;
        }, $"檢查合同是否存在失敗: {contractId}, Version: {version}");
    }
}

