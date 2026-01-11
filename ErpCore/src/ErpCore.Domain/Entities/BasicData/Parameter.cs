namespace ErpCore.Domain.Entities.BasicData;

/// <summary>
/// 參數資料實體 (SYSBC40)
/// </summary>
public class Parameter
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 參數標題
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 參數標籤/代碼
    /// </summary>
    public string Tag { get; set; } = string.Empty;

    /// <summary>
    /// 排序序號
    /// </summary>
    public int? SeqNo { get; set; }

    /// <summary>
    /// 參數內容
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// 多語言參數內容
    /// </summary>
    public string? Content2 { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// 狀態 (1:啟用, 0:停用)
    /// </summary>
    public string Status { get; set; } = "1";

    /// <summary>
    /// 只讀標誌 (1:只讀, 0:可編輯)
    /// </summary>
    public string? ReadOnly { get; set; } = "0";

    /// <summary>
    /// 系統ID
    /// </summary>
    public string? SystemId { get; set; }

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
