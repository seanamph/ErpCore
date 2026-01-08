using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Purchase;
using ErpCore.Application.Services.Purchase;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Procurement;

/// <summary>
/// 採購管理控制器 (SYSP110-SYSP190)
/// 提供採購單維護功能
/// </summary>
[ApiController]
[Route("api/v1/procurement")]
public class ProcurementController : BaseController
{
    private readonly IPurchaseOrderService _service;

    public ProcurementController(
        IPurchaseOrderService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢採購單列表 (SYSP110_FQ)
    /// </summary>
    [HttpGet("orders")]
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
    /// 查詢單筆採購單 (SYSP110_FQ)
    /// </summary>
    [HttpGet("orders/{orderId}")]
    public async Task<ActionResult<ApiResponse<PurchaseOrderFullDto>>> GetPurchaseOrder(string orderId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPurchaseOrderByIdAsync(orderId);
            return result;
        }, $"查詢採購單失敗: {orderId}");
    }

    /// <summary>
    /// 新增採購單 (SYSP110_FI)
    /// </summary>
    [HttpPost("orders")]
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
    /// 修改採購單 (SYSP110_FU)
    /// </summary>
    [HttpPut("orders/{orderId}")]
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
    /// 刪除採購單 (SYSP110_FD)
    /// </summary>
    [HttpDelete("orders/{orderId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeletePurchaseOrder(string orderId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeletePurchaseOrderAsync(orderId);
        }, $"刪除採購單失敗: {orderId}");
    }

    /// <summary>
    /// 批次刪除採購單 (SYSP110_FD)
    /// </summary>
    [HttpDelete("orders/batch")]
    public async Task<ActionResult<ApiResponse<object>>> BatchDeletePurchaseOrders(
        [FromBody] BatchDeletePurchaseOrdersDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            foreach (var orderId in dto.OrderIds)
            {
                await _service.DeletePurchaseOrderAsync(orderId);
            }
        }, "批次刪除採購單失敗");
    }

    /// <summary>
    /// 送出採購單 (SYSP110)
    /// </summary>
    [HttpPost("orders/{orderId}/submit")]
    public async Task<ActionResult<ApiResponse<object>>> SubmitPurchaseOrder(string orderId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.SubmitPurchaseOrderAsync(orderId);
        }, $"送出採購單失敗: {orderId}");
    }

    /// <summary>
    /// 審核採購單 (SYSP110)
    /// </summary>
    [HttpPost("orders/{orderId}/approve")]
    public async Task<ActionResult<ApiResponse<object>>> ApprovePurchaseOrder(string orderId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.ApprovePurchaseOrderAsync(orderId);
        }, $"審核採購單失敗: {orderId}");
    }

    /// <summary>
    /// 取消採購單 (SYSP110)
    /// </summary>
    [HttpPost("orders/{orderId}/cancel")]
    public async Task<ActionResult<ApiResponse<object>>> CancelPurchaseOrder(string orderId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.CancelPurchaseOrderAsync(orderId);
        }, $"取消採購單失敗: {orderId}");
    }

    /// <summary>
    /// 結案採購單 (SYSP110)
    /// </summary>
    [HttpPost("orders/{orderId}/close")]
    public async Task<ActionResult<ApiResponse<object>>> ClosePurchaseOrder(string orderId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.ClosePurchaseOrderAsync(orderId);
        }, $"結案採購單失敗: {orderId}");
    }
}

/// <summary>
/// 批次刪除採購單 DTO
/// </summary>
public class BatchDeletePurchaseOrdersDto
{
    public List<string> OrderIds { get; set; } = new();
}

