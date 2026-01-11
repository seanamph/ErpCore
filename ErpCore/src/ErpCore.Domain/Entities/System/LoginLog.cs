namespace ErpCore.Domain.Entities.System;

/// <summary>
/// 使用者異常登入記錄實體 (SYS0760)
/// </summary>
public class LoginLog
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 異常事件代碼 (1:密碼錯誤等)
    /// </summary>
    public string EventId { get; set; } = string.Empty;

    /// <summary>
    /// 使用者代碼
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// 登入IP位址
    /// </summary>
    public string? LoginIp { get; set; }

    /// <summary>
    /// 事件發生時間
    /// </summary>
    public DateTime EventTime { get; set; }

    /// <summary>
    /// 建立使用者
    /// </summary>
    public string? BUser { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime? BTime { get; set; }

    /// <summary>
    /// 異動使用者
    /// </summary>
    public string? CUser { get; set; }

    /// <summary>
    /// 異動時間
    /// </summary>
    public DateTime? CTime { get; set; }

    /// <summary>
    /// 建立優先權
    /// </summary>
    public int? CPriority { get; set; }

    /// <summary>
    /// 建立群組
    /// </summary>
    public string? CGroup { get; set; }
}
