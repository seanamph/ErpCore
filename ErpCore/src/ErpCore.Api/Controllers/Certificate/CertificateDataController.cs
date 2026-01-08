using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Certificate;
using ErpCore.Application.Services.Certificate;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Certificate;

/// <summary>
/// 憑證資料維護控制器 (SYSK110-SYSK150)
/// 提供憑證資料的新增、修改、刪除、查詢功能
/// </summary>
[Route("api/v1/vouchers")]
public class CertificateDataController : BaseController
{
    private readonly IVoucherService _voucherService;

    public CertificateDataController(
        IVoucherService voucherService,
        ILoggerService logger) : base(logger)
    {
        _voucherService = voucherService;
    }

    /// <summary>
    /// 查詢憑證列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<VoucherDto>>>> GetVouchers(
        [FromQuery] VoucherQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _voucherService.GetVouchersAsync(query);
            return result;
        }, "查詢憑證列表失敗");
    }

    /// <summary>
    /// 查詢單筆憑證
    /// </summary>
    [HttpGet("{voucherId}")]
    public async Task<ActionResult<ApiResponse<VoucherDto>>> GetVoucher(string voucherId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _voucherService.GetVoucherByIdAsync(voucherId);
            return result;
        }, $"查詢憑證失敗: {voucherId}");
    }

    /// <summary>
    /// 新增憑證
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<VoucherDto>>> CreateVoucher(
        [FromBody] CreateVoucherDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _voucherService.CreateVoucherAsync(dto);
            return result;
        }, "新增憑證失敗");
    }

    /// <summary>
    /// 修改憑證
    /// </summary>
    [HttpPut("{voucherId}")]
    public async Task<ActionResult<ApiResponse<VoucherDto>>> UpdateVoucher(
        string voucherId,
        [FromBody] UpdateVoucherDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _voucherService.UpdateVoucherAsync(voucherId, dto);
            return result;
        }, $"修改憑證失敗: {voucherId}");
    }

    /// <summary>
    /// 刪除憑證
    /// </summary>
    [HttpDelete("{voucherId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteVoucher(string voucherId)
    {
        return await ExecuteAsync(async () =>
        {
            await _voucherService.DeleteVoucherAsync(voucherId);
        }, $"刪除憑證失敗: {voucherId}");
    }

    /// <summary>
    /// 審核憑證
    /// </summary>
    [HttpPut("{voucherId}/approve")]
    public async Task<ActionResult<ApiResponse<object>>> ApproveVoucher(
        string voucherId,
        [FromBody] ApproveVoucherDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _voucherService.ApproveVoucherAsync(voucherId, dto);
        }, $"審核憑證失敗: {voucherId}");
    }

    /// <summary>
    /// 取消憑證
    /// </summary>
    [HttpPut("{voucherId}/cancel")]
    public async Task<ActionResult<ApiResponse<object>>> CancelVoucher(
        string voucherId,
        [FromBody] CancelVoucherDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _voucherService.CancelVoucherAsync(voucherId, dto);
        }, $"取消憑證失敗: {voucherId}");
    }
}

