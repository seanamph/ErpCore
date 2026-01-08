using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Transfer;
using ErpCore.Application.Services.Transfer;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Transfer;

/// <summary>
/// 調撥短溢維護作業控制器 (SYSW384)
/// </summary>
[Route("api/v1/transfer-shortage")]
public class TransferShortageController : BaseController
{
    private readonly ITransferShortageService _service;

    public TransferShortageController(
        ITransferShortageService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢短溢單列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<TransferShortageDto>>>> GetTransferShortages(
        [FromQuery] TransferShortageQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetTransferShortagesAsync(query);
            return result;
        }, "查詢短溢單列表失敗");
    }

    /// <summary>
    /// 查詢單筆短溢單
    /// </summary>
    [HttpGet("{shortageId}")]
    public async Task<ActionResult<ApiResponse<TransferShortageDto>>> GetTransferShortage(string shortageId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetTransferShortageByIdAsync(shortageId);
            return result;
        }, $"查詢短溢單失敗: {shortageId}");
    }

    /// <summary>
    /// 依調撥單號建立短溢單
    /// </summary>
    [HttpPost("from-transfer/{transferId}")]
    public async Task<ActionResult<ApiResponse<TransferShortageDto>>> CreateShortageFromTransfer(string transferId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateShortageFromTransferAsync(transferId);
            return result;
        }, $"依調撥單建立短溢單失敗: {transferId}");
    }

    /// <summary>
    /// 新增短溢單
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateTransferShortage(
        [FromBody] CreateTransferShortageDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var shortageId = await _service.CreateTransferShortageAsync(dto);
            return shortageId;
        }, "新增短溢單失敗");
    }

    /// <summary>
    /// 修改短溢單
    /// </summary>
    [HttpPut("{shortageId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateTransferShortage(
        string shortageId,
        [FromBody] UpdateTransferShortageDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateTransferShortageAsync(shortageId, dto);
        }, $"修改短溢單失敗: {shortageId}");
    }

    /// <summary>
    /// 刪除短溢單
    /// </summary>
    [HttpDelete("{shortageId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteTransferShortage(string shortageId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteTransferShortageAsync(shortageId);
        }, $"刪除短溢單失敗: {shortageId}");
    }

    /// <summary>
    /// 審核短溢單
    /// </summary>
    [HttpPost("{shortageId}/approve")]
    public async Task<ActionResult<ApiResponse<object>>> ApproveTransferShortage(
        string shortageId,
        [FromBody] ApproveTransferShortageDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.ApproveTransferShortageAsync(shortageId, dto);
        }, $"審核短溢單失敗: {shortageId}");
    }

    /// <summary>
    /// 處理短溢單
    /// </summary>
    [HttpPost("{shortageId}/process")]
    public async Task<ActionResult<ApiResponse<object>>> ProcessTransferShortage(
        string shortageId,
        [FromBody] ProcessTransferShortageDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.ProcessTransferShortageAsync(shortageId, dto);
        }, $"處理短溢單失敗: {shortageId}");
    }
}

