using ErpCore.Application.DTOs.CustomerInvoice;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.CustomerInvoice;
using ErpCore.Infrastructure.Repositories.CustomerInvoice;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using System.Net;
using System.Net.Mail;
using System.Text.Json;

namespace ErpCore.Application.Services.CustomerInvoice;

/// <summary>
/// 郵件傳真服務實作 (SYS2000 - 郵件傳真作業)
/// </summary>
public class MailFaxService : BaseService, IMailFaxService
{
    private readonly IMailFaxRepository _repository;
    private readonly IConfiguration _configuration;

    public MailFaxService(
        IMailFaxRepository repository,
        ILoggerService logger,
        IUserContext userContext,
        IConfiguration configuration) : base(logger, userContext)
    {
        _repository = repository;
        _configuration = configuration;
    }

    public async Task<PagedResult<EmailFaxJobDto>> GetEmailFaxJobsAsync(EmailFaxJobQueryDto query)
    {
        try
        {
            var repositoryQuery = new EmailFaxJobQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                JobId = query.JobId,
                JobType = query.JobType,
                Status = query.Status,
                SendDateFrom = query.SendDateFrom,
                SendDateTo = query.SendDateTo
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<EmailFaxJobDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢郵件傳真作業列表失敗", ex);
            throw;
        }
    }

    public async Task<EmailFaxJobDto> GetEmailFaxJobByIdAsync(string jobId)
    {
        try
        {
            var job = await _repository.GetByIdAsync(jobId);
            if (job == null)
            {
                throw new KeyNotFoundException($"郵件傳真作業不存在: {jobId}");
            }

            return MapToDto(job);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢郵件傳真作業失敗: {jobId}", ex);
            throw;
        }
    }

    public async Task<string> SendEmailAsync(SendEmailRequestDto request)
    {
        try
        {
            var jobId = $"EMAIL_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid():N}";
            var job = new EmailFaxJob
            {
                JobId = jobId,
                JobType = "EMAIL",
                Subject = request.Subject,
                Recipient = request.Recipient,
                Cc = request.Cc,
                Bcc = request.Bcc,
                Content = request.Content,
                AttachmentPath = request.AttachmentPaths != null ? string.Join(";", request.AttachmentPaths) : null,
                Status = "PENDING",
                ScheduleDate = request.ScheduleDate,
                TemplateId = request.TemplateId,
                MaxRetry = 3,
                CreatedBy = GetCurrentUserId()
            };

            await _repository.CreateAsync(job);

            try
            {
                // 實作郵件發送邏輯
                await SendEmailInternalAsync(job, request.AttachmentPaths);
                await _repository.UpdateStatusAsync(jobId, "SENT", DateTime.Now);
                _logger.LogInfo($"發送郵件成功: {jobId}");
            }
            catch (Exception ex)
            {
                await _repository.UpdateStatusAsync(jobId, "FAILED", null, ex.Message);
                _logger.LogError($"發送郵件失敗: {jobId}", ex);
                throw;
            }

            return jobId;
        }
        catch (Exception ex)
        {
            _logger.LogError("發送郵件失敗", ex);
            throw;
        }
    }

    public async Task<string> SendFaxAsync(SendFaxRequestDto request)
    {
        try
        {
            var jobId = $"FAX_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid():N}";
            var job = new EmailFaxJob
            {
                JobId = jobId,
                JobType = "FAX",
                Subject = request.Subject,
                Recipient = request.FaxNumber,
                Content = request.Content,
                AttachmentPath = request.AttachmentPaths != null ? string.Join(";", request.AttachmentPaths) : null,
                Status = "PENDING",
                ScheduleDate = request.ScheduleDate,
                TemplateId = request.TemplateId,
                MaxRetry = 3,
                CreatedBy = GetCurrentUserId()
            };

            await _repository.CreateAsync(job);

            try
            {
                // 實作傳真發送邏輯（需要外部傳真服務）
                await SendFaxInternalAsync(job, request.AttachmentPaths);
                await _repository.UpdateStatusAsync(jobId, "SENT", DateTime.Now);
                _logger.LogInfo($"發送傳真成功: {jobId}");
            }
            catch (Exception ex)
            {
                await _repository.UpdateStatusAsync(jobId, "FAILED", null, ex.Message);
                _logger.LogError($"發送傳真失敗: {jobId}", ex);
                throw;
            }

            return jobId;
        }
        catch (Exception ex)
        {
            _logger.LogError("發送傳真失敗", ex);
            throw;
        }
    }

    public async Task<IEnumerable<EmailFaxLogDto>> GetLogsAsync(string jobId)
    {
        try
        {
            var logs = await _repository.GetLogsByJobIdAsync(jobId);
            return logs.Select(x => new EmailFaxLogDto
            {
                TKey = x.TKey,
                JobId = x.JobId,
                LogDate = x.LogDate,
                LogType = x.LogType,
                LogMessage = x.LogMessage
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢發送記錄失敗: {jobId}", ex);
            throw;
        }
    }

    private EmailFaxJobDto MapToDto(EmailFaxJob job)
    {
        return new EmailFaxJobDto
        {
            TKey = job.TKey,
            JobId = job.JobId,
            JobType = job.JobType,
            Subject = job.Subject,
            Recipient = job.Recipient,
            Cc = job.Cc,
            Bcc = job.Bcc,
            Content = job.Content,
            AttachmentPath = job.AttachmentPath,
            Status = job.Status,
            SendDate = job.SendDate,
            SendUser = job.SendUser,
            ErrorMessage = job.ErrorMessage,
            RetryCount = job.RetryCount,
            MaxRetry = job.MaxRetry,
            ScheduleDate = job.ScheduleDate,
            TemplateId = job.TemplateId,
            Memo = job.Memo
        };
    }

    /// <summary>
    /// 內部郵件發送方法
    /// </summary>
    private async Task SendEmailInternalAsync(EmailFaxJob job, IEnumerable<string>? attachmentPaths)
    {
        try
        {
            var smtpHost = _configuration["EmailSettings:SmtpHost"] ?? "localhost";
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "25");
            var smtpUser = _configuration["EmailSettings:SmtpUser"];
            var smtpPassword = _configuration["EmailSettings:SmtpPassword"];
            var fromEmail = _configuration["EmailSettings:FromEmail"] ?? "noreply@erpcore.com";
            var fromName = _configuration["EmailSettings:FromName"] ?? "ErpCore System";

            using var client = new SmtpClient(smtpHost, smtpPort);
            if (!string.IsNullOrEmpty(smtpUser) && !string.IsNullOrEmpty(smtpPassword))
            {
                client.Credentials = new NetworkCredential(smtpUser, smtpPassword);
                client.EnableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"] ?? "false");
            }

            using var message = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = job.Subject,
                Body = job.Content,
                IsBodyHtml = job.Content?.Contains("<html>") == true || job.Content?.Contains("<body>") == true
            };

            // 設定收件人
            if (!string.IsNullOrEmpty(job.Recipient))
            {
                foreach (var recipient in job.Recipient.Split(';', ','))
                {
                    if (!string.IsNullOrWhiteSpace(recipient))
                        message.To.Add(recipient.Trim());
                }
            }

            // 設定副本
            if (!string.IsNullOrEmpty(job.Cc))
            {
                foreach (var cc in job.Cc.Split(';', ','))
                {
                    if (!string.IsNullOrWhiteSpace(cc))
                        message.CC.Add(cc.Trim());
                }
            }

            // 設定密件副本
            if (!string.IsNullOrEmpty(job.Bcc))
            {
                foreach (var bcc in job.Bcc.Split(';', ','))
                {
                    if (!string.IsNullOrWhiteSpace(bcc))
                        message.Bcc.Add(bcc.Trim());
                }
            }

            // 加入附件
            if (attachmentPaths != null)
            {
                foreach (var attachmentPath in attachmentPaths)
                {
                    if (File.Exists(attachmentPath))
                    {
                        message.Attachments.Add(new Attachment(attachmentPath));
                    }
                }
            }

            await client.SendMailAsync(message);
            _logger.LogInfo($"郵件發送成功: {job.JobId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"郵件發送失敗: {job.JobId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 內部傳真發送方法
    /// </summary>
    private async Task SendFaxInternalAsync(EmailFaxJob job, IEnumerable<string>? attachmentPaths)
    {
        try
        {
            // 傳真發送需要外部服務（如傳真服務器或第三方API）
            // 這裡提供基本框架，實際需要根據使用的傳真服務進行實作
            var faxApiUrl = _configuration["FaxSettings:ApiUrl"];
            var faxApiKey = _configuration["FaxSettings:ApiKey"];

            if (string.IsNullOrEmpty(faxApiUrl))
            {
                throw new InvalidOperationException("傳真服務未配置，請在 appsettings.json 中設定 FaxSettings:ApiUrl");
            }

            // TODO: 實作實際的傳真API呼叫
            // 範例：使用 HttpClient 呼叫傳真服務API
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {faxApiKey}");

            var faxNumber = job.Recipient;
            var content = job.Content;

            // 建立傳真請求
            var faxRequest = new
            {
                FaxNumber = faxNumber,
                Content = content,
                Subject = job.Subject,
                Attachments = attachmentPaths?.ToList() ?? new List<string>()
            };

            // 發送傳真請求
            var response = await httpClient.PostAsJsonAsync($"{faxApiUrl}/api/fax/send", faxRequest);
            response.EnsureSuccessStatusCode();

            _logger.LogInfo($"傳真發送成功: {job.JobId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"傳真發送失敗: {job.JobId}", ex);
            throw;
        }
    }
}

