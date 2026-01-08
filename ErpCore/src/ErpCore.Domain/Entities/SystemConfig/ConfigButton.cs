namespace ErpCore.Domain.Entities.SystemConfig;

/// <summary>
/// 系統功能按鈕資料實體 (CFG0440)
/// </summary>
public class ConfigButton
{
    /// <summary>
    /// 按鈕代碼
    /// </summary>
    public string ButtonId { get; set; } = string.Empty;

    /// <summary>
    /// 作業代碼
    /// </summary>
    public string ProgramId { get; set; } = string.Empty;

    /// <summary>
    /// 按鈕名稱
    /// </summary>
    public string ButtonName { get; set; } = string.Empty;

    /// <summary>
    /// 按鈕型態
    /// </summary>
    public string? ButtonType { get; set; }

    /// <summary>
    /// 排序序號
    /// </summary>
    public int? SeqNo { get; set; }

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
}

