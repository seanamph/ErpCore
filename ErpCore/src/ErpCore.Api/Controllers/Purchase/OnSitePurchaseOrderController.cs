using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Purchase;
using ErpCore.Application.Services.Purchase;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Purchase;

/// <summary>
/// 現場打單作業控制器 (SYSW322)
/// </summary>
[Route("api/v1/on-site-purchase-orders")]
public class OnSitePurchaseOrderController : BaseController
{
    private readonly IOnSitePurchaseOrderService _service;

    public OnSitePurchaseOrderController(
        IOnSitePurchaseOrderService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢現場打單申請單列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<PurchaseOrderDto>>>> GetOnSitePurchaseOrders(
        [FromQuery] PurchaseOrderQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetOnSitePurchaseOrdersAsync(query);
            return result;
        }, "查詢現場打單申請單列表失敗");
    }

    /// <summary>
    /// 查詢單筆現場打單申請單
    /// </summary>
    [HttpGet("{orderId}")]
    public async Task<ActionResult<ApiResponse<PurchaseOrderFullDto>>> GetOnSitePurchaseOrder(string orderId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetOnSitePurchaseOrderByIdAsync(orderId);
            return result;
        }, $"查詢現場打單申請單失敗: {orderId}");
    }

    /// <summary>
    /// 新增現場打單申請單
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateOnSitePurchaseOrder(
        [FromBody] CreatePurchaseOrderDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var orderId = await _service.CreateOnSitePurchaseOrderAsync(dto);
            return orderId;
        }, "新增現場打單申請單失敗");
    }

    /// <summary>
    /// 修改現場打單申請單
    /// </summary>
    [HttpPut("{orderId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateOnSitePurchaseOrder(
        string orderId,
        [FromBody] UpdatePurchaseOrderDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateOnSitePurchaseOrderAsync(orderId, dto);
        }, $"修改現場打單申請單失敗: {orderId}");
    }

    /// <summary>
    /// 刪除現場打單申請單
    /// </summary>
    [HttpDelete("{orderId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteOnSitePurchaseOrder(string orderId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteOnSitePurchaseOrderAsync(orderId);
        }, $"刪除現場打單申請單失敗: {orderId}");
    }

    /// <summary>
    /// 送出現場打單申請單
    /// </summary>
    [HttpPut("{orderId}/submit")]
    public async Task<ActionResult<ApiResponse<object>>> SubmitOnSitePurchaseOrder(string orderId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.SubmitOnSitePurchaseOrderAsync(orderId);
        }, $"送出現場打單申請單失敗: {orderId}");
    }

    /// <summary>
    /// 審核現場打單申請單
    /// </summary>
    [HttpPut("{orderId}/approve")]
    public async Task<ActionResult<ApiResponse<object>>> ApproveOnSitePurchaseOrder(string orderId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.ApproveOnSitePurchaseOrderAsync(orderId);
        }, $"審核現場打單申請單失敗: {orderId}");
    }

    /// <summary>
    /// 取消現場打單申請單
    /// </summary>
    [HttpPut("{orderId}/cancel")]
    public async Task<ActionResult<ApiResponse<object>>> CancelOnSitePurchaseOrder(string orderId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.CancelOnSitePurchaseOrderAsync(orderId);
        }, $"取消現場打單申請單失敗: {orderId}");
    }

    /// <summary>
    /// 根據條碼查詢商品資訊（現場打單專用）
    /// </summary>
    [HttpGet("goods-by-barcode")]
    public async Task<ActionResult<ApiResponse<GoodsByBarcodeDto>>> GetGoodsByBarcode([FromQuery] string barcode)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetGoodsByBarcodeAsync(barcode);
            return result;
        }, $"根據條碼查詢商品失敗: {barcode}");
    }
}
