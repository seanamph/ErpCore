namespace ErpCore.Domain.Entities.Accounting;

/// <summary>
/// 財務交易實體 (SYSN210)
/// </summary>
public class FinancialTransaction
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 交易單號
    /// </summary>
    public string TxnNo { get; set; } = string.Empty;

    /// <summary>
    /// 交易日期
    /// </summary>
    public DateTime TxnDate { get; set; }

    /// <summary>
    /// 交易類型
    /// </summary>
    public string TxnType { get; set; } = string.Empty;

    /// <summary>
    /// 會計科目代號
    /// </summary>
    public string StypeId { get; set; } = string.Empty;

    /// <summary>
    /// 借貸方向 (D:借方, C:貸方)
    /// </summary>
    public string Dc { get; set; } = string.Empty;

    /// <summary>
    /// 交易金額
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 交易說明
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 狀態 (DRAFT:草稿, CONFIRMED:確認, POSTED:過帳)
    /// </summary>
    public string Status { get; set; } = "DRAFT";

    /// <summary>
    /// 審核者
    /// </summary>
    public string? Verifier { get; set; }

    /// <summary>
    /// 審核日期
    /// </summary>
    public DateTime? VerifyDate { get; set; }

    /// <summary>
    /// 過帳者
    /// </summary>
    public string? PostedBy { get; set; }

    /// <summary>
    /// 過帳日期
    /// </summary>
    public DateTime? PostedDate { get; set; }

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

