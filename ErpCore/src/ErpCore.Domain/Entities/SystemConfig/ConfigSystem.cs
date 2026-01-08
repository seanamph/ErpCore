namespace ErpCore.Domain.Entities.SystemConfig;

/// <summary>
/// 主系統項目資料實體 (CFG0410)
/// </summary>
public class ConfigSystem
{
    /// <summary>
    /// 主系統代碼
    /// </summary>
    public string SystemId { get; set; } = string.Empty;

    /// <summary>
    /// 主系統名稱
    /// </summary>
    public string SystemName { get; set; } = string.Empty;

    /// <summary>
    /// 排序序號
    /// </summary>
    public int? SeqNo { get; set; }

    /// <summary>
    /// 系統型態
    /// </summary>
    public string? SystemType { get; set; }

    /// <summary>
    /// 伺服器主機名稱
    /// </summary>
    public string? ServerIp { get; set; }

    /// <summary>
    /// 模組代碼
    /// </summary>
    public string? ModuleId { get; set; }

    /// <summary>
    /// 資料庫使用者
    /// </summary>
    public string? DbUser { get; set; }

    /// <summary>
    /// 資料庫密碼（加密儲存）
    /// </summary>
    public string? DbPass { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";

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

