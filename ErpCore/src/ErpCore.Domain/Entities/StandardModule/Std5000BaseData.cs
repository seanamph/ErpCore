namespace ErpCore.Domain.Entities.StandardModule;

/// <summary>
/// STD5000 基礎資料實體 (SYS5110-SYS5150 - 基礎資料維護)
/// </summary>
public class Std5000BaseData
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 資料代碼
    /// </summary>
    public string DataId { get; set; } = string.Empty;

    /// <summary>
    /// 資料名稱
    /// </summary>
    public string DataName { get; set; } = string.Empty;

    /// <summary>
    /// 資料類型 (SYS5110, SYS5120, SYS5130, SYS5140, SYS5150)
    /// </summary>
    public string DataType { get; set; } = string.Empty;

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";

    /// <summary>
    /// 備註
    /// </summary>
    public string? Memo { get; set; }

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
}

