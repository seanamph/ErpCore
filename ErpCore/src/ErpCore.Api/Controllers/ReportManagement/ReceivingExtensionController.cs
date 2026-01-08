using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.ReportManagement;
using ErpCore.Application.Services.ReportManagement;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.ReportManagement;

/// <summary>
/// 收款擴展功能控制器 (SYSR310-SYSR450)
/// 提供拋轉收款沖帳傳票、收款查詢與報表等擴展功能
/// </summary>
[Route("api/v1/receipt")]
public class ReceivingExtensionController : BaseController
{
    private readonly IReceivingExtensionService _service;

    public ReceivingExtensionController(
        IReceivingExtensionService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    #region 收款沖帳傳票維護 (SYSR310)

    /// <summary>
    /// 查詢收款沖帳傳票列表
    /// </summary>
    [HttpGet("vouchertransfer")]
    public async Task<ActionResult<ApiResponse<PagedResult<ReceiptVoucherTransferDto>>>> GetReceiptVoucherTransfer(
        [FromQuery] ReceiptVoucherTransferQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            return await _service.QueryReceiptVoucherTransferAsync(query);
        }, "查詢收款沖帳傳票列表失敗");
    }

    /// <summary>
    /// 查詢單筆收款沖帳傳票
    /// </summary>
    [HttpGet("vouchertransfer/{tKey}")]
    public async Task<ActionResult<ApiResponse<ReceiptVoucherTransferDto>>> GetReceiptVoucherTransfer(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            return await _service.GetReceiptVoucherTransferByIdAsync(tKey);
        }, $"查詢收款沖帳傳票失敗: {tKey}");
    }

    /// <summary>
    /// 新增收款沖帳傳票
    /// </summary>
    [HttpPost("vouchertransfer")]
    public async Task<ActionResult<ApiResponse<ReceiptVoucherTransferDto>>> CreateReceiptVoucherTransfer(
        [FromBody] CreateReceiptVoucherTransferDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            return await _service.CreateReceiptVoucherTransferAsync(dto);
        }, "新增收款沖帳傳票失敗");
    }

    /// <summary>
    /// 修改收款沖帳傳票
    /// </summary>
    [HttpPut("vouchertransfer/{tKey}")]
    public async Task<ActionResult<ApiResponse<ReceiptVoucherTransferDto>>> UpdateReceiptVoucherTransfer(
        long tKey,
        [FromBody] UpdateReceiptVoucherTransferDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            return await _service.UpdateReceiptVoucherTransferAsync(tKey, dto);
        }, $"修改收款沖帳傳票失敗: {tKey}");
    }

    /// <summary>
    /// 刪除收款沖帳傳票
    /// </summary>
    [HttpDelete("vouchertransfer/{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteReceiptVoucherTransfer(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteReceiptVoucherTransferAsync(tKey);
            return new { message = "刪除成功" };
        }, $"刪除收款沖帳傳票失敗: {tKey}");
    }

    #endregion

    #region 拋轉收款沖帳傳票 (SYSR310)

    /// <summary>
    /// 拋轉收款沖帳傳票
    /// </summary>
    [HttpPost("vouchertransfer/{tKey}/transfer")]
    public async Task<ActionResult<ApiResponse<ReceiptVoucherTransferDto>>> TransferReceiptVoucher(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            return await _service.TransferReceiptVoucherAsync(tKey);
        }, $"拋轉收款沖帳傳票失敗: {tKey}");
    }

    /// <summary>
    /// 批次拋轉收款沖帳傳票
    /// </summary>
    [HttpPost("vouchertransfer/batch/transfer")]
    public async Task<ActionResult<ApiResponse<BatchTransferResultDto>>> BatchTransferReceiptVoucher(
        [FromBody] BatchTransferReceiptVoucherDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            return await _service.BatchTransferReceiptVoucherAsync(dto);
        }, "批次拋轉收款沖帳傳票失敗");
    }

    #endregion
}

