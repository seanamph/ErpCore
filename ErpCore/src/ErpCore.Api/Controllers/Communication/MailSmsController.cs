using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Communication;
using ErpCore.Application.Services.Communication;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Communication;

/// <summary>
/// 郵件簡訊發送作業控制器 (SYS5000)
/// </summary>
[Route("api/v1/notifications")]
public class MailSmsController : BaseController
{
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;

    public MailSmsController(
        IEmailService emailService,
        ISmsService smsService,
        ILoggerService logger) : base(logger)
    {
        _emailService = emailService;
        _smsService = smsService;
    }

    /// <summary>
    /// 發送郵件
    /// </summary>
    [HttpPost("email/send")]
    public async Task<ActionResult<ApiResponse<SendEmailResponseDto>>> SendEmail([FromBody] SendEmailRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            return await _emailService.SendEmailAsync(request);
        }, "發送郵件失敗");
    }

    /// <summary>
    /// 使用模板發送郵件
    /// </summary>
    [HttpPost("email/send-template")]
    public async Task<ActionResult<ApiResponse<SendEmailResponseDto>>> SendEmailWithTemplate([FromBody] SendEmailTemplateRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            return await _emailService.SendEmailWithTemplateAsync(request);
        }, "使用模板發送郵件失敗");
    }

    /// <summary>
    /// 查詢郵件發送記錄
    /// </summary>
    [HttpGet("email/logs")]
    public async Task<ActionResult<ApiResponse<PagedResult<EmailLogDto>>>> GetEmailLogs([FromQuery] EmailLogQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            return await _emailService.GetEmailLogsAsync(query);
        }, "查詢郵件發送記錄失敗");
    }

    /// <summary>
    /// 根據ID查詢郵件記錄
    /// </summary>
    [HttpGet("email/logs/{id}")]
    public async Task<ActionResult<ApiResponse<EmailLogDto>>> GetEmailLogById(long id)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _emailService.GetEmailLogByIdAsync(id);
            if (result == null)
            {
                throw new Exception("郵件記錄不存在");
            }
            return result;
        }, "查詢郵件記錄失敗");
    }

    /// <summary>
    /// 取得郵件附件列表
    /// </summary>
    [HttpGet("email/{emailLogId}/attachments")]
    public async Task<ActionResult<ApiResponse<List<EmailAttachmentDto>>>> GetEmailAttachments(long emailLogId)
    {
        return await ExecuteAsync(async () =>
        {
            return await _emailService.GetEmailAttachmentsAsync(emailLogId);
        }, "查詢郵件附件失敗");
    }

    /// <summary>
    /// 發送簡訊
    /// </summary>
    [HttpPost("sms/send")]
    public async Task<ActionResult<ApiResponse<SendSmsResponseDto>>> SendSms([FromBody] SendSmsRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            return await _smsService.SendSmsAsync(request);
        }, "發送簡訊失敗");
    }

    /// <summary>
    /// 查詢簡訊發送記錄
    /// </summary>
    [HttpGet("sms/logs")]
    public async Task<ActionResult<ApiResponse<PagedResult<SmsLogDto>>>> GetSmsLogs([FromQuery] SmsLogQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            return await _smsService.GetSmsLogsAsync(query);
        }, "查詢簡訊發送記錄失敗");
    }

    /// <summary>
    /// 根據ID查詢簡訊記錄
    /// </summary>
    [HttpGet("sms/logs/{id}")]
    public async Task<ActionResult<ApiResponse<SmsLogDto>>> GetSmsLogById(long id)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _smsService.GetSmsLogByIdAsync(id);
            if (result == null)
            {
                throw new Exception("簡訊記錄不存在");
            }
            return result;
        }, "查詢簡訊記錄失敗");
    }
}

