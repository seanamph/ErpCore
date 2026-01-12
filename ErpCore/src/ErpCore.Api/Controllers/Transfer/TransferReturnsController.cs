using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Transfer;
using ErpCore.Application.Services.Transfer;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Transfer;

/// <summary>
/// 調撥單驗退作業控制器 (SYSW362)
/// </summary>
[Route("api/v1/transfer-returns")]
public class TransferReturnsController : BaseController
{
    private readonly ITransferReturnService _service;

    public TransferReturnsController(
        ITransferReturnService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢待驗退調撥單列表
    /// </summary>
    [HttpGet("pending-transfers")]
    public async Task<ActionResult<ApiResponse<PagedResult<PendingTransferOrderForReturnDto>>>> GetPendingTransfers(
        [FromQuery] PendingTransferOrderForReturnQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPendingTransfersAsync(query);
            return result;
        }, "查詢待驗退調撥單失敗");
    }

    /// <summary>
    /// 查詢驗退單列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<TransferReturnDto>>>> GetTransferReturns(
        [FromQuery] TransferReturnQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetTransferReturnsAsync(query);
            return result;
        }, "查詢驗退單列表失敗");
    }

    /// <summary>
    /// 查詢單筆驗退單
    /// </summary>
    [HttpGet("{returnId}")]
    public async Task<ActionResult<ApiResponse<TransferReturnDto>>> GetTransferReturn(string returnId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetTransferReturnByIdAsync(returnId);
            return result;
        }, $"查詢驗退單失敗: {returnId}");
    }

    /// <summary>
    /// 依調撥單號建立驗退單
    /// </summary>
    [HttpPost("from-transfer/{transferId}")]
    public async Task<ActionResult<ApiResponse<TransferReturnDto>>> CreateReturnFromTransfer(string transferId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateReturnFromTransferAsync(transferId);
            return result;
        }, $"依調撥單建立驗退單失敗: {transferId}");
    }

    /// <summary>
    /// 新增驗退單
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateTransferReturn(
        [FromBody] CreateTransferReturnDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var returnId = await _service.CreateTransferReturnAsync(dto);
            return returnId;
        }, "新增驗退單失敗");
    }

    /// <summary>
    /// 修改驗退單
    /// </summary>
    [HttpPut("{returnId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateTransferReturn(
        string returnId,
        [FromBody] UpdateTransferReturnDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateTransferReturnAsync(returnId, dto);
        }, $"修改驗退單失敗: {returnId}");
    }

    /// <summary>
    /// 刪除驗退單
    /// </summary>
    [HttpDelete("{returnId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteTransferReturn(string returnId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteTransferReturnAsync(returnId);
        }, $"刪除驗退單失敗: {returnId}");
    }

    /// <summary>
    /// 確認驗退
    /// </summary>
    [HttpPost("{returnId}/confirm")]
    public async Task<ActionResult<ApiResponse<object>>> ConfirmReturn(string returnId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.ConfirmReturnAsync(returnId);
        }, $"確認驗退失敗: {returnId}");
    }

    /// <summary>
    /// 取消驗退單
    /// </summary>
    [HttpPost("{returnId}/cancel")]
    public async Task<ActionResult<ApiResponse<object>>> CancelTransferReturn(string returnId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.CancelTransferReturnAsync(returnId);
        }, $"取消驗退單失敗: {returnId}");
    }
}
