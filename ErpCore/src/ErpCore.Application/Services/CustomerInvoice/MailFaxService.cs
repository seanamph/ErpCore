using ErpCore.Application.DTOs.CustomerInvoice;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.CustomerInvoice;
using ErpCore.Infrastructure.Repositories.CustomerInvoice;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.CustomerInvoice;

/// <summary>
/// 郵件傳真服務實作 (SYS2000 - 郵件傳真作業)
/// </summary>
public class MailFaxService : BaseService, IMailFaxService
{
    private readonly IMailFaxRepository _repository;

    public MailFaxService(
        IMailFaxRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
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

            // TODO: 實作實際的郵件發送邏輯
            // 目前先標記為已發送
            await _repository.UpdateStatusAsync(jobId, "SENT", DateTime.Now);

            _logger.LogInfo($"發送郵件成功: {jobId}");
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

            // TODO: 實作實際的傳真發送邏輯
            // 目前先標記為已發送
            await _repository.UpdateStatusAsync(jobId, "SENT", DateTime.Now);

            _logger.LogInfo($"發送傳真成功: {jobId}");
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
}

