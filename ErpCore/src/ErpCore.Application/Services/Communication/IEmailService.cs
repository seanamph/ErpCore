using ErpCore.Application.DTOs.Communication;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Communication;

/// <summary>
/// 郵件服務介面
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// 發送郵件
    /// </summary>
    Task<SendEmailResponseDto> SendEmailAsync(SendEmailRequestDto request);

    /// <summary>
    /// 使用模板發送郵件
    /// </summary>
    Task<SendEmailResponseDto> SendEmailWithTemplateAsync(SendEmailTemplateRequestDto request);

    /// <summary>
    /// 查詢郵件記錄列表
    /// </summary>
    Task<PagedResult<EmailLogDto>> GetEmailLogsAsync(EmailLogQueryDto query);

    /// <summary>
    /// 根據ID查詢郵件記錄
    /// </summary>
    Task<EmailLogDto?> GetEmailLogByIdAsync(long id);

    /// <summary>
    /// 取得郵件附件列表
    /// </summary>
    Task<List<EmailAttachmentDto>> GetEmailAttachmentsAsync(long emailLogId);
}

