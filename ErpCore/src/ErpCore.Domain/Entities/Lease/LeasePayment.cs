namespace ErpCore.Domain.Entities.Lease;

/// <summary>
/// 租賃付款記錄 (SYS8110-SYS8220)
/// </summary>
public class LeasePayment
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
    /// 付款日期
    /// </summary>
    public DateTime PaymentDate { get; set; }

    /// <summary>
    /// 付款金額
    /// </summary>
    public decimal PaymentAmount { get; set; }

    /// <summary>
    /// 付款方式
    /// </summary>
    public string? PaymentMethod { get; set; }

    /// <summary>
    /// 付款狀態 (P:已付款, U:未付款, O:逾期)
    /// </summary>
    public string? PaymentStatus { get; set; } = "P";

    /// <summary>
    /// 發票號碼
    /// </summary>
    public string? InvoiceNo { get; set; }

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

