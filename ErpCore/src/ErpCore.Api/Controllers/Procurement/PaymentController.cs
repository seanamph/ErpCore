using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Procurement;
using ErpCore.Application.Services.Procurement;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Procurement;

/// <summary>
/// 付款管理控制器 (SYSP271-SYSP2B0)
/// </summary>
[ApiController]
[Route("api/v1/payments")]
public class PaymentController : BaseController
{
    private readonly IPaymentService _service;

    public PaymentController(
        IPaymentService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢付款單列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<PaymentDto>>>> GetPayments(
        [FromQuery] PaymentQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPaymentsAsync(query);
            return result;
        }, "查詢付款單列表失敗");
    }

    /// <summary>
    /// 查詢單筆付款單
    /// </summary>
    [HttpGet("{paymentId}")]
    public async Task<ActionResult<ApiResponse<PaymentDto>>> GetPayment(string paymentId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPaymentByIdAsync(paymentId);
            return result;
        }, $"查詢付款單失敗: {paymentId}");
    }

    /// <summary>
    /// 新增付款單
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreatePayment(
        [FromBody] CreatePaymentDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var paymentId = await _service.CreatePaymentAsync(dto);
            return paymentId;
        }, "新增付款單失敗");
    }

    /// <summary>
    /// 修改付款單
    /// </summary>
    [HttpPut("{paymentId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdatePayment(
        string paymentId,
        [FromBody] UpdatePaymentDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdatePaymentAsync(paymentId, dto);
        }, $"修改付款單失敗: {paymentId}");
    }

    /// <summary>
    /// 刪除付款單
    /// </summary>
    [HttpDelete("{paymentId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeletePayment(string paymentId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeletePaymentAsync(paymentId);
        }, $"刪除付款單失敗: {paymentId}");
    }

    /// <summary>
    /// 確認付款單
    /// </summary>
    [HttpPost("{paymentId}/confirm")]
    public async Task<ActionResult<ApiResponse<object>>> ConfirmPayment(string paymentId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.ConfirmPaymentAsync(paymentId);
        }, $"確認付款單失敗: {paymentId}");
    }

    /// <summary>
    /// 檢查付款單是否存在
    /// </summary>
    [HttpGet("{paymentId}/exists")]
    public async Task<ActionResult<ApiResponse<bool>>> CheckPaymentExists(string paymentId)
    {
        return await ExecuteAsync(async () =>
        {
            var exists = await _service.ExistsAsync(paymentId);
            return exists;
        }, $"檢查付款單是否存在失敗: {paymentId}");
    }
}

