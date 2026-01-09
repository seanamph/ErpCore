namespace ErpCore.Domain.Entities.Loyalty;

/// <summary>
/// 忠誠度系統初始化記錄 (WEBLOYALTYINI - 忠誠度系統初始化)
/// </summary>
public class LoyaltySystemInitLog
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 初始化批次編號
    /// </summary>
    public string InitId { get; set; } = string.Empty;

    /// <summary>
    /// 初始化狀態 (SUCCESS, FAILED)
    /// </summary>
    public string InitStatus { get; set; } = string.Empty;

    /// <summary>
    /// 初始化日期
    /// </summary>
    public DateTime InitDate { get; set; }

    /// <summary>
    /// 初始化訊息
    /// </summary>
    public string? InitMessage { get; set; }

    /// <summary>
    /// 建立人員
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

