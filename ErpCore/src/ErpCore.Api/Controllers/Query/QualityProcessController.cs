using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Query;
using ErpCore.Application.Services.Query;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Query;

/// <summary>
/// 質量管理處理功能控制器 (SYSQ210-SYSQ250)
/// 提供零用金維護、請款、拋轉、盤點、審核傳送等處理作業
/// </summary>
[Route("api/v1/quality/process")]
public class QualityProcessController : BaseController
{
    private readonly IPcCashService _pcCashService;
    private readonly IVoucherAuditService _voucherAuditService;

    public QualityProcessController(
        IPcCashService pcCashService,
        IVoucherAuditService voucherAuditService,
        ILoggerService logger) : base(logger)
    {
        _pcCashService = pcCashService;
        _voucherAuditService = voucherAuditService;
    }

    #region 零用金維護 (SYSQ210)

    /// <summary>
    /// 查詢零用金列表
    /// </summary>
    [HttpGet("pc-cash")]
    public async Task<ActionResult<ApiResponse<PagedResult<PcCashDto>>>> GetPcCashList(
        [FromQuery] PcCashQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _pcCashService.QueryAsync(query);
            return result;
        }, "查詢零用金列表失敗");
    }

    /// <summary>
    /// 查詢單筆零用金
    /// </summary>
    [HttpGet("pc-cash/{tKey}")]
    public async Task<ActionResult<ApiResponse<PcCashDto>>> GetPcCash(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _pcCashService.GetByIdAsync(tKey);
            return result;
        }, $"查詢零用金失敗: {tKey}");
    }

    /// <summary>
    /// 新增零用金
    /// </summary>
    [HttpPost("pc-cash")]
    public async Task<ActionResult<ApiResponse<PcCashDto>>> CreatePcCash(
        [FromBody] CreatePcCashDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _pcCashService.CreateAsync(dto);
            return result;
        }, "新增零用金失敗");
    }

    /// <summary>
    /// 修改零用金
    /// </summary>
    [HttpPut("pc-cash/{tKey}")]
    public async Task<ActionResult<ApiResponse<PcCashDto>>> UpdatePcCash(
        long tKey,
        [FromBody] UpdatePcCashDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _pcCashService.UpdateAsync(tKey, dto);
            return result;
        }, $"修改零用金失敗: {tKey}");
    }

    /// <summary>
    /// 刪除零用金
    /// </summary>
    [HttpDelete("pc-cash/{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeletePcCash(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _pcCashService.DeleteAsync(tKey);
        }, $"刪除零用金失敗: {tKey}");
    }

    /// <summary>
    /// 批量新增零用金
    /// </summary>
    [HttpPost("pc-cash/batch")]
    public async Task<ActionResult<ApiResponse<List<PcCashDto>>>> BatchCreatePcCash(
        [FromBody] BatchCreatePcCashDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _pcCashService.BatchCreateAsync(dto);
            return result;
        }, "批量新增零用金失敗");
    }

    #endregion

    #region 傳票審核傳送 (SYSQ250)

    /// <summary>
    /// 查詢傳票審核傳送檔列表
    /// </summary>
    [HttpGet("voucher-audit")]
    public async Task<ActionResult<ApiResponse<PagedResult<VoucherAuditDto>>>> GetVoucherAuditList(
        [FromQuery] VoucherAuditQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _voucherAuditService.QueryAsync(query);
            return result;
        }, "查詢傳票審核傳送檔列表失敗");
    }

    /// <summary>
    /// 查詢單筆傳票審核傳送檔
    /// </summary>
    [HttpGet("voucher-audit/{tKey}")]
    public async Task<ActionResult<ApiResponse<VoucherAuditDto>>> GetVoucherAudit(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _voucherAuditService.GetByIdAsync(tKey);
            return result;
        }, $"查詢傳票審核傳送檔失敗: {tKey}");
    }

    /// <summary>
    /// 依傳票編號查詢傳票審核傳送檔
    /// </summary>
    [HttpGet("voucher-audit/voucher/{voucherId}")]
    public async Task<ActionResult<ApiResponse<VoucherAuditDto>>> GetVoucherAuditByVoucherId(string voucherId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _voucherAuditService.GetByVoucherIdAsync(voucherId);
            return result;
        }, $"查詢傳票審核傳送檔失敗: {voucherId}");
    }

    /// <summary>
    /// 新增傳票審核傳送檔
    /// </summary>
    [HttpPost("voucher-audit")]
    public async Task<ActionResult<ApiResponse<VoucherAuditDto>>> CreateVoucherAudit(
        [FromBody] VoucherAuditDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _voucherAuditService.CreateAsync(dto);
            return result;
        }, "新增傳票審核傳送檔失敗");
    }

    /// <summary>
    /// 修改傳票審核傳送檔
    /// </summary>
    [HttpPut("voucher-audit/{tKey}")]
    public async Task<ActionResult<ApiResponse<VoucherAuditDto>>> UpdateVoucherAudit(
        long tKey,
        [FromBody] UpdateVoucherAuditDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _voucherAuditService.UpdateAsync(tKey, dto);
            return result;
        }, $"修改傳票審核傳送檔失敗: {tKey}");
    }

    /// <summary>
    /// 刪除傳票審核傳送檔
    /// </summary>
    [HttpDelete("voucher-audit/{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteVoucherAudit(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _voucherAuditService.DeleteAsync(tKey);
        }, $"刪除傳票審核傳送檔失敗: {tKey}");
    }

    #endregion
}

