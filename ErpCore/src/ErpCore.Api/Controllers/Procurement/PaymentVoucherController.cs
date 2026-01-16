using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Procurement;
using ErpCore.Application.Services.Procurement;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Procurement;

/// <summary>
/// 付款單管理控制器 (SYSP271-SYSP2B0)
/// </summary>
[ApiController]
[Route("api/v1/payment-vouchers")]
public class PaymentVoucherController : BaseController
{
    private readonly IPaymentVoucherService _service;

    public PaymentVoucherController(
        IPaymentVoucherService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢付款單列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<PaymentVoucherDto>>>> GetPaymentVouchers(
        [FromQuery] PaymentVoucherQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPaymentVouchersAsync(query);
            return result;
        }, "查詢付款單列表失敗");
    }

    /// <summary>
    /// 查詢單筆付款單
    /// </summary>
    [HttpGet("{paymentNo}")]
    public async Task<ActionResult<ApiResponse<PaymentVoucherDto>>> GetPaymentVoucher(string paymentNo)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPaymentVoucherByIdAsync(paymentNo);
            return result;
        }, $"查詢付款單失敗: {paymentNo}");
    }

    /// <summary>
    /// 新增付款單
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreatePaymentVoucher(
        [FromBody] CreatePaymentVoucherDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var paymentNo = await _service.CreatePaymentVoucherAsync(dto);
            return paymentNo;
        }, "新增付款單失敗");
    }

    /// <summary>
    /// 修改付款單
    /// </summary>
    [HttpPut("{paymentNo}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdatePaymentVoucher(
        string paymentNo,
        [FromBody] UpdatePaymentVoucherDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdatePaymentVoucherAsync(paymentNo, dto);
        }, $"修改付款單失敗: {paymentNo}");
    }

    /// <summary>
    /// 刪除付款單
    /// </summary>
    [HttpDelete("{paymentNo}")]
    public async Task<ActionResult<ApiResponse<object>>> DeletePaymentVoucher(string paymentNo)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeletePaymentVoucherAsync(paymentNo);
        }, $"刪除付款單失敗: {paymentNo}");
    }

    /// <summary>
    /// 確認付款單
    /// </summary>
    [HttpPost("{paymentNo}/confirm")]
    public async Task<ActionResult<ApiResponse<object>>> ConfirmPaymentVoucher(string paymentNo)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.ConfirmPaymentVoucherAsync(paymentNo);
        }, $"確認付款單失敗: {paymentNo}");
    }

    /// <summary>
    /// 檢查付款單是否存在
    /// </summary>
    [HttpGet("{paymentNo}/exists")]
    public async Task<ActionResult<ApiResponse<bool>>> CheckPaymentVoucherExists(string paymentNo)
    {
        return await ExecuteAsync(async () =>
        {
            var exists = await _service.ExistsAsync(paymentNo);
            return exists;
        }, $"檢查付款單是否存在失敗: {paymentNo}");
    }
}
