namespace ErpCore.Domain.Entities.InvoiceSales;

/// <summary>
/// 電子發票實體 (SYSG210-SYSG2B0 - 電子發票列印)
/// </summary>
public class ElectronicInvoice
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// POS代碼
    /// </summary>
    public string? PosId { get; set; }

    /// <summary>
    /// 發票年月 (YYYYMM格式)
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

/// <summary>
/// 電子發票列印設定實體
/// </summary>
public class ElectronicInvoicePrintSetting
{
    /// <summary>
    /// 設定ID
    /// </summary>
    public string SettingId { get; set; } = string.Empty;

    /// <summary>
    /// 列印格式 (A4, A5, THERMAL)
    /// </summary>
    public string PrintFormat { get; set; } = string.Empty;

    /// <summary>
    /// 條碼類型 (code128, ean13等)
    /// </summary>
    public string? BarcodeType { get; set; }

    /// <summary>
    /// 條碼高度
    /// </summary>
    public int? BarcodeSize { get; set; } = 40;

    /// <summary>
    /// 條碼間距
    /// </summary>
    public int? BarcodeMargin { get; set; } = 5;

    /// <summary>
    /// 每頁欄數
    /// </summary>
    public int? ColCount { get; set; } = 2;

    /// <summary>
    /// 每頁筆數
    /// </summary>
    public int? PageCount { get; set; } = 14;

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

