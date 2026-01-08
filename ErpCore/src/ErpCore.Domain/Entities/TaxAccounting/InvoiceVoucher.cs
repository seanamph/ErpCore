namespace ErpCore.Domain.Entities.TaxAccounting;

/// <summary>
/// 發票傳票實體 (SYST211-SYST212)
/// </summary>
public class InvoiceVoucher
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 傳票主檔T_KEY (外鍵至Vouchers)
    /// </summary>
    public long VoucherTKey { get; set; }

    /// <summary>
    /// 發票類型 (1:進項稅額, 2:進項折讓, 3:進項退回, 4:銷項稅額, 5:銷項折讓, 6:銷項退回)
    /// </summary>
    public string InvoiceType { get; set; } = string.Empty;

    /// <summary>
    /// 發票號碼
    /// </summary>
    public string? InvoiceNo { get; set; }

    /// <summary>
    /// 發票日期
    /// </summary>
    public DateTime? InvoiceDate { get; set; }

    /// <summary>
    /// 發票格式
    /// </summary>
    public string? InvoiceFormat { get; set; }

    /// <summary>
    /// 發票金額
    /// </summary>
    public decimal? InvoiceAmount { get; set; }

    /// <summary>
    /// 稅額
    /// </summary>
    public decimal? TaxAmount { get; set; }

    /// <summary>
    /// 扣抵代號
    /// </summary>
    public string? DeductCode { get; set; }

    /// <summary>
    /// 類別區分
    /// </summary>
    public string? CategoryType { get; set; }

    /// <summary>
    /// 憑證單號
    /// </summary>
    public string? VoucherNo { get; set; }

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

