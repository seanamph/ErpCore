namespace ErpCore.Domain.Entities.Lease;

/// <summary>
/// 租賃條件 (SYSE110-SYSE140)
/// </summary>
public class LeaseTerm
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
    /// 條件類型
    /// </summary>
    public string TermType { get; set; } = string.Empty;

    /// <summary>
    /// 條件名稱
    /// </summary>
    public string? TermName { get; set; }

    /// <summary>
    /// 條件值
    /// </summary>
    public string? TermValue { get; set; }

    /// <summary>
    /// 條件金額
    /// </summary>
    public decimal? TermAmount { get; set; }

    /// <summary>
    /// 條件日期
    /// </summary>
    public DateTime? TermDate { get; set; }

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

