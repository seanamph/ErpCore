namespace ErpCore.Domain.Entities.UiComponent;

/// <summary>
/// UI組件設定實體
/// </summary>
public class UIComponent
{
    /// <summary>
    /// 組件ID
    /// </summary>
    public long ComponentId { get; set; }

    /// <summary>
    /// 組件代碼
    /// </summary>
    public string ComponentCode { get; set; } = string.Empty;

    /// <summary>
    /// 組件名稱
    /// </summary>
    public string ComponentName { get; set; } = string.Empty;

    /// <summary>
    /// 組件類型 (FB, FI, FU, FQ, PR, FS)
    /// </summary>
    public string ComponentType { get; set; } = string.Empty;

    /// <summary>
    /// 組件版本 (V1, V2)
    /// </summary>
    public string ComponentVersion { get; set; } = "V1";

    /// <summary>
    /// 組件配置 (JSON格式)
    /// </summary>
    public string? ConfigJson { get; set; }

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
}

