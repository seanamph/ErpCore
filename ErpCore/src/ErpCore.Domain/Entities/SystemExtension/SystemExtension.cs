namespace ErpCore.Domain.Entities.SystemExtension;

/// <summary>
/// 系統擴展資料實體 (SYSX110, SYSX120, SYSX140)
/// </summary>
public class SystemExtension
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 擴展功能代碼
    /// </summary>
    public string ExtensionId { get; set; } = string.Empty;

    /// <summary>
    /// 擴展功能名稱
    /// </summary>
    public string ExtensionName { get; set; } = string.Empty;

    /// <summary>
    /// 擴展類型
    /// </summary>
    public string? ExtensionType { get; set; }

    /// <summary>
    /// 擴展值
    /// </summary>
    public string? ExtensionValue { get; set; }

    /// <summary>
    /// 擴展設定 (JSON格式)
    /// </summary>
    public string? ExtensionConfig { get; set; }

    /// <summary>
    /// 排序序號
    /// </summary>
    public int? SeqNo { get; set; }

    /// <summary>
    /// 狀態 (1:啟用, 0:停用)
    /// </summary>
    public string Status { get; set; } = "1";

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

