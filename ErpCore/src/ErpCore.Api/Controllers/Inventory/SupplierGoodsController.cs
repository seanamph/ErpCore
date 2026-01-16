using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Inventory;
using ErpCore.Application.Services.Inventory;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Inventory;

/// <summary>
/// 供應商商品資料維護控制器 (SYSW110)
/// </summary>
[Route("api/v1/supplier-goods")]
public class SupplierGoodsController : BaseController
{
    private readonly ISupplierGoodsService _service;

    public SupplierGoodsController(
        ISupplierGoodsService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢供應商商品列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<SupplierGoodsDto>>>> GetSupplierGoods(
        [FromQuery] SupplierGoodsQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSupplierGoodsAsync(query);
            return result;
        }, "查詢供應商商品列表失敗");
    }

    /// <summary>
    /// 查詢單筆供應商商品
    /// </summary>
    [HttpGet("{supplierId}/{barcodeId}/{shopId}")]
    public async Task<ActionResult<ApiResponse<SupplierGoodsDto>>> GetSupplierGoods(
        string supplierId, 
        string barcodeId, 
        string shopId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSupplierGoodsByIdAsync(supplierId, barcodeId, shopId);
            return result;
        }, $"查詢供應商商品失敗: {supplierId}/{barcodeId}/{shopId}");
    }

    /// <summary>
    /// 新增供應商商品
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<object>>> CreateSupplierGoods(
        [FromBody] CreateSupplierGoodsDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.CreateSupplierGoodsAsync(dto);
        }, "新增供應商商品失敗");
    }

    /// <summary>
    /// 修改供應商商品
    /// </summary>
    [HttpPut("{supplierId}/{barcodeId}/{shopId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateSupplierGoods(
        string supplierId,
        string barcodeId,
        string shopId,
        [FromBody] UpdateSupplierGoodsDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateSupplierGoodsAsync(supplierId, barcodeId, shopId, dto);
        }, $"修改供應商商品失敗: {supplierId}/{barcodeId}/{shopId}");
    }

    /// <summary>
    /// 刪除供應商商品
    /// </summary>
    [HttpDelete("{supplierId}/{barcodeId}/{shopId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteSupplierGoods(
        string supplierId, 
        string barcodeId, 
        string shopId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteSupplierGoodsAsync(supplierId, barcodeId, shopId);
        }, $"刪除供應商商品失敗: {supplierId}/{barcodeId}/{shopId}");
    }

    /// <summary>
    /// 批次刪除供應商商品
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteSupplierGoodsBatch(
        [FromBody] BatchDeleteSupplierGoodsDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteSupplierGoodsBatchAsync(dto);
        }, "批次刪除供應商商品失敗");
    }

    /// <summary>
    /// 更新狀態
    /// </summary>
    [HttpPut("{supplierId}/{barcodeId}/{shopId}/status")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateStatus(
        string supplierId,
        string barcodeId,
        string shopId,
        [FromBody] UpdateSupplierGoodsStatusDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateStatusAsync(supplierId, barcodeId, shopId, dto.Status);
        }, $"更新供應商商品狀態失敗: {supplierId}/{barcodeId}/{shopId}");
    }
}
