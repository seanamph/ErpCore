using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.TaxAccounting;
using ErpCore.Application.Services.TaxAccounting;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.TaxAccounting;

/// <summary>
/// 交易資料處理控制器 (SYST221, SYST311-SYST352)
/// 提供傳票確認、過帳、反過帳、年結處理等功能
/// </summary>
[Route("api/v1/tax-accounting/transaction-data")]
public class TransactionDataController : BaseController
{
    private readonly ITransactionDataService _service;

    public TransactionDataController(
        ITransactionDataService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    #region SYST311-SYST312 - 傳票確認作業

    /// <summary>
    /// 查詢傳票確認列表
    /// </summary>
    [HttpGet("confirm")]
    public async Task<ActionResult<ApiResponse<PagedResult<InvoiceVoucherDto>>>> GetConfirmVouchers(
        [FromQuery] VoucherConfirmQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetConfirmVouchersAsync(query);
            return result;
        }, "查詢傳票確認列表失敗");
    }

    /// <summary>
    /// 批次確認傳票
    /// </summary>
    [HttpPost("confirm/batch")]
    public async Task<ActionResult<ApiResponse<BatchConfirmResultDto>>> BatchConfirmVouchers(
        [FromBody] BatchConfirmVoucherDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.BatchConfirmVouchersAsync(dto);
            return result;
        }, "批次確認傳票失敗");
    }

    #endregion

    #region SYST321-SYST322 - 傳票過帳作業

    /// <summary>
    /// 查詢傳票過帳列表
    /// </summary>
    [HttpGet("posting")]
    public async Task<ActionResult<ApiResponse<PagedResult<InvoiceVoucherDto>>>> GetPostingVouchers(
        [FromQuery] VoucherPostingQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPostingVouchersAsync(query);
            return result;
        }, "查詢傳票過帳列表失敗");
    }

    /// <summary>
    /// 批次過帳傳票
    /// </summary>
    [HttpPost("posting/batch")]
    public async Task<ActionResult<ApiResponse<BatchPostingResultDto>>> BatchPostingVouchers(
        [FromBody] BatchPostingVoucherDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.BatchPostingVouchersAsync(dto);
            return result;
        }, "批次過帳傳票失敗");
    }

    /// <summary>
    /// 查詢傳票狀態統計
    /// </summary>
    [HttpGet("status-count")]
    public async Task<ActionResult<ApiResponse<VoucherStatusCountDto>>> GetVoucherStatusCount(
        [FromQuery] string postingYearMonth)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetVoucherStatusCountAsync(postingYearMonth);
            return result;
        }, "查詢傳票狀態統計失敗");
    }

    #endregion

    #region SYST331-SYST332 - 傳票查詢作業

    /// <summary>
    /// 查詢傳票列表
    /// </summary>
    [HttpGet("query")]
    public async Task<ActionResult<ApiResponse<PagedResult<InvoiceVoucherDto>>>> QueryVouchers(
        [FromQuery] InvoiceVoucherQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.QueryVouchersAsync(query);
            return result;
        }, "查詢傳票列表失敗");
    }

    #endregion

    #region SYST351-SYST352 - 反過帳資料年結處理作業

    /// <summary>
    /// 查詢反過帳資料年結處理
    /// </summary>
    [HttpGet("reverse-year-end")]
    public async Task<ActionResult<ApiResponse<PagedResult<InvoiceVoucherDto>>>> GetReverseYearEndVouchers(
        [FromQuery] ReverseYearEndQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetReverseYearEndVouchersAsync(query);
            return result;
        }, "查詢反過帳資料年結處理失敗");
    }

    /// <summary>
    /// 反過帳傳票
    /// </summary>
    [HttpPost("vouchers/{voucherId}/reverse-posting")]
    public async Task<ActionResult<ApiResponse<object>>> ReversePostingVoucher(
        string voucherId,
        [FromBody] ReversePostingDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.ReversePostingVoucherAsync(voucherId, dto);
            return (object)null!;
        }, $"反過帳傳票失敗: {voucherId}");
    }

    #endregion
}

