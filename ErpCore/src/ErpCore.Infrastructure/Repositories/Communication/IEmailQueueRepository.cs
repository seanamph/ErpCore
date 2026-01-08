using ErpCore.Domain.Entities.Communication;

namespace ErpCore.Infrastructure.Repositories.Communication;

/// <summary>
/// 郵件佇列儲存庫介面
/// </summary>
public interface IEmailQueueRepository
{
    /// <summary>
    /// 建立郵件佇列項目
    /// </summary>
    Task<EmailQueue> CreateAsync(EmailQueue entity);

    /// <summary>
    /// 根據ID查詢郵件佇列項目
    /// </summary>
    Task<EmailQueue?> GetByIdAsync(long id);

    /// <summary>
    /// 取得待處理的郵件佇列（按優先權排序）
    /// </summary>
    Task<IEnumerable<EmailQueue>> GetPendingEmailsAsync(int batchSize, DateTime? beforeDate = null);

    /// <summary>
    /// 更新佇列狀態
    /// </summary>
    Task<bool> UpdateStatusAsync(long id, string status, DateTime? processedAt = null, string? errorMessage = null);

    /// <summary>
    /// 更新重試資訊
    /// </summary>
    Task<bool> UpdateRetryAsync(long id, int retryCount, DateTime? nextRetryAt, string? errorMessage = null);

    /// <summary>
    /// 取得佇列狀態統計
    /// </summary>
    Task<EmailQueueStatus> GetQueueStatusAsync();
}

/// <summary>
/// 郵件佇列狀態統計
/// </summary>
public class EmailQueueStatus
{
    public int PendingCount { get; set; }
    public int ProcessingCount { get; set; }
    public int SentCount { get; set; }
    public int FailedCount { get; set; }
    public int TotalCount { get; set; }
}

