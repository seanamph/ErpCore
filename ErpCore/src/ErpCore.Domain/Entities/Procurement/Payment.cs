namespace ErpCore.Domain.Entities.Procurement;

/// <summary>
/// 付款單主檔 (SYSP271-SYSP2B0)
/// </summary>
public class Payment
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 付款單號
    /// </summary>
    public string PaymentId { get; set; } = string.Empty;

    /// <summary>
    /// 付款日期
    /// </summary>
    public DateTime PaymentDate { get; set; }

    /// <summary>
    /// 供應商編號
    /// </summary>
    public string SupplierId { get; set; } = string.Empty;

    /// <summary>
    /// 付款類型
    /// </summary>
    public string PaymentType { get; set; } = string.Empty;

    /// <summary>
    /// 付款金額
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 幣別
    /// </summary>
    public string? CurrencyId { get; set; }

    /// <summary>
    /// 匯率
    /// </summary>
    public decimal? ExchangeRate { get; set; }

    /// <summary>
    /// 銀行帳戶編號
    /// </summary>
    public string? BankAccountId { get; set; }

    /// <summary>
    /// 支票號碼
    /// </summary>
    public string? CheckNumber { get; set; }

    /// <summary>
    /// 狀態 (D:草稿, A:已確認, C:已取消)
    /// </summary>
    public string Status { get; set; } = "D";

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

