namespace ErpCore.Domain.Entities.Accounting;

/// <summary>
/// 傳票明細實體 (SYSN120)
/// </summary>
public class VoucherDetail
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
    /// 序號
    /// </summary>
    public int SeqNo { get; set; }

    /// <summary>
    /// 會計科目代號
    /// </summary>
    public string? StypeId { get; set; }

    /// <summary>
    /// 借/貸 (D:借方, C:貸方)
    /// </summary>
    public string Dc { get; set; } = string.Empty;

    /// <summary>
    /// 金額
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 摘要
    /// </summary>
    public string? Description { get; set; }

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

