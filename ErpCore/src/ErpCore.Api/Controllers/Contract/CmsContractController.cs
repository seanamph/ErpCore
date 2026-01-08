using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Contract;
using ErpCore.Application.Services.Contract;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Contract;

/// <summary>
/// CMS合同維護控制器 (CMS2310-CMS2320)
/// </summary>
[ApiController]
[Route("api/v1/cms-contracts")]
public class CmsContractController : BaseController
{
    private readonly ICmsContractService _service;

    public CmsContractController(
        ICmsContractService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢CMS合同列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<CmsContractDto>>>> GetCmsContracts(
        [FromQuery] CmsContractQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetCmsContractsAsync(query);
            return result;
        }, "查詢CMS合同列表失敗");
    }

    /// <summary>
    /// 查詢單筆CMS合同（根據主鍵）
    /// </summary>
    [HttpGet("{tKey}")]
    public async Task<ActionResult<ApiResponse<CmsContractDto>>> GetCmsContract(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetCmsContractByIdAsync(tKey);
            return result;
        }, $"查詢CMS合同失敗: {tKey}");
    }

    /// <summary>
    /// 查詢單筆CMS合同（根據合同編號和版本）
    /// </summary>
    [HttpGet("{cmsContractId}/{version}")]
    public async Task<ActionResult<ApiResponse<CmsContractDto>>> GetCmsContractByContractId(
        string cmsContractId,
        int version)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetCmsContractByContractIdAsync(cmsContractId, version);
            return result;
        }, $"查詢CMS合同失敗: {cmsContractId}, Version: {version}");
    }

    /// <summary>
    /// 新增CMS合同
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CmsContractResultDto>>> CreateCmsContract(
        [FromBody] CreateCmsContractDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateCmsContractAsync(dto);
            return result;
        }, "新增CMS合同失敗");
    }

    /// <summary>
    /// 修改CMS合同
    /// </summary>
    [HttpPut("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateCmsContract(
        long tKey,
        [FromBody] UpdateCmsContractDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateCmsContractAsync(tKey, dto);
        }, $"修改CMS合同失敗: {tKey}");
    }

    /// <summary>
    /// 刪除CMS合同
    /// </summary>
    [HttpDelete("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteCmsContract(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteCmsContractAsync(tKey);
        }, $"刪除CMS合同失敗: {tKey}");
    }

    /// <summary>
    /// 批次刪除CMS合同
    /// </summary>
    [HttpPost("batch-delete")]
    public async Task<ActionResult<ApiResponse<int>>> BatchDeleteCmsContracts(
        [FromBody] BatchDeleteDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.BatchDeleteCmsContractsAsync(dto.TKeys);
            return result;
        }, "批次刪除CMS合同失敗");
    }

    /// <summary>
    /// 檢查CMS合同是否存在
    /// </summary>
    [HttpGet("{cmsContractId}/{version}/exists")]
    public async Task<ActionResult<ApiResponse<bool>>> CheckCmsContractExists(
        string cmsContractId,
        int version)
    {
        return await ExecuteAsync(async () =>
        {
            var exists = await _service.ExistsAsync(cmsContractId, version);
            return exists;
        }, $"檢查CMS合同是否存在失敗: {cmsContractId}, Version: {version}");
    }
}

