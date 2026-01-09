using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.StoreMember;
using ErpCore.Application.Services.StoreMember;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.StoreMember;

/// <summary>
/// 商店資料維護控制器 (SYS3000 - 商店資料維護)
/// </summary>
[Route("api/v1/stores")]
public class StoreController : BaseController
{
    private readonly IStoreService _service;

    public StoreController(
        IStoreService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢商店列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ShopDto>>>> GetShops(
        [FromQuery] ShopQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetShopsAsync(query);
            return result;
        }, "查詢商店列表失敗");
    }

    /// <summary>
    /// 查詢單筆商店
    /// </summary>
    [HttpGet("{shopId}")]
    public async Task<ActionResult<ApiResponse<ShopDto>>> GetShop(string shopId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetShopByIdAsync(shopId);
            return result;
        }, $"查詢商店失敗: {shopId}");
    }

    /// <summary>
    /// 新增商店
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateShop(
        [FromBody] CreateShopDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateShopAsync(dto);
            return result;
        }, "新增商店失敗");
    }

    /// <summary>
    /// 修改商店
    /// </summary>
    [HttpPut("{shopId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateShop(
        string shopId,
        [FromBody] UpdateShopDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateShopAsync(shopId, dto);
        }, $"修改商店失敗: {shopId}");
    }

    /// <summary>
    /// 刪除商店
    /// </summary>
    [HttpDelete("{shopId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteShop(string shopId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteShopAsync(shopId);
        }, $"刪除商店失敗: {shopId}");
    }

    /// <summary>
    /// 檢查商店編號是否存在
    /// </summary>
    [HttpGet("check/{shopId}")]
    public async Task<ActionResult<ApiResponse<bool>>> CheckShopExists(string shopId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.ExistsAsync(shopId);
            return result;
        }, $"檢查商店編號是否存在失敗: {shopId}");
    }

    /// <summary>
    /// 更新商店狀態
    /// </summary>
    [HttpPut("{shopId}/status")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateShopStatus(
        string shopId,
        [FromBody] UpdateShopStatusDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateStatusAsync(shopId, dto.Status);
        }, $"更新商店狀態失敗: {shopId}");
    }
}

