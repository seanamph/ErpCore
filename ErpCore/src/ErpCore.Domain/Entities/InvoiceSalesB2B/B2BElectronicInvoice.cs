namespace ErpCore.Domain.Entities.InvoiceSalesB2B;

/// <summary>
/// B2B電子發票實體 (SYSG000_B2B - B2B電子發票列印)
/// </summary>
public class B2BElectronicInvoice
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 發票編號
    /// </summary>
    public string InvoiceId { get; set; } = string.Empty;

    /// <summary>
    /// POS代碼
    /// </summary>
    public string? PosId { get; set; }

    /// <summary>
    /// 發票年月 (YYYYMM)
    /// </summary>
    public string InvYm { get; set; } = string.Empty;

    /// <summary>
    /// 字軌
    /// </summary>
    public string? Track { get; set; }

    /// <summary>
    /// 發票號碼起
    /// </summary>
    public string? InvNoB { get; set; }

    /// <summary>
    /// 發票號碼迄
    /// </summary>
    public string? InvNoE { get; set; }

    /// <summary>
    /// 列印條碼
    /// </summary>
    public string? PrintCode { get; set; }

    /// <summary>
    /// 發票日期
    /// </summary>
    public DateTime? InvoiceDate { get; set; }

    /// <summary>
    /// 獎項類型
    /// </summary>
    public string? PrizeType { get; set; }

    /// <summary>
    /// 獎項金額
    /// </summary>
    public decimal? PrizeAmt { get; set; }

    /// <summary>
    /// 載具識別碼（明碼）
    /// </summary>
    public string? CarrierIdClear { get; set; }

    /// <summary>
    /// 中獎列印標記
    /// </summary>
    public string? AwardPrint { get; set; }

    /// <summary>
    /// 中獎POS
    /// </summary>
    public string? AwardPos { get; set; }

    /// <summary>
    /// 中獎日期
    /// </summary>
    public DateTime? AwardDate { get; set; }

    /// <summary>
    /// B2B標記
    /// </summary>
    public string B2BFlag { get; set; } = "Y";

    /// <summary>
    /// 傳輸類型 (BREG, CTSG, EINB, EING等)
    /// </summary>
    public string? TransferType { get; set; }

    /// <summary>
    /// 傳輸狀態
    /// </summary>
    public string? TransferStatus { get; set; }

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";

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

