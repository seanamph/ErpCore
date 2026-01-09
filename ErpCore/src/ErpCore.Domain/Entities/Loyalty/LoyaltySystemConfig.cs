namespace ErpCore.Domain.Entities.Loyalty;

/// <summary>
/// 忠誠度系統設定 (WEBLOYALTYINI - 忠誠度系統初始化)
/// </summary>
public class LoyaltySystemConfig
{
    /// <summary>
    /// 設定編號
    /// </summary>
    public string ConfigId { get; set; } = string.Empty;

    /// <summary>
    /// 設定名稱
    /// </summary>
    public string ConfigName { get; set; } = string.Empty;

    /// <summary>
    /// 設定值
    /// </summary>
    public string? ConfigValue { get; set; }

    /// <summary>
    /// 設定類型 (PARAM, RULE, ENV)
    /// </summary>
    public string ConfigType { get; set; } = string.Empty;

    /// <summary>
    /// 說明
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";

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

