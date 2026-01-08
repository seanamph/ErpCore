using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.TaxAccounting;
using ErpCore.Application.Services.TaxAccounting;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.TaxAccounting;

/// <summary>
/// 會計憑證管理控制器 (SYST121-SYST12B)
/// 提供傳票型態設定、常用傳票資料維護等功能
/// </summary>
[Route("api/v1/tax-accounting/vouchers")]
public class AccountingVoucherController : BaseController
{
    private readonly IVoucherTypeService _voucherTypeService;
    private readonly ICommonVoucherService _commonVoucherService;

    public AccountingVoucherController(
        IVoucherTypeService voucherTypeService,
        ICommonVoucherService commonVoucherService,
        ILoggerService logger) : base(logger)
    {
        _voucherTypeService = voucherTypeService;
        _commonVoucherService = commonVoucherService;
    }

    #region 傳票型態設定 (SYST121-SYST122)

    /// <summary>
    /// 查詢傳票型態列表
    /// </summary>
    [HttpGet("types")]
    public async Task<ActionResult<ApiResponse<PagedResult<VoucherTypeDto>>>> GetVoucherTypes(
        [FromQuery] VoucherTypeQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _voucherTypeService.GetVoucherTypesAsync(query);
            return result;
        }, "查詢傳票型態列表失敗");
    }

    /// <summary>
    /// 查詢單筆傳票型態
    /// </summary>
    [HttpGet("types/{voucherId}")]
    public async Task<ActionResult<ApiResponse<VoucherTypeDto>>> GetVoucherType(string voucherId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _voucherTypeService.GetVoucherTypeByIdAsync(voucherId);
            return result;
        }, $"查詢傳票型態失敗: {voucherId}");
    }

    /// <summary>
    /// 新增傳票型態
    /// </summary>
    [HttpPost("types")]
    public async Task<ActionResult<ApiResponse<string>>> CreateVoucherType(
        [FromBody] CreateVoucherTypeDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _voucherTypeService.CreateVoucherTypeAsync(dto);
            return result;
        }, "新增傳票型態失敗");
    }

    /// <summary>
    /// 修改傳票型態
    /// </summary>
    [HttpPut("types/{voucherId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateVoucherType(
        string voucherId,
        [FromBody] UpdateVoucherTypeDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _voucherTypeService.UpdateVoucherTypeAsync(voucherId, dto);
            return new object();
        }, $"修改傳票型態失敗: {voucherId}");
    }

    /// <summary>
    /// 刪除傳票型態
    /// </summary>
    [HttpDelete("types/{voucherId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteVoucherType(string voucherId)
    {
        return await ExecuteAsync(async () =>
        {
            await _voucherTypeService.DeleteVoucherTypeAsync(voucherId);
            return new object();
        }, $"刪除傳票型態失敗: {voucherId}");
    }

    /// <summary>
    /// 檢查型態代號是否存在
    /// </summary>
    [HttpGet("types/{voucherId}/exists")]
    public async Task<ActionResult<ApiResponse<bool>>> CheckVoucherTypeExists(string voucherId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _voucherTypeService.ExistsAsync(voucherId);
            return result;
        }, $"檢查型態代號是否存在失敗: {voucherId}");
    }

    #endregion

    #region 常用傳票資料 (SYST123)

    /// <summary>
    /// 查詢常用傳票列表
    /// </summary>
    [HttpGet("common")]
    public async Task<ActionResult<ApiResponse<PagedResult<CommonVoucherDto>>>> GetCommonVouchers(
        [FromQuery] CommonVoucherQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _commonVoucherService.GetCommonVouchersAsync(query);
            return result;
        }, "查詢常用傳票列表失敗");
    }

    /// <summary>
    /// 查詢單筆常用傳票（含明細）
    /// </summary>
    [HttpGet("common/{tKey}")]
    public async Task<ActionResult<ApiResponse<CommonVoucherDto>>> GetCommonVoucher(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _commonVoucherService.GetCommonVoucherByTKeyAsync(tKey);
            return result;
        }, $"查詢常用傳票失敗: {tKey}");
    }

    /// <summary>
    /// 新增常用傳票
    /// </summary>
    [HttpPost("common")]
    public async Task<ActionResult<ApiResponse<long>>> CreateCommonVoucher(
        [FromBody] CreateCommonVoucherDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _commonVoucherService.CreateCommonVoucherAsync(dto);
            return result;
        }, "新增常用傳票失敗");
    }

    /// <summary>
    /// 修改常用傳票
    /// </summary>
    [HttpPut("common/{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateCommonVoucher(
        long tKey,
        [FromBody] UpdateCommonVoucherDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _commonVoucherService.UpdateCommonVoucherAsync(tKey, dto);
            return new object();
        }, $"修改常用傳票失敗: {tKey}");
    }

    /// <summary>
    /// 刪除常用傳票
    /// </summary>
    [HttpDelete("common/{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteCommonVoucher(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _commonVoucherService.DeleteCommonVoucherAsync(tKey);
            return new object();
        }, $"刪除常用傳票失敗: {tKey}");
    }

    /// <summary>
    /// 檢查傳票代號是否存在
    /// </summary>
    [HttpGet("common/exists/{voucherId}")]
    public async Task<ActionResult<ApiResponse<bool>>> CheckCommonVoucherExists(string voucherId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _commonVoucherService.ExistsAsync(voucherId);
            return result;
        }, $"檢查傳票代號是否存在失敗: {voucherId}");
    }

    #endregion
}

