using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Communication;
using ErpCore.Application.Services.Communication;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Communication;

/// <summary>
/// 自動處理郵件作業控制器
/// </summary>
[Route("api/v1/email/process")]
public class AutoProcessMailController : BaseController
{
    private readonly IEmailQueueService _emailQueueService;

    public AutoProcessMailController(
        IEmailQueueService emailQueueService,
        ILoggerService logger) : base(logger)
    {
        _emailQueueService = emailQueueService;
    }

    /// <summary>
    /// 處理郵件佇列
    /// </summary>
    [HttpPost("queue")]
    public async Task<ActionResult<ApiResponse<ProcessEmailQueueResponseDto>>> ProcessQueue([FromBody] ProcessEmailQueueRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            return await _emailQueueService.ProcessQueueAsync(request);
        }, "處理郵件佇列失敗");
    }

    /// <summary>
    /// 重試失敗郵件
    /// </summary>
    [HttpPost("retry-failed")]
    public async Task<ActionResult<ApiResponse<ProcessEmailQueueResponseDto>>> RetryFailedEmails([FromBody] RetryFailedEmailsRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            return await _emailQueueService.RetryFailedEmailsAsync(request);
        }, "重試失敗郵件失敗");
    }

    /// <summary>
    /// 查詢郵件佇列狀態
    /// </summary>
    [HttpGet("queue-status")]
    public async Task<ActionResult<ApiResponse<EmailQueueStatusDto>>> GetQueueStatus()
    {
        return await ExecuteAsync(async () =>
        {
            return await _emailQueueService.GetQueueStatusAsync();
        }, "查詢郵件佇列狀態失敗");
    }
}

