using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Accounting;
using ErpCore.Application.Services.Accounting;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Accounting;

/// <summary>
/// 會計管理控制器 (SYSN120-SYSN154)
/// 提供會計憑證管理、會計帳簿管理、會計報表查詢、會計結帳作業等功能
/// </summary>
[Route("api/v1/accounting/vouchers")]
public class AccountingController : BaseController
{
    private readonly IVoucherService _service;

    public AccountingController(
        IVoucherService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢傳票列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<VoucherDto>>>> GetVouchers(
        [FromQuery] VoucherQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetVouchersAsync(query);
            return result;
        }, "查詢傳票列表失敗");
    }

    /// <summary>
    /// 根據傳票編號查詢傳票
    /// </summary>
    [HttpGet("{voucherId}")]
    public async Task<ActionResult<ApiResponse<VoucherDto>>> GetVoucher(string voucherId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetVoucherByIdAsync(voucherId);
            return result;
        }, "查詢傳票失敗");
    }

    /// <summary>
    /// 新增傳票
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateVoucher(
        [FromBody] CreateVoucherDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateVoucherAsync(dto);
            return result;
        }, "新增傳票失敗");
    }

    /// <summary>
    /// 修改傳票
    /// </summary>
    [HttpPut("{voucherId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateVoucher(
        string voucherId,
        [FromBody] UpdateVoucherDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateVoucherAsync(voucherId, dto);
        }, "修改傳票失敗");
    }

    /// <summary>
    /// 刪除傳票
    /// </summary>
    [HttpDelete("{voucherId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteVoucher(string voucherId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteVoucherAsync(voucherId);
        }, "刪除傳票失敗");
    }

    /// <summary>
    /// 過帳傳票
    /// </summary>
    [HttpPost("{voucherId}/post")]
    public async Task<ActionResult<ApiResponse<object>>> PostVoucher(string voucherId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.PostVoucherAsync(voucherId);
        }, "過帳傳票失敗");
    }
}

