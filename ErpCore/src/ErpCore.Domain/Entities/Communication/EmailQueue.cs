namespace ErpCore.Domain.Entities.Communication;

/// <summary>
/// 郵件佇列實體
/// </summary>
public class EmailQueue
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 郵件記錄ID
    /// </summary>
    public long EmailLogId { get; set; }

    /// <summary>
    /// 優先權 (1-5, 1最高)
    /// </summary>
    public int Priority { get; set; } = 3;

    /// <summary>
    /// 重試次數
    /// </summary>
    public int RetryCount { get; set; } = 0;

    /// <summary>
    /// 最大重試次數
    /// </summary>
    public int MaxRetryCount { get; set; } = 3;

    /// <summary>
    /// 狀態 (Pending/Processing/Sent/Failed)
    /// </summary>
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// 下次重試時間
    /// </summary>
    public DateTime? NextRetryAt { get; set; }

    /// <summary>
    /// 處理時間
    /// </summary>
    public DateTime? ProcessedAt { get; set; }

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

