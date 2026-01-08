using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Transfer;
using ErpCore.Application.Services.Transfer;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Transfer;

/// <summary>
/// 調撥單驗收作業控制器 (SYSW352)
/// </summary>
[Route("api/v1/transfer-receipt")]
public class TransferReceiptController : BaseController
{
    private readonly ITransferReceiptService _service;

    public TransferReceiptController(
        ITransferReceiptService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢待驗收調撥單列表
    /// </summary>
    [HttpGet("pending-orders")]
    public async Task<ActionResult<ApiResponse<PagedResult<PendingTransferOrderDto>>>> GetPendingOrders(
        [FromQuery] PendingTransferOrderQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPendingOrdersAsync(query);
            return result;
        }, "查詢待驗收調撥單失敗");
    }

    /// <summary>
    /// 查詢驗收單列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<TransferReceiptDto>>>> GetTransferReceipts(
        [FromQuery] TransferReceiptQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetTransferReceiptsAsync(query);
            return result;
        }, "查詢驗收單列表失敗");
    }

    /// <summary>
    /// 查詢單筆驗收單
    /// </summary>
    [HttpGet("{receiptId}")]
    public async Task<ActionResult<ApiResponse<TransferReceiptDto>>> GetTransferReceipt(string receiptId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetTransferReceiptByIdAsync(receiptId);
            return result;
        }, $"查詢驗收單失敗: {receiptId}");
    }

    /// <summary>
    /// 依調撥單號建立驗收單
    /// </summary>
    [HttpPost("from-order/{transferId}")]
    public async Task<ActionResult<ApiResponse<TransferReceiptDto>>> CreateReceiptFromOrder(string transferId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateReceiptFromOrderAsync(transferId);
            return result;
        }, $"依調撥單建立驗收單失敗: {transferId}");
    }

    /// <summary>
    /// 新增驗收單
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateTransferReceipt(
        [FromBody] CreateTransferReceiptDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var receiptId = await _service.CreateTransferReceiptAsync(dto);
            return receiptId;
        }, "新增驗收單失敗");
    }

    /// <summary>
    /// 修改驗收單
    /// </summary>
    [HttpPut("{receiptId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateTransferReceipt(
        string receiptId,
        [FromBody] UpdateTransferReceiptDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateTransferReceiptAsync(receiptId, dto);
        }, $"修改驗收單失敗: {receiptId}");
    }

    /// <summary>
    /// 刪除驗收單
    /// </summary>
    [HttpDelete("{receiptId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteTransferReceipt(string receiptId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteTransferReceiptAsync(receiptId);
        }, $"刪除驗收單失敗: {receiptId}");
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
        }, $"確認驗收失敗: {receiptId}");
    }

    /// <summary>
    /// 取消驗收單
    /// </summary>
    [HttpPost("{receiptId}/cancel")]
    public async Task<ActionResult<ApiResponse<object>>> CancelTransferReceipt(string receiptId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.CancelTransferReceiptAsync(receiptId);
        }, $"取消驗收單失敗: {receiptId}");
    }
}

