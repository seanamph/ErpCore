using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Purchase;
using ErpCore.Application.Services.Purchase;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Purchase;

/// <summary>
/// 訂退貨申請作業控制器 V2 (SYSW316)
/// </summary>
[Route("api/v1/purchase-orders-v2")]
public class PurchaseOrdersV2Controller : BaseController
{
    private readonly IPurchaseOrderService _service;

    public PurchaseOrdersV2Controller(
        IPurchaseOrderService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢採購單列表 (SYSW316)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<PurchaseOrderDto>>>> GetPurchaseOrders(
        [FromQuery] PurchaseOrderQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            // 自動過濾 SYSW316 的資料
            query.SourceProgram = "SYSW316";
            var result = await _service.GetPurchaseOrdersAsync(query);
            return result;
        }, "查詢採購單列表失敗");
    }

    /// <summary>
    /// 查詢單筆採購單 (SYSW316)
    /// </summary>
    [HttpGet("{orderId}")]
    public async Task<ActionResult<ApiResponse<PurchaseOrderFullDto>>> GetPurchaseOrder(string orderId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPurchaseOrderByIdAsync(orderId);
            // 驗證是否為 SYSW316 的資料
            if (result != null && result.SourceProgram != "SYSW316")
            {
                throw new InvalidOperationException($"採購單 {orderId} 不屬於 SYSW316 功能");
            }
            return result;
        }, $"查詢採購單失敗: {orderId}");
    }

    /// <summary>
    /// 新增採購單 (SYSW316)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreatePurchaseOrder(
        [FromBody] CreatePurchaseOrderDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            // 強制設定為 SYSW316
            dto.SourceProgram = "SYSW316";
            var orderId = await _service.CreatePurchaseOrderAsync(dto);
            return orderId;
        }, "新增採購單失敗");
    }

    /// <summary>
    /// 修改採購單 (SYSW316)
    /// </summary>
    [HttpPut("{orderId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdatePurchaseOrder(
        string orderId,
        [FromBody] UpdatePurchaseOrderDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            // 驗證是否為 SYSW316 的資料
            var existing = await _service.GetPurchaseOrderByIdAsync(orderId);
            if (existing == null)
            {
                throw new InvalidOperationException($"採購單不存在: {orderId}");
            }
            if (existing.SourceProgram != "SYSW316")
            {
                throw new InvalidOperationException($"採購單 {orderId} 不屬於 SYSW316 功能");
            }
            await _service.UpdatePurchaseOrderAsync(orderId, dto);
        }, $"修改採購單失敗: {orderId}");
    }

    /// <summary>
    /// 刪除採購單 (SYSW316)
    /// </summary>
    [HttpDelete("{orderId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeletePurchaseOrder(string orderId)
    {
        return await ExecuteAsync(async () =>
        {
            // 驗證是否為 SYSW316 的資料
            var existing = await _service.GetPurchaseOrderByIdAsync(orderId);
            if (existing == null)
            {
                throw new InvalidOperationException($"採購單不存在: {orderId}");
            }
            if (existing.SourceProgram != "SYSW316")
            {
                throw new InvalidOperationException($"採購單 {orderId} 不屬於 SYSW316 功能");
            }
            await _service.DeletePurchaseOrderAsync(orderId);
        }, $"刪除採購單失敗: {orderId}");
    }

    /// <summary>
    /// 送出採購單 (SYSW316)
    /// </summary>
    [HttpPost("{orderId}/submit")]
    public async Task<ActionResult<ApiResponse<object>>> SubmitPurchaseOrder(string orderId)
    {
        return await ExecuteAsync(async () =>
        {
            // 驗證是否為 SYSW316 的資料
            var existing = await _service.GetPurchaseOrderByIdAsync(orderId);
            if (existing == null)
            {
                throw new InvalidOperationException($"採購單不存在: {orderId}");
            }
            if (existing.SourceProgram != "SYSW316")
            {
                throw new InvalidOperationException($"採購單 {orderId} 不屬於 SYSW316 功能");
            }
            await _service.SubmitPurchaseOrderAsync(orderId);
        }, $"送出採購單失敗: {orderId}");
    }

    /// <summary>
    /// 審核採購單 (SYSW316)
    /// </summary>
    [HttpPost("{orderId}/approve")]
    public async Task<ActionResult<ApiResponse<object>>> ApprovePurchaseOrder(string orderId)
    {
        return await ExecuteAsync(async () =>
        {
            // 驗證是否為 SYSW316 的資料
            var existing = await _service.GetPurchaseOrderByIdAsync(orderId);
            if (existing == null)
            {
                throw new InvalidOperationException($"採購單不存在: {orderId}");
            }
            if (existing.SourceProgram != "SYSW316")
            {
                throw new InvalidOperationException($"採購單 {orderId} 不屬於 SYSW316 功能");
            }
            await _service.ApprovePurchaseOrderAsync(orderId);
        }, $"審核採購單失敗: {orderId}");
    }

    /// <summary>
    /// 取消採購單 (SYSW316)
    /// </summary>
    [HttpPost("{orderId}/cancel")]
    public async Task<ActionResult<ApiResponse<object>>> CancelPurchaseOrder(string orderId)
    {
        return await ExecuteAsync(async () =>
        {
            // 驗證是否為 SYSW316 的資料
            var existing = await _service.GetPurchaseOrderByIdAsync(orderId);
            if (existing == null)
            {
                throw new InvalidOperationException($"採購單不存在: {orderId}");
            }
            if (existing.SourceProgram != "SYSW316")
            {
                throw new InvalidOperationException($"採購單 {orderId} 不屬於 SYSW316 功能");
            }
            await _service.CancelPurchaseOrderAsync(orderId);
        }, $"取消採購單失敗: {orderId}");
    }
}
