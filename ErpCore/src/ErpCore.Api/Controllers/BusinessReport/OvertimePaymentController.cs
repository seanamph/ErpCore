using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.BusinessReport;
using ErpCore.Application.Services.BusinessReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.BusinessReport;

/// <summary>
/// 加班發放管理控制器 (SYSL510)
/// </summary>
[Route("api/v1/overtime-payments")]
public class OvertimePaymentController : BaseController
{
    private readonly IOvertimePaymentService _service;

    public OvertimePaymentController(
        IOvertimePaymentService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢加班發放列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<OvertimePaymentDto>>>> GetOvertimePayments(
        [FromQuery] OvertimePaymentQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetOvertimePaymentsAsync(query);
            return result;
        }, "查詢加班發放列表失敗");
    }

    /// <summary>
    /// 根據發放單號查詢單筆加班發放
    /// </summary>
    [HttpGet("{paymentNo}")]
    public async Task<ActionResult<ApiResponse<OvertimePaymentDto>>> GetOvertimePaymentByPaymentNo(string paymentNo)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetOvertimePaymentByPaymentNoAsync(paymentNo);
            if (result == null)
            {
                throw new Exception($"找不到加班發放: {paymentNo}");
            }
            return result;
        }, "查詢加班發放失敗");
    }

    /// <summary>
    /// 新增加班發放
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateOvertimePayment([FromBody] CreateOvertimePaymentDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateOvertimePaymentAsync(dto);
            return result;
        }, "新增加班發放失敗");
    }

    /// <summary>
    /// 修改加班發放
    /// </summary>
    [HttpPut("{paymentNo}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateOvertimePayment(
        string paymentNo,
        [FromBody] UpdateOvertimePaymentDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateOvertimePaymentAsync(paymentNo, dto);
            return (object)null!;
        }, "修改加班發放失敗");
    }

    /// <summary>
    /// 刪除加班發放
    /// </summary>
    [HttpDelete("{paymentNo}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteOvertimePayment(string paymentNo)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteOvertimePaymentAsync(paymentNo);
            return (object)null!;
        }, "刪除加班發放失敗");
    }

    /// <summary>
    /// 審核加班發放
    /// </summary>
    [HttpPost("{paymentNo}/approve")]
    public async Task<ActionResult<ApiResponse<object>>> ApproveOvertimePayment(
        string paymentNo,
        [FromBody] ApproveOvertimePaymentDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.ApproveOvertimePaymentAsync(paymentNo, dto);
            return (object)null!;
        }, "審核加班發放失敗");
    }
}

