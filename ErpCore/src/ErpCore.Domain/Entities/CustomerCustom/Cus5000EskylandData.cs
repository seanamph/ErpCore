namespace ErpCore.Domain.Entities.CustomerCustom;

/// <summary>
/// CUS5000.ESKYLAND 資料實體
/// ESKYLAND客戶定制版本，功能類似CUS5000但針對ESKYLAND客戶場景優化
/// </summary>
public class Cus5000EskylandData
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 資料編號
    /// </summary>
    public string DataId { get; set; } = string.Empty;

    /// <summary>
    /// 資料名稱
    /// </summary>
    public string DataName { get; set; } = string.Empty;

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

