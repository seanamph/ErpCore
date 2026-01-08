namespace ErpCore.Domain.Entities.Certificate;

/// <summary>
/// 憑證明細 (SYSK110-SYSK150)
/// </summary>
public class VoucherDetail
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 憑證編號
    /// </summary>
    public string VoucherId { get; set; } = string.Empty;

    /// <summary>
    /// 行號 (LINE_NUM)
    /// </summary>
    public int LineNum { get; set; }

    /// <summary>
    /// 會計科目代碼 (ACCOUNT_ID)
    /// </summary>
    public string AccountId { get; set; } = string.Empty;

    /// <summary>
    /// 借方金額 (DEBIT_AMT)
    /// </summary>
    public decimal DebitAmount { get; set; }

    /// <summary>
    /// 貸方金額 (CREDIT_AMT)
    /// </summary>
    public decimal CreditAmount { get; set; }

    /// <summary>
    /// 摘要 (DESCRIPTION)
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Memo { get; set; }

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

