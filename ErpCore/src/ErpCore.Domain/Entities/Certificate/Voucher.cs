namespace ErpCore.Domain.Entities.Certificate;

/// <summary>
/// 憑證主檔 (SYSK110-SYSK150)
/// </summary>
public class Voucher
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 憑證編號 (VCH_NO)
    /// </summary>
    public string VoucherId { get; set; } = string.Empty;

    /// <summary>
    /// 憑證日期 (VCH_DATE)
    /// </summary>
    public DateTime VoucherDate { get; set; }

    /// <summary>
    /// 憑證類型 (VCH_TYPE, V:傳票, R:收據, I:發票)
    /// </summary>
    public string VoucherType { get; set; } = string.Empty;

    /// <summary>
    /// 分店代碼 (SHOP_ID)
    /// </summary>
    public string ShopId { get; set; } = string.Empty;

    /// <summary>
    /// 狀態 (STATUS, D:草稿, S:已送出, A:已審核, X:已取消, C:已結案)
    /// </summary>
    public string Status { get; set; } = "D";

    /// <summary>
    /// 申請人員 (APPLY_USER)
    /// </summary>
    public string? ApplyUserId { get; set; }

    /// <summary>
    /// 申請日期 (APPLY_DATE)
    /// </summary>
    public DateTime? ApplyDate { get; set; }

    /// <summary>
    /// 審核人員 (APPROVE_USER)
    /// </summary>
    public string? ApproveUserId { get; set; }

    /// <summary>
    /// 審核日期 (APPROVE_DATE)
    /// </summary>
    public DateTime? ApproveDate { get; set; }

    /// <summary>
    /// 總金額 (TOTAL_AMT)
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 借方總額 (TOTAL_DEBIT_AMT)
    /// </summary>
    public decimal TotalDebitAmount { get; set; }

    /// <summary>
    /// 貸方總額 (TOTAL_CREDIT_AMT)
    /// </summary>
    public decimal TotalCreditAmount { get; set; }

    /// <summary>
    /// 備註 (MEMO)
    /// </summary>
    public string? Memo { get; set; }

    /// <summary>
    /// 分公司代碼 (SITE_ID)
    /// </summary>
    public string? SiteId { get; set; }

    /// <summary>
    /// 組織代碼 (ORG_ID)
    /// </summary>
    public string? OrgId { get; set; }

    /// <summary>
    /// 幣別 (CURRENCY_ID)
    /// </summary>
    public string CurrencyId { get; set; } = "TWD";

    /// <summary>
    /// 匯率 (EXCHANGE_RATE)
    /// </summary>
    public decimal ExchangeRate { get; set; } = 1;

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

