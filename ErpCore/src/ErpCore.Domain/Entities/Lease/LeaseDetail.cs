namespace ErpCore.Domain.Entities.Lease;

/// <summary>
/// 租賃明細 (SYS8110-SYS8220)
/// </summary>
public class LeaseDetail
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
    /// 行號
    /// </summary>
    public int LineNum { get; set; }

    /// <summary>
    /// 項目類型 (RENT:租金, UTILITY:水電費, OTHER:其他)
    /// </summary>
    public string? ItemType { get; set; }

    /// <summary>
    /// 項目名稱
    /// </summary>
    public string? ItemName { get; set; }

    /// <summary>
    /// 金額
    /// </summary>
    public decimal? Amount { get; set; } = 0;

    /// <summary>
    /// 開始日期
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// 結束日期
    /// </summary>
    public DateTime? EndDate { get; set; }

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

