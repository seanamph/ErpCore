namespace ErpCore.Domain.Entities.MirModule;

/// <summary>
/// MIRW000 資料表
/// </summary>
public class MirW000Data
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
    /// 資料值
    /// </summary>
    public string? DataValue { get; set; }

    /// <summary>
    /// 資料類型
    /// </summary>
    public string? DataType { get; set; }

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";

    /// <summary>
    /// 排序順序
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Memo { get; set; }

    /// <summary>
    /// 建立人員
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新人員
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

