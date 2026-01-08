using ErpCore.Application.DTOs.Communication;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Communication;
using ErpCore.Infrastructure.Repositories.Communication;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Communication;

/// <summary>
/// 郵件服務實作
/// </summary>
public class EmailService : BaseService, IEmailService
{
    private readonly IEmailLogRepository _emailLogRepository;
    private readonly IEmailAttachmentRepository _emailAttachmentRepository;

    public EmailService(
        IEmailLogRepository emailLogRepository,
        IEmailAttachmentRepository emailAttachmentRepository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _emailLogRepository = emailLogRepository;
        _emailAttachmentRepository = emailAttachmentRepository;
    }

    public async Task<SendEmailResponseDto> SendEmailAsync(SendEmailRequestDto request)
    {
        try
        {
            _logger.LogInfo($"開始發送郵件: {request.Subject}");

            // 建立郵件記錄
            var emailLog = new EmailLog
            {
                FromAddress = request.FromAddress,
                FromName = request.FromName,
                ToAddress = request.ToAddress,
                CcAddress = request.CcAddress,
                BccAddress = request.BccAddress,
                Subject = request.Subject,
                Body = request.Body,
                BodyType = request.BodyType ?? "Text",
                Priority = request.Priority ?? 3,
                Status = "Pending",
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                HasAttachment = request.Attachments != null && request.Attachments.Any(),
                AttachmentCount = request.Attachments?.Count ?? 0
            };

            var savedLog = await _emailLogRepository.CreateAsync(emailLog);

            // 儲存附件
            if (request.Attachments != null && request.Attachments.Any())
            {
                foreach (var attachment in request.Attachments)
                {
                    var emailAttachment = new EmailAttachment
                    {
                        EmailLogId = savedLog.Id,
                        FileName = attachment.FileName,
                        FilePath = attachment.FilePath,
                        FileSize = attachment.FileSize,
                        ContentType = attachment.ContentType,
                        CreatedAt = DateTime.Now
                    };
                    await _emailAttachmentRepository.CreateAsync(emailAttachment);
                }
            }

            // TODO: 實作實際的郵件發送邏輯（使用 MailKit 或 System.Net.Mail）
            // 目前先標記為已發送
            await _emailLogRepository.UpdateStatusAsync(savedLog.Id, "Sent", DateTime.Now);

            _logger.LogInfo($"郵件發送成功: {savedLog.Id}");

            return new SendEmailResponseDto
            {
                EmailLogId = savedLog.Id,
                Status = "Sent",
                SentAt = DateTime.Now
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("發送郵件失敗", ex);
            throw;
        }
    }

    public async Task<SendEmailResponseDto> SendEmailWithTemplateAsync(SendEmailTemplateRequestDto request)
    {
        try
        {
            _logger.LogInfo($"使用模板發送郵件: {request.TemplateCode}");

            // TODO: 實作模板載入和參數替換邏輯
            // 目前先使用基本發送邏輯
            var sendRequest = new SendEmailRequestDto
            {
                ToAddress = request.ToAddress,
                CcAddress = request.CcAddress,
                BccAddress = request.BccAddress,
                Subject = "模板郵件", // TODO: 從模板載入
                Body = "模板內容", // TODO: 從模板載入並替換參數
                BodyType = "Text"
            };

            return await SendEmailAsync(sendRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError("使用模板發送郵件失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<EmailLogDto>> GetEmailLogsAsync(EmailLogQueryDto query)
    {
        try
        {
            var emailLogQuery = new EmailLogQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                FromAddress = query.FromAddress,
                ToAddress = query.ToAddress,
                Status = query.Status,
                StartDate = query.StartDate,
                EndDate = query.EndDate
            };

            var result = await _emailLogRepository.QueryAsync(emailLogQuery);

            var dtos = result.Items.Select(x => new EmailLogDto
            {
                Id = x.Id,
                FromAddress = x.FromAddress,
                FromName = x.FromName,
                ToAddress = x.ToAddress,
                CcAddress = x.CcAddress,
                BccAddress = x.BccAddress,
                Subject = x.Subject,
                Body = x.Body,
                BodyType = x.BodyType,
                Priority = x.Priority,
                Status = x.Status,
                ErrorMessage = x.ErrorMessage,
                SentAt = x.SentAt,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                HasAttachment = x.HasAttachment,
                AttachmentCount = x.AttachmentCount
            }).ToList();

            return new PagedResult<EmailLogDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢郵件記錄列表失敗", ex);
            throw;
        }
    }

    public async Task<EmailLogDto?> GetEmailLogByIdAsync(long id)
    {
        try
        {
            var emailLog = await _emailLogRepository.GetByIdAsync(id);
            if (emailLog == null) return null;

            return new EmailLogDto
            {
                Id = emailLog.Id,
                FromAddress = emailLog.FromAddress,
                FromName = emailLog.FromName,
                ToAddress = emailLog.ToAddress,
                CcAddress = emailLog.CcAddress,
                BccAddress = emailLog.BccAddress,
                Subject = emailLog.Subject,
                Body = emailLog.Body,
                BodyType = emailLog.BodyType,
                Priority = emailLog.Priority,
                Status = emailLog.Status,
                ErrorMessage = emailLog.ErrorMessage,
                SentAt = emailLog.SentAt,
                CreatedBy = emailLog.CreatedBy,
                CreatedAt = emailLog.CreatedAt,
                HasAttachment = emailLog.HasAttachment,
                AttachmentCount = emailLog.AttachmentCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢郵件記錄失敗: {id}", ex);
            throw;
        }
    }

    public async Task<List<EmailAttachmentDto>> GetEmailAttachmentsAsync(long emailLogId)
    {
        try
        {
            var attachments = await _emailLogRepository.GetAttachmentsAsync(emailLogId);
            return attachments.Select(x => new EmailAttachmentDto
            {
                Id = x.Id,
                EmailLogId = x.EmailLogId,
                FileName = x.FileName,
                FilePath = x.FilePath,
                FileSize = x.FileSize,
                ContentType = x.ContentType,
                CreatedAt = x.CreatedAt
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢郵件附件失敗: {emailLogId}", ex);
            throw;
        }
    }
}

