namespace ErpCore.Domain.Entities.TaxAccounting;

/// <summary>
/// 常用傳票明細實體 (SYST123)
/// </summary>
public class CommonVoucherDetail
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 傳票主檔TKey
    /// </summary>
    public long VoucherTKey { get; set; }

    /// <summary>
    /// 序號
    /// </summary>
    public int SeqNo { get; set; }

    /// <summary>
    /// 會計科目代號
    /// </summary>
    public string? StypeId { get; set; }

    /// <summary>
    /// 借方金額
    /// </summary>
    public decimal DebitAmount { get; set; }

    /// <summary>
    /// 貸方金額
    /// </summary>
    public decimal CreditAmount { get; set; }

    /// <summary>
    /// 組織代號
    /// </summary>
    public string? OrgId { get; set; }

    /// <summary>
    /// 摘要
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// 廠客代號
    /// </summary>
    public string? VendorId { get; set; }

    /// <summary>
    /// 自訂欄位1
    /// </summary>
    public string? CustomField1 { get; set; }

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

