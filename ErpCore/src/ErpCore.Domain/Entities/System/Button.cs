namespace ErpCore.Domain.Entities.System;

/// <summary>
/// 系統功能按鈕資料實體 (SYS0440)
/// </summary>
public class Button
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 作業代碼
    /// </summary>
    public string ProgramId { get; set; } = string.Empty;

    /// <summary>
    /// 按鈕代碼
    /// </summary>
    public string ButtonId { get; set; } = string.Empty;

    /// <summary>
    /// 按鈕名稱
    /// </summary>
    public string ButtonName { get; set; } = string.Empty;

    /// <summary>
    /// 頁面代碼
    /// </summary>
    public string? PageId { get; set; }

    /// <summary>
    /// 按鈕訊息
    /// </summary>
    public string? ButtonMsg { get; set; }

    /// <summary>
    /// 按鈕屬性
    /// </summary>
    public string? ButtonAttr { get; set; }

    /// <summary>
    /// 網頁鏈結位址
    /// </summary>
    public string? ButtonUrl { get; set; }

    /// <summary>
    /// 訊息型態
    /// </summary>
    public string? MsgType { get; set; }

    /// <summary>
    /// 狀態 (1:啟用, 0:停用)
    /// </summary>
    public string Status { get; set; } = "1";

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新者
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// 建立者等級
    /// </summary>
    public int? CreatedPriority { get; set; }

    /// <summary>
    /// 建立者群組
    /// </summary>
    public string? CreatedGroup { get; set; }
}
