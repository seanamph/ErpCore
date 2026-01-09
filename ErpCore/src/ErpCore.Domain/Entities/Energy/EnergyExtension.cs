namespace ErpCore.Domain.Entities.Energy;

/// <summary>
/// 能源擴展資料 (SYSOU10-SYSOU33 - 能源擴展功能)
/// </summary>
public class EnergyExtension
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 擴展編號
    /// </summary>
    public string ExtensionId { get; set; } = string.Empty;

    /// <summary>
    /// 能源編號
    /// </summary>
    public string EnergyId { get; set; } = string.Empty;

    /// <summary>
    /// 擴展類型
    /// </summary>
    public string? ExtensionType { get; set; }

    /// <summary>
    /// 擴展值
    /// </summary>
    public string? ExtensionValue { get; set; }

    /// <summary>
    /// 狀態
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

