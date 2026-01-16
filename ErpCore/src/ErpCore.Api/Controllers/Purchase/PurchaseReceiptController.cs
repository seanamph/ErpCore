using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Purchase;
using ErpCore.Application.Services.Purchase;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Purchase;

/// <summary>
/// 採購單驗收作業控制器 (SYSW324)
/// </summary>
[Route("api/v1/purchase-receipts")]
public class PurchaseReceiptController : BaseController
{
    private readonly IPurchaseReceiptService _service;

    public PurchaseReceiptController(
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
        }, "查詢待驗收採購單失敗 (SYSW324)");
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
            // 自動過濾 SYSW324 的資料
            query.SourceProgram = "SYSW324";
            var result = await _service.GetPurchaseReceiptsAsync(query);
            return result;
        }, "查詢驗收單列表失敗 (SYSW324)");
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
        }, $"查詢驗收單失敗: {receiptId}");
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
        }, $"依採購單建立驗收單失敗: {orderId}");
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
        }, "新增驗收單失敗");
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
        }, $"修改驗收單失敗: {receiptId}");
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
    public async Task<ActionResult<ApiResponse<object>>> CancelPurchaseReceipt(string receiptId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.CancelPurchaseReceiptAsync(receiptId);
        }, $"取消驗收單失敗: {receiptId}");
    }

    // ========== SYSW333 - 已日結採購單驗收調整作業 ==========

    /// <summary>
    /// 查詢已日結採購單驗收調整列表
    /// </summary>
    [HttpGet("settled-adjustments")]
    public async Task<ActionResult<ApiResponse<PagedResult<PurchaseReceiptDto>>>> GetSettledAdjustments(
        [FromQuery] SettledPurchaseReceiptAdjustmentQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSettledAdjustmentsAsync(query);
            return result;
        }, "查詢已日結採購單驗收調整列表失敗");
    }

    /// <summary>
    /// 查詢單筆已日結採購單驗收調整
    /// </summary>
    [HttpGet("settled-adjustments/{receiptId}")]
    public async Task<ActionResult<ApiResponse<PurchaseReceiptFullDto>>> GetSettledAdjustment(string receiptId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSettledAdjustmentByIdAsync(receiptId);
            return result;
        }, $"查詢已日結採購單驗收調整失敗: {receiptId}");
    }

    /// <summary>
    /// 查詢可用已日結採購單
    /// </summary>
    [HttpGet("settled-orders")]
    public async Task<ActionResult<ApiResponse<List<PurchaseOrderDto>>>> GetSettledOrders(
        [FromQuery] SettledOrderQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSettledOrdersAsync(query);
            return result;
        }, "查詢可用已日結採購單失敗");
    }

    /// <summary>
    /// 新增已日結採購單驗收調整
    /// </summary>
    [HttpPost("settled-adjustments")]
    public async Task<ActionResult<ApiResponse<string>>> CreateSettledAdjustment(
        [FromBody] CreatePurchaseReceiptDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var receiptId = await _service.CreateSettledAdjustmentAsync(dto);
            return receiptId;
        }, "新增已日結採購單驗收調整失敗");
    }

    /// <summary>
    /// 修改已日結採購單驗收調整
    /// </summary>
    [HttpPut("settled-adjustments/{receiptId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateSettledAdjustment(
        string receiptId,
        [FromBody] UpdatePurchaseReceiptDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateSettledAdjustmentAsync(receiptId, dto);
        }, $"修改已日結採購單驗收調整失敗: {receiptId}");
    }

    /// <summary>
    /// 刪除已日結採購單驗收調整
    /// </summary>
    [HttpDelete("settled-adjustments/{receiptId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteSettledAdjustment(string receiptId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteSettledAdjustmentAsync(receiptId);
        }, $"刪除已日結採購單驗收調整失敗: {receiptId}");
    }

    /// <summary>
    /// 審核已日結採購單驗收調整
    /// </summary>
    [HttpPost("settled-adjustments/{receiptId}/approve")]
    public async Task<ActionResult<ApiResponse<object>>> ApproveSettledAdjustment(
        string receiptId,
        [FromBody] ApproveSettledAdjustmentDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.ApproveSettledAdjustmentAsync(receiptId, dto);
        }, $"審核已日結採購單驗收調整失敗: {receiptId}");
    }

    // ========== SYSW530 - 已日結退貨單驗退調整作業 ==========

    /// <summary>
    /// 查詢已日結退貨單驗退調整列表
    /// </summary>
    [HttpGet("closed-return-adjustments")]
    public async Task<ActionResult<ApiResponse<PagedResult<PurchaseReceiptDto>>>> GetClosedReturnAdjustments(
        [FromQuery] ClosedReturnAdjustmentQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetClosedReturnAdjustmentsAsync(query);
            return result;
        }, "查詢已日結退貨單驗退調整列表失敗");
    }

    /// <summary>
    /// 查詢單筆已日結退貨單驗退調整
    /// </summary>
    [HttpGet("closed-return-adjustments/{receiptId}")]
    public async Task<ActionResult<ApiResponse<PurchaseReceiptFullDto>>> GetClosedReturnAdjustment(string receiptId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetClosedReturnAdjustmentByIdAsync(receiptId);
            return result;
        }, $"查詢已日結退貨單驗退調整失敗: {receiptId}");
    }

    /// <summary>
    /// 查詢可用的已日結退貨單
    /// </summary>
    [HttpGet("closed-return-orders")]
    public async Task<ActionResult<ApiResponse<List<PurchaseOrderDto>>>> GetClosedReturnOrders(
        [FromQuery] ClosedReturnOrderQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetClosedReturnOrdersAsync(query);
            return result;
        }, "查詢可用的已日結退貨單失敗");
    }

    /// <summary>
    /// 新增已日結退貨單驗退調整
    /// </summary>
    [HttpPost("closed-return-adjustments")]
    public async Task<ActionResult<ApiResponse<string>>> CreateClosedReturnAdjustment(
        [FromBody] CreatePurchaseReceiptDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var receiptId = await _service.CreateClosedReturnAdjustmentAsync(dto);
            return receiptId;
        }, "新增已日結退貨單驗退調整失敗");
    }

    /// <summary>
    /// 修改已日結退貨單驗退調整
    /// </summary>
    [HttpPut("closed-return-adjustments/{receiptId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateClosedReturnAdjustment(
        string receiptId,
        [FromBody] UpdatePurchaseReceiptDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateClosedReturnAdjustmentAsync(receiptId, dto);
        }, $"修改已日結退貨單驗退調整失敗: {receiptId}");
    }

    /// <summary>
    /// 刪除已日結退貨單驗退調整
    /// </summary>
    [HttpDelete("closed-return-adjustments/{receiptId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteClosedReturnAdjustment(string receiptId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteClosedReturnAdjustmentAsync(receiptId);
        }, $"刪除已日結退貨單驗退調整失敗: {receiptId}");
    }

    /// <summary>
    /// 審核已日結退貨單驗退調整
    /// </summary>
    [HttpPost("closed-return-adjustments/{receiptId}/approve")]
    public async Task<ActionResult<ApiResponse<object>>> ApproveClosedReturnAdjustment(
        string receiptId,
        [FromBody] ApproveSettledAdjustmentDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.ApproveClosedReturnAdjustmentAsync(receiptId, dto);
        }, $"審核已日結退貨單驗退調整失敗: {receiptId}");
    }

    // ========== SYSW337 - 已日結退貨單驗退調整作業 ==========

    /// <summary>
    /// 查詢已日結退貨單驗退調整列表 (SYSW337)
    /// </summary>
    [HttpGet("closed-return-adjustments-v2")]
    public async Task<ActionResult<ApiResponse<PagedResult<PurchaseReceiptDto>>>> GetClosedReturnAdjustmentsV2(
        [FromQuery] ClosedReturnAdjustmentQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetClosedReturnAdjustmentsV2Async(query);
            return result;
        }, "查詢已日結退貨單驗退調整列表失敗 (SYSW337)");
    }

    /// <summary>
    /// 查詢單筆已日結退貨單驗退調整 (SYSW337)
    /// </summary>
    [HttpGet("closed-return-adjustments-v2/{receiptId}")]
    public async Task<ActionResult<ApiResponse<PurchaseReceiptFullDto>>> GetClosedReturnAdjustmentV2(string receiptId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetClosedReturnAdjustmentV2ByIdAsync(receiptId);
            return result;
        }, $"查詢已日結退貨單驗退調整失敗 (SYSW337): {receiptId}");
    }

    /// <summary>
    /// 查詢可用的已日結退貨單 (SYSW337)
    /// </summary>
    [HttpGet("closed-return-orders-v2")]
    public async Task<ActionResult<ApiResponse<List<PurchaseOrderDto>>>> GetClosedReturnOrdersV2(
        [FromQuery] ClosedReturnOrderQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetClosedReturnOrdersV2Async(query);
            return result;
        }, "查詢可用的已日結退貨單失敗 (SYSW337)");
    }

    /// <summary>
    /// 新增已日結退貨單驗退調整 (SYSW337)
    /// </summary>
    [HttpPost("closed-return-adjustments-v2")]
    public async Task<ActionResult<ApiResponse<string>>> CreateClosedReturnAdjustmentV2(
        [FromBody] CreatePurchaseReceiptDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var receiptId = await _service.CreateClosedReturnAdjustmentV2Async(dto);
            return receiptId;
        }, "新增已日結退貨單驗退調整失敗 (SYSW337)");
    }

    /// <summary>
    /// 修改已日結退貨單驗退調整 (SYSW337)
    /// </summary>
    [HttpPut("closed-return-adjustments-v2/{receiptId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateClosedReturnAdjustmentV2(
        string receiptId,
        [FromBody] UpdatePurchaseReceiptDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateClosedReturnAdjustmentV2Async(receiptId, dto);
        }, $"修改已日結退貨單驗退調整失敗 (SYSW337): {receiptId}");
    }

    /// <summary>
    /// 刪除已日結退貨單驗退調整 (SYSW337)
    /// </summary>
    [HttpDelete("closed-return-adjustments-v2/{receiptId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteClosedReturnAdjustmentV2(string receiptId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteClosedReturnAdjustmentV2Async(receiptId);
        }, $"刪除已日結退貨單驗退調整失敗 (SYSW337): {receiptId}");
    }

    /// <summary>
    /// 審核已日結退貨單驗退調整 (SYSW337)
    /// </summary>
    [HttpPost("closed-return-adjustments-v2/{receiptId}/approve")]
    public async Task<ActionResult<ApiResponse<object>>> ApproveClosedReturnAdjustmentV2(
        string receiptId,
        [FromBody] ApproveSettledAdjustmentDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.ApproveClosedReturnAdjustmentV2Async(receiptId, dto);
        }, $"審核已日結退貨單驗退調整失敗 (SYSW337): {receiptId}");
    }
}

