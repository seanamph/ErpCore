using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.TaxAccounting;
using ErpCore.Application.Services.TaxAccounting;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.TaxAccounting;

/// <summary>
/// 暫存傳票審核控制器 (SYSTA00-SYSTA70)
/// 提供暫存傳票的審核、維護、傳送、查詢功能
/// </summary>
[Route("api/v1/tax-accounting/voucher-audit")]
public class VoucherAuditController : BaseController
{
    private readonly IVoucherAuditService _service;

    public VoucherAuditController(
        IVoucherAuditService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢前端系統列表
    /// </summary>
    [HttpGet("system-list")]
    public async Task<ActionResult<ApiResponse<List<SystemVoucherCountDto>>>> GetSystemList()
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSystemListAsync();
            return result;
        }, "查詢前端系統列表失敗");
    }

    /// <summary>
    /// 查詢暫存傳票列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<TmpVoucherDto>>>> GetTmpVouchers(
        [FromQuery] TmpVoucherQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetTmpVouchersAsync(query);
            return result;
        }, "查詢暫存傳票列表失敗");
    }

    /// <summary>
    /// 查詢單筆暫存傳票
    /// </summary>
    [HttpGet("{tKey}")]
    public async Task<ActionResult<ApiResponse<TmpVoucherDetailDto>>> GetTmpVoucher(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetTmpVoucherByIdAsync(tKey);
            return result;
        }, $"查詢暫存傳票失敗: {tKey}");
    }

    /// <summary>
    /// 修改暫存傳票
    /// </summary>
    [HttpPut("{tKey}")]
    public async Task<ActionResult<ApiResponse<TmpVoucherDetailDto>>> UpdateTmpVoucher(
        long tKey,
        [FromBody] UpdateTmpVoucherDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.UpdateTmpVoucherAsync(tKey, dto);
            return result;
        }, $"修改暫存傳票失敗: {tKey}");
    }

    /// <summary>
    /// 刪除暫存傳票
    /// </summary>
    [HttpDelete("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteTmpVoucher(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteTmpVoucherAsync(tKey);
            return new object();
        }, $"刪除暫存傳票失敗: {tKey}");
    }

    /// <summary>
    /// 拋轉暫存傳票
    /// </summary>
    [HttpPost("{tKey}/transfer")]
    public async Task<ActionResult<ApiResponse<TransferVoucherResultDto>>> TransferTmpVoucher(
        long tKey,
        [FromBody] TransferVoucherDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.TransferTmpVoucherAsync(tKey, dto);
            return result;
        }, $"拋轉暫存傳票失敗: {tKey}");
    }

    /// <summary>
    /// 批次拋轉暫存傳票
    /// </summary>
    [HttpPost("batch-transfer")]
    public async Task<ActionResult<ApiResponse<BatchTransferResultDto>>> BatchTransferTmpVouchers(
        [FromBody] BatchTransferVoucherDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.BatchTransferTmpVouchersAsync(dto);
            return result;
        }, "批次拋轉暫存傳票失敗");
    }

    /// <summary>
    /// 查詢未審核筆數
    /// </summary>
    [HttpGet("unreviewed-count")]
    public async Task<ActionResult<ApiResponse<UnreviewedCountDto>>> GetUnreviewedCount(
        [FromQuery] string? typeId = null,
        [FromQuery] string? sysId = null)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetUnreviewedCountAsync(typeId, sysId);
            return result;
        }, "查詢未審核筆數失敗");
    }
}

