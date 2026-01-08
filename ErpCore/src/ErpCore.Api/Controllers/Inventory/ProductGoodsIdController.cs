using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Inventory;
using ErpCore.Application.Services.Inventory;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Inventory;

/// <summary>
/// 商品進銷碼維護控制器 (SYSW137)
/// </summary>
[Route("api/v1/products/goods-ids")]
public class ProductGoodsIdController : BaseController
{
    private readonly IProductGoodsIdService _service;

    public ProductGoodsIdController(
        IProductGoodsIdService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢商品進銷碼列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ProductGoodsIdDto>>>> GetProductGoodsIds(
        [FromQuery] ProductGoodsIdQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetProductGoodsIdsAsync(query);
            return result;
        }, "查詢商品進銷碼列表失敗");
    }

    /// <summary>
    /// 查詢單筆商品進銷碼
    /// </summary>
    [HttpGet("{goodsId}")]
    public async Task<ActionResult<ApiResponse<ProductGoodsIdDto>>> GetProductGoodsId(string goodsId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetProductGoodsIdByIdAsync(goodsId);
            return result;
        }, $"查詢商品進銷碼失敗: {goodsId}");
    }

    /// <summary>
    /// 新增商品進銷碼
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<object>>> CreateProductGoodsId(
        [FromBody] CreateProductGoodsIdDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var goodsId = await _service.CreateProductGoodsIdAsync(dto);
            return new { GoodsId = goodsId };
        }, "新增商品進銷碼失敗");
    }

    /// <summary>
    /// 修改商品進銷碼
    /// </summary>
    [HttpPut("{goodsId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateProductGoodsId(
        string goodsId,
        [FromBody] UpdateProductGoodsIdDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateProductGoodsIdAsync(goodsId, dto);
        }, $"修改商品進銷碼失敗: {goodsId}");
    }

    /// <summary>
    /// 刪除商品進銷碼
    /// </summary>
    [HttpDelete("{goodsId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteProductGoodsId(string goodsId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteProductGoodsIdAsync(goodsId);
        }, $"刪除商品進銷碼失敗: {goodsId}");
    }

    /// <summary>
    /// 批次刪除商品進銷碼
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ActionResult<ApiResponse<object>>> BatchDeleteProductGoodsId(
        [FromBody] BatchDeleteProductGoodsIdDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.BatchDeleteProductGoodsIdAsync(dto);
        }, "批次刪除商品進銷碼失敗");
    }

    /// <summary>
    /// 檢查商品進銷碼是否存在
    /// </summary>
    [HttpGet("{goodsId}/exists")]
    public async Task<ActionResult<ApiResponse<bool>>> CheckProductGoodsIdExists(string goodsId)
    {
        return await ExecuteAsync(async () =>
        {
            var exists = await _service.ExistsAsync(goodsId);
            return exists;
        }, $"檢查商品進銷碼是否存在失敗: {goodsId}");
    }

    /// <summary>
    /// 更新商品進銷碼狀態
    /// </summary>
    [HttpPut("{goodsId}/status")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateStatus(
        string goodsId,
        [FromBody] UpdateProductGoodsIdStatusDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateStatusAsync(goodsId, dto);
        }, $"更新商品進銷碼狀態失敗: {goodsId}");
    }
}

