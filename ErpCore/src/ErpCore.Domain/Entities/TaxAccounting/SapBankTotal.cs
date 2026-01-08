namespace ErpCore.Domain.Entities.TaxAccounting;

/// <summary>
/// SAP銀行往來總表實體 (SYST510)
/// </summary>
public class SapBankTotal
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// SAP日期 (YYYYMMDD)
    /// </summary>
    public string SapDate { get; set; } = string.Empty;

    /// <summary>
    /// SAP會計科目
    /// </summary>
    public string? SapStypeId { get; set; }

    /// <summary>
    /// 公司代號
    /// </summary>
    public string CompId { get; set; } = string.Empty;

    /// <summary>
    /// 銀行金額
    /// </summary>
    public decimal BankAmt { get; set; }

    /// <summary>
    /// 銀行餘額
    /// </summary>
    public decimal BankBalance { get; set; }

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

