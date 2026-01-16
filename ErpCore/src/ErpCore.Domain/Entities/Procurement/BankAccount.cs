namespace ErpCore.Domain.Entities.Procurement;

/// <summary>
/// 銀行帳戶主檔 (銀行帳戶維護)
/// </summary>
public class BankAccount
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 銀行帳戶編號
    /// </summary>
    public string BankAccountId { get; set; } = string.Empty;

    /// <summary>
    /// 銀行代號
    /// </summary>
    public string BankId { get; set; } = string.Empty;

    /// <summary>
    /// 帳戶名稱
    /// </summary>
    public string AccountName { get; set; } = string.Empty;

    /// <summary>
    /// 帳戶號碼
    /// </summary>
    public string AccountNumber { get; set; } = string.Empty;

    /// <summary>
    /// 帳戶類型 (1:活期, 2:定期, 3:外幣)
    /// </summary>
    public string? AccountType { get; set; }

    /// <summary>
    /// 幣別
    /// </summary>
    public string? CurrencyId { get; set; }

    /// <summary>
    /// 狀態 (1:啟用, 0:停用)
    /// </summary>
    public string Status { get; set; } = "1";

    /// <summary>
    /// 帳戶餘額
    /// </summary>
    public decimal? Balance { get; set; }

    /// <summary>
    /// 開戶日期
    /// </summary>
    public DateTime? OpeningDate { get; set; }

    /// <summary>
    /// 結清日期
    /// </summary>
    public DateTime? ClosingDate { get; set; }

    /// <summary>
    /// 聯絡人
    /// </summary>
    public string? ContactPerson { get; set; }

    /// <summary>
    /// 聯絡電話
    /// </summary>
    public string? ContactPhone { get; set; }

    /// <summary>
    /// 聯絡信箱
    /// </summary>
    public string? ContactEmail { get; set; }

    /// <summary>
    /// 分行名稱
    /// </summary>
    public string? BranchName { get; set; }

    /// <summary>
    /// 分行代號
    /// </summary>
    public string? BranchCode { get; set; }

    /// <summary>
    /// SWIFT代號
    /// </summary>
    public string? SwiftCode { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Notes { get; set; }

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

    /// <summary>
    /// 建立者等級
    /// </summary>
    public int? CreatedPriority { get; set; }

    /// <summary>
    /// 建立者群組
    /// </summary>
    public string? CreatedGroup { get; set; }
}
