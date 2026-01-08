namespace ErpCore.Domain.Entities.Lease;

/// <summary>
/// 租賃會計分類 (SYSE110-SYSE140)
/// </summary>
public class LeaseAccountingCategory
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
    /// 版本號
    /// </summary>
    public string Version { get; set; } = "1";

    /// <summary>
    /// 會計分類代碼
    /// </summary>
    public string CategoryId { get; set; } = string.Empty;

    /// <summary>
    /// 會計分類名稱
    /// </summary>
    public string? CategoryName { get; set; }

    /// <summary>
    /// 金額
    /// </summary>
    public decimal? Amount { get; set; }

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

