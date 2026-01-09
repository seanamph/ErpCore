namespace ErpCore.Domain.Entities.CustomerCustom;

/// <summary>
/// SYS8000.ESKYLAND 租賃實體
/// ESKYLAND客戶定制版本，功能類似SYS8000但針對ESKYLAND客戶場景優化
/// </summary>
public class Sys8000EskylandLease
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 租賃編號
    /// </summary>
    public string LeaseId { get; set; } = string.Empty;

    /// <summary>
    /// 租賃名稱
    /// </summary>
    public string LeaseName { get; set; } = string.Empty;

    /// <summary>
    /// ESKYLAND特定欄位
    /// </summary>
    public string? EskylandSpecificField { get; set; }

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

