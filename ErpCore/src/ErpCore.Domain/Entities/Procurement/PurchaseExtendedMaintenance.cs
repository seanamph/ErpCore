namespace ErpCore.Domain.Entities.Procurement;

/// <summary>
/// 採購擴展維護主檔 (SYSPA10-SYSPB60)
/// </summary>
public class PurchaseExtendedMaintenance
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 維護代碼
    /// </summary>
    public string MaintenanceId { get; set; } = string.Empty;

    /// <summary>
    /// 維護名稱
    /// </summary>
    public string MaintenanceName { get; set; } = string.Empty;

    /// <summary>
    /// 維護類型
    /// </summary>
    public string? MaintenanceType { get; set; }

    /// <summary>
    /// 維護說明
    /// </summary>
    public string? MaintenanceDesc { get; set; }

    /// <summary>
    /// 維護配置 (JSON格式)
    /// </summary>
    public string? MaintenanceConfig { get; set; }

    /// <summary>
    /// 參數配置 (JSON格式)
    /// </summary>
    public string? ParameterConfig { get; set; }

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";

    /// <summary>
    /// 排序序號
    /// </summary>
    public int SeqNo { get; set; }

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
