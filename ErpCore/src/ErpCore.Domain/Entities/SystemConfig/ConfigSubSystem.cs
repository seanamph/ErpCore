namespace ErpCore.Domain.Entities.SystemConfig;

/// <summary>
/// 子系統項目資料實體 (CFG0420)
/// </summary>
public class ConfigSubSystem
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 子系統項目代碼
    /// </summary>
    public string SubSystemId { get; set; } = string.Empty;

    /// <summary>
    /// 子系統項目名稱
    /// </summary>
    public string SubSystemName { get; set; } = string.Empty;

    /// <summary>
    /// 排序序號
    /// </summary>
    public int? SeqNo { get; set; }

    /// <summary>
    /// 主系統代碼
    /// </summary>
    public string SystemId { get; set; } = string.Empty;

    /// <summary>
    /// 上層子系統代碼
    /// </summary>
    public string? ParentSubSystemId { get; set; }

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";

    /// <summary>
    /// 備註
    /// </summary>
    public string? Notes { get; set; }

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

