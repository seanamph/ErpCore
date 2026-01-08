namespace ErpCore.Domain.Entities.Communication;

/// <summary>
/// 簡訊發送記錄實體
/// </summary>
public class SmsLog
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 手機號碼
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// 簡訊內容
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 狀態 (Pending/Sent/Failed)
    /// </summary>
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 發送時間
    /// </summary>
    public DateTime? SentAt { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 簡訊服務提供商
    /// </summary>
    public string? Provider { get; set; }

    /// <summary>
    /// 服務商訊息ID
    /// </summary>
    public string? ProviderMessageId { get; set; }
}

