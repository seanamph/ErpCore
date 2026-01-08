namespace ErpCore.Domain.Entities.System;

/// <summary>
/// 按鈕操作記錄實體 (SYS0790)
/// </summary>
public class ButtonLog
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 使用者編號
    /// </summary>
    public string BUser { get; set; } = string.Empty;

    /// <summary>
    /// 操作時間
    /// </summary>
    public DateTime BTime { get; set; }

    /// <summary>
    /// 作業代碼
    /// </summary>
    public string? ProgId { get; set; }

    /// <summary>
    /// 作業名稱
    /// </summary>
    public string? ProgName { get; set; }

    /// <summary>
    /// 按鈕名稱
    /// </summary>
    public string? ButtonName { get; set; }

    /// <summary>
    /// 網頁位址
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// 框架名稱
    /// </summary>
    public string? FrameName { get; set; }
}

