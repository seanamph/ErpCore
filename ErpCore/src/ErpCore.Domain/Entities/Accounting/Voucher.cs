namespace ErpCore.Domain.Entities.Accounting;

/// <summary>
/// 傳票實體 (SYSN120)
/// </summary>
public class Voucher
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 傳票編號
    /// </summary>
    public string VoucherId { get; set; } = string.Empty;

    /// <summary>
    /// 傳票日期
    /// </summary>
    public DateTime VoucherDate { get; set; }

    /// <summary>
    /// 傳票型態代號
    /// </summary>
    public string? VoucherTypeId { get; set; }

    /// <summary>
    /// 摘要
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 狀態 (D:草稿, P:已過帳, C:已取消)
    /// </summary>
    public string Status { get; set; } = "D";

    /// <summary>
    /// 過帳者
    /// </summary>
    public string? PostedBy { get; set; }

    /// <summary>
    /// 過帳時間
    /// </summary>
    public DateTime? PostedAt { get; set; }

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

    /// <summary>
    /// 建立者等級
    /// </summary>
    public int? CreatedPriority { get; set; }

    /// <summary>
    /// 建立者群組
    /// </summary>
    public string? CreatedGroup { get; set; }
}

