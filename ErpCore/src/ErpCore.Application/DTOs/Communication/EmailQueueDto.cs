namespace ErpCore.Application.DTOs.Communication;

/// <summary>
/// 處理郵件佇列請求 DTO
/// </summary>
public class ProcessEmailQueueRequestDto
{
    public int BatchSize { get; set; } = 100;
    public int MaxRetryCount { get; set; } = 3;
}

/// <summary>
/// 處理郵件佇列回應 DTO
/// </summary>
public class ProcessEmailQueueResponseDto
{
    public int ProcessedCount { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public int RetryCount { get; set; }
}

/// <summary>
/// 郵件佇列狀態 DTO
/// </summary>
public class EmailQueueStatusDto
{
    public int PendingCount { get; set; }
    public int ProcessingCount { get; set; }
    public int SentCount { get; set; }
    public int FailedCount { get; set; }
    public int TotalCount { get; set; }
}

/// <summary>
/// 重試失敗郵件請求 DTO
/// </summary>
public class RetryFailedEmailsRequestDto
{
    public List<long> EmailQueueIds { get; set; } = new();
    public int MaxRetryCount { get; set; } = 3;
}

