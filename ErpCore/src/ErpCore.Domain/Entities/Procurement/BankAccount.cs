namespace ErpCore.Domain.Entities.Procurement;

/// <summary>
/// 銀行帳戶主檔
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
    /// 銀行代碼
    /// </summary>
    public string BankId { get; set; } = string.Empty;

    /// <summary>
    /// 帳戶名稱
    /// </summary>
    public string AccountName { get; set; } = string.Empty;

    /// <summary>
    /// 帳號
    /// </summary>
    public string AccountNumber { get; set; } = string.Empty;

    /// <summary>
    /// 帳戶類型
    /// </summary>
    public string? AccountType { get; set; }

    /// <summary>
    /// 幣別
    /// </summary>
    public string? CurrencyId { get; set; }

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";

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

