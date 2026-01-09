namespace ErpCore.Domain.Entities.CustomerInvoice;

/// <summary>
/// 總帳主檔實體 (SYS2000 - 總帳資料維護)
/// </summary>
public class GeneralLedger
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 總帳編號
    /// </summary>
    public string LedgerId { get; set; } = string.Empty;

    /// <summary>
    /// 總帳日期
    /// </summary>
    public DateTime LedgerDate { get; set; }

    /// <summary>
    /// 會計科目編號
    /// </summary>
    public string AccountId { get; set; } = string.Empty;

    /// <summary>
    /// 憑證號碼
    /// </summary>
    public string? VoucherNo { get; set; }

    /// <summary>
    /// 說明
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 借方金額
    /// </summary>
    public decimal DebitAmount { get; set; }

    /// <summary>
    /// 貸方金額
    /// </summary>
    public decimal CreditAmount { get; set; }

    /// <summary>
    /// 餘額
    /// </summary>
    public decimal Balance { get; set; }

    /// <summary>
    /// 幣別
    /// </summary>
    public string CurrencyId { get; set; } = "TWD";

    /// <summary>
    /// 會計期間 (YYYYMM格式)
    /// </summary>
    public string Period { get; set; } = string.Empty;

    /// <summary>
    /// 狀態 (DRAFT:草稿, POSTED:已過帳, CLOSED:已結帳)
    /// </summary>
    public string Status { get; set; } = "DRAFT";

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

/// <summary>
/// 會計科目實體
/// </summary>
public class Account
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 會計科目編號
    /// </summary>
    public string AccountId { get; set; } = string.Empty;

    /// <summary>
    /// 會計科目名稱
    /// </summary>
    public string AccountName { get; set; } = string.Empty;

    /// <summary>
    /// 科目類型 (ASSET:資產, LIABILITY:負債, EQUITY:權益, REVENUE:收入, EXPENSE:費用)
    /// </summary>
    public string AccountType { get; set; } = string.Empty;

    /// <summary>
    /// 上級科目編號
    /// </summary>
    public string? ParentAccountId { get; set; }

    /// <summary>
    /// 科目層級
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// 是否為末級科目
    /// </summary>
    public bool IsLeaf { get; set; }

    /// <summary>
    /// 狀態
    /// </summary>
    public string Status { get; set; } = "A";

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

/// <summary>
/// 會計憑證實體
/// </summary>
public class Voucher
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 憑證號碼
    /// </summary>
    public string VoucherNo { get; set; } = string.Empty;

    /// <summary>
    /// 憑證日期
    /// </summary>
    public DateTime VoucherDate { get; set; }

    /// <summary>
    /// 憑證類型 (GENERAL:一般, ADJUSTMENT:調整, CLOSING:結帳)
    /// </summary>
    public string VoucherType { get; set; } = string.Empty;

    /// <summary>
    /// 說明
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 借方總額
    /// </summary>
    public decimal TotalDebitAmount { get; set; }

    /// <summary>
    /// 貸方總額
    /// </summary>
    public decimal TotalCreditAmount { get; set; }

    /// <summary>
    /// 狀態
    /// </summary>
    public string Status { get; set; } = "DRAFT";

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

/// <summary>
/// 會計憑證明細實體
/// </summary>
public class VoucherDetail
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 憑證號碼
    /// </summary>
    public string VoucherNo { get; set; } = string.Empty;

    /// <summary>
    /// 行號
    /// </summary>
    public int LineNum { get; set; }

    /// <summary>
    /// 會計科目編號
    /// </summary>
    public string AccountId { get; set; } = string.Empty;

    /// <summary>
    /// 說明
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 借方金額
    /// </summary>
    public decimal DebitAmount { get; set; }

    /// <summary>
    /// 貸方金額
    /// </summary>
    public decimal CreditAmount { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 會計科目餘額實體
/// </summary>
public class AccountBalance
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 會計科目編號
    /// </summary>
    public string AccountId { get; set; } = string.Empty;

    /// <summary>
    /// 會計期間
    /// </summary>
    public string Period { get; set; } = string.Empty;

    /// <summary>
    /// 期初餘額
    /// </summary>
    public decimal OpeningBalance { get; set; }

    /// <summary>
    /// 借方金額
    /// </summary>
    public decimal DebitAmount { get; set; }

    /// <summary>
    /// 貸方金額
    /// </summary>
    public decimal CreditAmount { get; set; }

    /// <summary>
    /// 期末餘額
    /// </summary>
    public decimal ClosingBalance { get; set; }

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

