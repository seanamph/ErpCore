using ErpCore.Application.DTOs.Communication;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Communication;
using ErpCore.Infrastructure.Repositories.Communication;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Communication;

/// <summary>
/// 郵件佇列服務實作
/// </summary>
public class EmailQueueService : BaseService, IEmailQueueService
{
    private readonly IEmailQueueRepository _emailQueueRepository;
    private readonly IEmailLogRepository _emailLogRepository;
    private readonly IEmailService _emailService;

    public EmailQueueService(
        IEmailQueueRepository emailQueueRepository,
        IEmailLogRepository emailLogRepository,
        IEmailService emailService,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _emailQueueRepository = emailQueueRepository;
        _emailLogRepository = emailLogRepository;
        _emailService = emailService;
    }

    public async Task<ProcessEmailQueueResponseDto> ProcessQueueAsync(ProcessEmailQueueRequestDto request)
    {
        try
        {
            _logger.LogInfo($"開始處理郵件佇列，批次大小: {request.BatchSize}");

            var result = new ProcessEmailQueueResponseDto();
            var pendingEmails = await _emailQueueRepository.GetPendingEmailsAsync(request.BatchSize);

            foreach (var queueItem in pendingEmails)
            {
                try
                {
                    // 更新狀態為處理中
                    await _emailQueueRepository.UpdateStatusAsync(queueItem.Id, "Processing", DateTime.Now);

                    // 取得郵件記錄
                    var emailLog = await _emailLogRepository.GetByIdAsync(queueItem.EmailLogId);
                    if (emailLog == null)
                    {
                        await _emailQueueRepository.UpdateStatusAsync(queueItem.Id, "Failed", DateTime.Now, "郵件記錄不存在");
                        result.FailedCount++;
                        continue;
                    }

                    // 實作實際的郵件發送邏輯
                    var sendRequest = new SendEmailRequestDto
                    {
                        FromAddress = emailLog.FromAddress,
                        FromName = emailLog.FromName,
                        ToAddress = emailLog.ToAddress,
                        CcAddress = emailLog.CcAddress,
                        BccAddress = emailLog.BccAddress,
                        Subject = emailLog.Subject,
                        Body = emailLog.Body,
                        BodyType = emailLog.BodyType
                    };
                    
                    await _emailService.SendEmailAsync(sendRequest);
                    await _emailQueueRepository.UpdateStatusAsync(queueItem.Id, "Sent", DateTime.Now);

                    result.SuccessCount++;
                    result.ProcessedCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"處理郵件佇列項目失敗: {queueItem.Id}", ex);

                    // 檢查是否需要重試
                    if (queueItem.RetryCount < request.MaxRetryCount)
                    {
                        var nextRetryAt = DateTime.Now.AddMinutes(Math.Pow(2, queueItem.RetryCount));
                        await _emailQueueRepository.UpdateRetryAsync(queueItem.Id, queueItem.RetryCount + 1, nextRetryAt, ex.Message);
                        result.RetryCount++;
                    }
                    else
                    {
                        await _emailQueueRepository.UpdateStatusAsync(queueItem.Id, "Failed", DateTime.Now, ex.Message);
                        result.FailedCount++;
                    }
                    result.ProcessedCount++;
                }
            }

            _logger.LogInfo($"郵件佇列處理完成，成功: {result.SuccessCount}, 失敗: {result.FailedCount}, 重試: {result.RetryCount}");

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("處理郵件佇列失敗", ex);
            throw;
        }
    }

    public async Task<ProcessEmailQueueResponseDto> RetryFailedEmailsAsync(RetryFailedEmailsRequestDto request)
    {
        try
        {
            _logger.LogInfo($"重試失敗郵件，數量: {request.EmailQueueIds.Count}");

            var result = new ProcessEmailQueueResponseDto();

            foreach (var queueId in request.EmailQueueIds)
            {
                try
                {
                    var queueItem = await _emailQueueRepository.GetByIdAsync(queueId);
                    if (queueItem == null || queueItem.Status != "Failed")
                    {
                        continue;
                    }

                    // 重置佇列項目狀態
                    await _emailQueueRepository.UpdateRetryAsync(queueId, 0, DateTime.Now, null);
                    result.ProcessedCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"重試郵件失敗: {queueId}", ex);
                    result.FailedCount++;
                }
            }

            // 處理重試的郵件
            var processRequest = new ProcessEmailQueueRequestDto
            {
                BatchSize = request.EmailQueueIds.Count,
                MaxRetryCount = request.MaxRetryCount
            };
            var processResult = await ProcessQueueAsync(processRequest);

            result.SuccessCount += processResult.SuccessCount;
            result.RetryCount += processResult.RetryCount;
            result.FailedCount += processResult.FailedCount;

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("重試失敗郵件失敗", ex);
            throw;
        }
    }

    public async Task<EmailQueueStatusDto> GetQueueStatusAsync()
    {
        try
        {
            var status = await _emailQueueRepository.GetQueueStatusAsync();
            return new EmailQueueStatusDto
            {
                PendingCount = status.PendingCount,
                ProcessingCount = status.ProcessingCount,
                SentCount = status.SentCount,
                FailedCount = status.FailedCount,
                TotalCount = status.TotalCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢郵件佇列狀態失敗", ex);
            throw;
        }
    }
}

