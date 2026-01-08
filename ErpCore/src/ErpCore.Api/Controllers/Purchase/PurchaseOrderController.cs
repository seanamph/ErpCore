using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Purchase;
using ErpCore.Application.Services.Purchase;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Purchase;

/// <summary>
/// 訂退貨申請作業控制器 (SYSW315)
/// </summary>
[Route("api/v1/purchase-orders")]
public class PurchaseOrderController : BaseController
{
    private readonly IPurchaseOrderService _service;

    public PurchaseOrderController(
        IPurchaseOrderService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢採購單列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<PurchaseOrderDto>>>> GetPurchaseOrders(
        [FromQuery] PurchaseOrderQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPurchaseOrdersAsync(query);
            return result;
        }, "查詢採購單列表失敗");
    }

    /// <summary>
    /// 查詢單筆採購單
    /// </summary>
    [HttpGet("{orderId}")]
    public async Task<ActionResult<ApiResponse<PurchaseOrderFullDto>>> GetPurchaseOrder(string orderId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPurchaseOrderByIdAsync(orderId);
            return result;
        }, $"查詢採購單失敗: {orderId}");
    }

    /// <summary>
    /// 新增採購單
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreatePurchaseOrder(
        [FromBody] CreatePurchaseOrderDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var orderId = await _service.CreatePurchaseOrderAsync(dto);
            return orderId;
        }, "新增採購單失敗");
    }

    /// <summary>
    /// 修改採購單
    /// </summary>
    [HttpPut("{orderId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdatePurchaseOrder(
        string orderId,
        [FromBody] UpdatePurchaseOrderDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdatePurchaseOrderAsync(orderId, dto);
        }, $"修改採購單失敗: {orderId}");
    }

    /// <summary>
    /// 刪除採購單
    /// </summary>
    [HttpDelete("{orderId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeletePurchaseOrder(string orderId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeletePurchaseOrderAsync(orderId);
        }, $"刪除採購單失敗: {orderId}");
    }

    /// <summary>
    /// 送出採購單
    /// </summary>
    [HttpPost("{orderId}/submit")]
    public async Task<ActionResult<ApiResponse<object>>> SubmitPurchaseOrder(string orderId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.SubmitPurchaseOrderAsync(orderId);
        }, $"送出採購單失敗: {orderId}");
    }

    /// <summary>
    /// 審核採購單
    /// </summary>
    [HttpPost("{orderId}/approve")]
    public async Task<ActionResult<ApiResponse<object>>> ApprovePurchaseOrder(string orderId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.ApprovePurchaseOrderAsync(orderId);
        }, $"審核採購單失敗: {orderId}");
    }

    /// <summary>
    /// 取消採購單
    /// </summary>
    [HttpPost("{orderId}/cancel")]
    public async Task<ActionResult<ApiResponse<object>>> CancelPurchaseOrder(string orderId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.CancelPurchaseOrderAsync(orderId);
        }, $"取消採購單失敗: {orderId}");
    }
}

