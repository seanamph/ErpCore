using ErpCore.Application.DTOs.Communication;

namespace ErpCore.Application.Services.Communication;

/// <summary>
/// 郵件佇列服務介面
/// </summary>
public interface IEmailQueueService
{
    /// <summary>
    /// 處理郵件佇列
    /// </summary>
    Task<ProcessEmailQueueResponseDto> ProcessQueueAsync(ProcessEmailQueueRequestDto request);

    /// <summary>
    /// 重試失敗郵件
    /// </summary>
    Task<ProcessEmailQueueResponseDto> RetryFailedEmailsAsync(RetryFailedEmailsRequestDto request);

    /// <summary>
    /// 取得佇列狀態
    /// </summary>
    Task<EmailQueueStatusDto> GetQueueStatusAsync();
}

