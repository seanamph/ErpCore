using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Purchase;
using ErpCore.Application.Services.Purchase;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Purchase;

/// <summary>
/// 採購單驗收作業控制器 (SYSW336)
/// </summary>
[Route("api/v1/purchase-receipts-v2")]
public class PurchaseReceiptsV2Controller : BaseController
{
    private readonly IPurchaseReceiptService _service;

    public PurchaseReceiptsV2Controller(
        IPurchaseReceiptService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢待驗收採購單列表
    /// </summary>
    [HttpGet("pending-orders")]
    public async Task<ActionResult<ApiResponse<PagedResult<PendingPurchaseOrderDto>>>> GetPendingOrders(
        [FromQuery] PendingPurchaseOrderQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPendingOrdersAsync(query);
            return result;
        }, "查詢待驗收採購單失敗 (SYSW336)");
    }

    /// <summary>
    /// 查詢驗收單列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<PurchaseReceiptDto>>>> GetPurchaseReceipts(
        [FromQuery] PurchaseReceiptQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPurchaseReceiptsAsync(query);
            return result;
        }, "查詢驗收單列表失敗 (SYSW336)");
    }

    /// <summary>
    /// 查詢單筆驗收單
    /// </summary>
    [HttpGet("{receiptId}")]
    public async Task<ActionResult<ApiResponse<PurchaseReceiptFullDto>>> GetPurchaseReceipt(string receiptId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPurchaseReceiptByIdAsync(receiptId);
            return result;
        }, $"查詢驗收單失敗 (SYSW336): {receiptId}");
    }

    /// <summary>
    /// 依採購單號建立驗收單
    /// </summary>
    [HttpPost("from-order/{orderId}")]
    public async Task<ActionResult<ApiResponse<PurchaseReceiptFullDto>>> CreateReceiptFromOrder(string orderId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateReceiptFromOrderAsync(orderId);
            return result;
        }, $"依採購單建立驗收單失敗 (SYSW336): {orderId}");
    }

    /// <summary>
    /// 新增驗收單
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreatePurchaseReceipt(
        [FromBody] CreatePurchaseReceiptDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var receiptId = await _service.CreatePurchaseReceiptAsync(dto);
            return receiptId;
        }, "新增驗收單失敗 (SYSW336)");
    }

    /// <summary>
    /// 修改驗收單
    /// </summary>
    [HttpPut("{receiptId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdatePurchaseReceipt(
        string receiptId,
        [FromBody] UpdatePurchaseReceiptDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdatePurchaseReceiptAsync(receiptId, dto);
        }, $"修改驗收單失敗 (SYSW336): {receiptId}");
    }

    /// <summary>
    /// 刪除驗收單
    /// </summary>
    [HttpDelete("{receiptId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeletePurchaseReceipt(string receiptId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeletePurchaseReceiptAsync(receiptId);
        }, $"刪除驗收單失敗 (SYSW336): {receiptId}");
    }

    /// <summary>
    /// 確認驗收
    /// </summary>
    [HttpPost("{receiptId}/confirm")]
    public async Task<ActionResult<ApiResponse<object>>> ConfirmReceipt(string receiptId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.ConfirmReceiptAsync(receiptId);
        }, $"確認驗收失敗 (SYSW336): {receiptId}");
    }

    /// <summary>
    /// 取消驗收單
    /// </summary>
    [HttpPost("{receiptId}/cancel")]
    public async Task<ActionResult<ApiResponse<object>>> CancelPurchaseReceipt(string receiptId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.CancelPurchaseReceiptAsync(receiptId);
        }, $"取消驗收單失敗 (SYSW336): {receiptId}");
    }
}
