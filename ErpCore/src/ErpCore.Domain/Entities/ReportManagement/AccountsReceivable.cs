namespace ErpCore.Domain.Entities.ReportManagement;

/// <summary>
/// 應收帳款主檔 (SYSR210-SYSR240)
/// </summary>
public class AccountsReceivable
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 傳票主檔KEY值 (VOUCHERM_T_KEY)
    /// </summary>
    public long? VoucherM_TKey { get; set; }

    /// <summary>
    /// 對象別編號 (OBJECT_ID)
    /// </summary>
    public string? ObjectId { get; set; }

    /// <summary>
    /// 對帳KEY值 (ACCT_KEY)
    /// </summary>
    public string? AcctKey { get; set; }

    /// <summary>
    /// 收款日期 (RECEIPT_DATE)
    /// </summary>
    public DateTime? ReceiptDate { get; set; }

    /// <summary>
    /// 收款金額 (RECEIPT_AMOUNT)
    /// </summary>
    public decimal ReceiptAmount { get; set; }

    /// <summary>
    /// 收款項目代號 (ARITEM_ID)
    /// </summary>
    public string? AritemId { get; set; }

    /// <summary>
    /// 收款單號 (RECEIPT_NO)
    /// </summary>
    public string? ReceiptNo { get; set; }

    /// <summary>
    /// 傳票單號 (VOUCHER_NO)
    /// </summary>
    public string? VoucherNo { get; set; }

    /// <summary>
    /// 傳票狀態 (VOUCHER_STATUS)
    /// </summary>
    public string? VoucherStatus { get; set; }

    /// <summary>
    /// 分店代碼 (SHOP_ID)
    /// </summary>
    public string? ShopId { get; set; }

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
    public string? CurrencyId { get; set; }

    /// <summary>
    /// 匯率 (EXCHANGE_RATE)
    /// </summary>
    public decimal ExchangeRate { get; set; }

    /// <summary>
    /// 備註 (NOTES)
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

