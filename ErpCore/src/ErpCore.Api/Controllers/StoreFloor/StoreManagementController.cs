using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.StoreFloor;
using ErpCore.Application.Services.StoreFloor;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.StoreFloor;

/// <summary>
/// 商店資料維護控制器 (SYS6110-SYS6140)
/// </summary>
[Route("api/v1/shop-floors")]
public class StoreManagementController : BaseController
{
    private readonly IShopFloorService _service;

    public StoreManagementController(
        IShopFloorService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢商店列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ShopFloorDto>>>> GetShopFloors(
        [FromQuery] ShopFloorQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetShopFloorsAsync(query);
            return result;
        }, "查詢商店列表失敗");
    }

    /// <summary>
    /// 查詢單筆商店
    /// </summary>
    [HttpGet("{shopId}")]
    public async Task<ActionResult<ApiResponse<ShopFloorDto>>> GetShopFloor(string shopId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetShopFloorByIdAsync(shopId);
            return result;
        }, $"查詢商店失敗: {shopId}");
    }

    /// <summary>
    /// 新增商店
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateShopFloor(
        [FromBody] CreateShopFloorDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateShopFloorAsync(dto);
            return result;
        }, "新增商店失敗");
    }

    /// <summary>
    /// 修改商店
    /// </summary>
    [HttpPut("{shopId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateShopFloor(
        string shopId,
        [FromBody] UpdateShopFloorDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateShopFloorAsync(shopId, dto);
        }, $"修改商店失敗: {shopId}");
    }

    /// <summary>
    /// 刪除商店
    /// </summary>
    [HttpDelete("{shopId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteShopFloor(string shopId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteShopFloorAsync(shopId);
        }, $"刪除商店失敗: {shopId}");
    }

    /// <summary>
    /// 檢查商店編號是否存在
    /// </summary>
    [HttpGet("check/{shopId}")]
    public async Task<ActionResult<ApiResponse<bool>>> CheckShopFloorExists(string shopId)
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
    public async Task<ActionResult<ApiResponse<object>>> UpdateShopFloorStatus(
        string shopId,
        [FromBody] UpdateShopFloorStatusDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateStatusAsync(shopId, dto.Status);
        }, $"更新商店狀態失敗: {shopId}");
    }
}

