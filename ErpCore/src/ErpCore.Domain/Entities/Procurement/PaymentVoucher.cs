namespace ErpCore.Domain.Entities.Procurement;

/// <summary>
/// 付款單主檔 (SYSP271-SYSP2B0)
/// </summary>
public class PaymentVoucher
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 付款單號
    /// </summary>
    public string PaymentNo { get; set; } = string.Empty;

    /// <summary>
    /// 付款日期
    /// </summary>
    public DateTime PaymentDate { get; set; }

    /// <summary>
    /// 供應商編號
    /// </summary>
    public string SupplierId { get; set; } = string.Empty;

    /// <summary>
    /// 付款金額
    /// </summary>
    public decimal PaymentAmount { get; set; }

    /// <summary>
    /// 付款方式
    /// </summary>
    public string? PaymentMethod { get; set; }

    /// <summary>
    /// 銀行帳號
    /// </summary>
    public string? BankAccount { get; set; }

    /// <summary>
    /// 狀態 (DRAFT:草稿, CONFIRMED:確認, PAID:已付款)
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
