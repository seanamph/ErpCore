namespace ErpCore.Domain.Entities.CustomerCustomJgjn;

/// <summary>
/// JGJN資料實體 (SYSCUST_JGJN系列)
/// </summary>
public class JgjNData
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
    /// 模組代碼 (SYS1610, SYS1620, SYS1630, SYS1640, SYS1645, SYS1646, SYSC210等)
    /// </summary>
    public string ModuleCode { get; set; } = string.Empty;

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

