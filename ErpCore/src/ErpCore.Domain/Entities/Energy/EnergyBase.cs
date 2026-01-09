namespace ErpCore.Domain.Entities.Energy;

/// <summary>
/// 能源基礎資料 (SYSO100-SYSO130 - 能源基礎功能)
/// </summary>
public class EnergyBase
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 能源編號
    /// </summary>
    public string EnergyId { get; set; } = string.Empty;

    /// <summary>
    /// 能源名稱
    /// </summary>
    public string EnergyName { get; set; } = string.Empty;

    /// <summary>
    /// 能源類型 (ELECTRICITY:電力, WATER:水, GAS:瓦斯, OTHER:其他)
    /// </summary>
    public string? EnergyType { get; set; }

    /// <summary>
    /// 單位 (KWH, M3, LITER等)
    /// </summary>
    public string? Unit { get; set; }

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
}

