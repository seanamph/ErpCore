using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.TaxAccounting;
using ErpCore.Application.Services.TaxAccounting;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.TaxAccounting;

/// <summary>
/// 發票資料維護控制器 (SYST211-SYST212)
/// 提供傳票維護作業、費用/收入分攤比率設定等功能
/// </summary>
[Route("api/v1/tax-accounting/invoice-data")]
public class InvoiceDataController : BaseController
{
    private readonly IInvoiceDataService _service;

    public InvoiceDataController(
        IInvoiceDataService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    #region SYST211 - 傳票維護作業

    /// <summary>
    /// 查詢傳票列表
    /// </summary>
    [HttpGet("vouchers")]
    public async Task<ActionResult<ApiResponse<PagedResult<InvoiceVoucherDto>>>> GetVouchers(
        [FromQuery] InvoiceVoucherQueryDto query)
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
    [HttpGet("vouchers/{voucherId}")]
    public async Task<ActionResult<ApiResponse<InvoiceVoucherDto>>> GetVoucher(string voucherId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetVoucherByIdAsync(voucherId);
            if (result == null)
            {
                throw new InvalidOperationException($"傳票不存在: {voucherId}");
            }
            return result;
        }, $"查詢傳票失敗: {voucherId}");
    }

    /// <summary>
    /// 新增傳票
    /// </summary>
    [HttpPost("vouchers")]
    public async Task<ActionResult<ApiResponse<string>>> CreateVoucher(
        [FromBody] CreateInvoiceVoucherDto dto)
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
    [HttpPut("vouchers/{voucherId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateVoucher(
        string voucherId,
        [FromBody] UpdateInvoiceVoucherDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateVoucherAsync(voucherId, dto);
            return (object)null!;
        }, $"修改傳票失敗: {voucherId}");
    }

    /// <summary>
    /// 刪除傳票
    /// </summary>
    [HttpDelete("vouchers/{voucherId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteVoucher(string voucherId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteVoucherAsync(voucherId);
            return (object)null!;
        }, $"刪除傳票失敗: {voucherId}");
    }

    /// <summary>
    /// 作廢傳票
    /// </summary>
    [HttpPost("vouchers/{voucherId}/void")]
    public async Task<ActionResult<ApiResponse<object>>> VoidVoucher(string voucherId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.VoidVoucherAsync(voucherId);
            return (object)null!;
        }, $"作廢傳票失敗: {voucherId}");
    }

    /// <summary>
    /// 檢查傳票借貸平衡
    /// </summary>
    [HttpGet("vouchers/{voucherId}/check-balance")]
    public async Task<ActionResult<ApiResponse<BalanceCheckDto>>> CheckBalance(string voucherId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CheckBalanceAsync(voucherId);
            return result;
        }, $"檢查傳票借貸平衡失敗: {voucherId}");
    }

    #endregion

    #region SYST212 - 費用/收入分攤比率設定

    /// <summary>
    /// 查詢費用/收入分攤比率列表
    /// </summary>
    [HttpGet("allocation-ratios")]
    public async Task<ActionResult<ApiResponse<PagedResult<AllocationRatioDto>>>> GetAllocationRatios(
        [FromQuery] AllocationRatioQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetAllocationRatiosAsync(query);
            return result;
        }, "查詢分攤比率列表失敗");
    }

    /// <summary>
    /// 新增費用/收入分攤比率
    /// </summary>
    [HttpPost("allocation-ratios")]
    public async Task<ActionResult<ApiResponse<long>>> CreateAllocationRatio(
        [FromBody] CreateAllocationRatioDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateAllocationRatioAsync(dto);
            return result;
        }, "新增分攤比率失敗");
    }

    /// <summary>
    /// 修改費用/收入分攤比率
    /// </summary>
    [HttpPut("allocation-ratios/{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateAllocationRatio(
        long tKey,
        [FromBody] UpdateAllocationRatioDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateAllocationRatioAsync(tKey, dto);
            return (object)null!;
        }, $"修改分攤比率失敗: {tKey}");
    }

    /// <summary>
    /// 刪除費用/收入分攤比率
    /// </summary>
    [HttpDelete("allocation-ratios/{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteAllocationRatio(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteAllocationRatioAsync(tKey);
            return (object)null!;
        }, $"刪除分攤比率失敗: {tKey}");
    }

    #endregion
}

