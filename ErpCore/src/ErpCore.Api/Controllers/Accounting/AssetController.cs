using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Accounting;
using ErpCore.Application.Services.Accounting;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Accounting;

/// <summary>
/// 資產管理控制器 (SYSN310)
/// 提供資產資料的新增、修改、刪除、查詢功能
/// </summary>
[Route("api/v1/accounting/assets")]
public class AssetController : BaseController
{
    private readonly IAssetService _service;

    public AssetController(
        IAssetService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢資產列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<AssetDto>>>> GetAssets(
        [FromQuery] AssetQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetAssetsAsync(query);
            return result;
        }, "查詢資產列表失敗");
    }

    /// <summary>
    /// 根據資產編號查詢資產
    /// </summary>
    [HttpGet("{assetId}")]
    public async Task<ActionResult<ApiResponse<AssetDto>>> GetAsset(string assetId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetAssetByIdAsync(assetId);
            return result;
        }, "查詢資產失敗");
    }

    /// <summary>
    /// 新增資產
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateAsset(
        [FromBody] CreateAssetDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateAssetAsync(dto);
            return result;
        }, "新增資產失敗");
    }

    /// <summary>
    /// 修改資產
    /// </summary>
    [HttpPut("{assetId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateAsset(
        string assetId,
        [FromBody] UpdateAssetDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateAssetAsync(assetId, dto);
        }, "修改資產失敗");
    }

    /// <summary>
    /// 刪除資產
    /// </summary>
    [HttpDelete("{assetId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteAsset(string assetId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteAssetAsync(assetId);
        }, "刪除資產失敗");
    }
}

