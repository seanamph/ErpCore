namespace ErpCore.Domain.Entities.CustomerInvoice;

/// <summary>
/// 發票主檔實體 (SYS2000 - 發票列印作業)
/// </summary>
public class Invoice
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 發票號碼
    /// </summary>
    public string InvoiceNo { get; set; } = string.Empty;

    /// <summary>
    /// 發票類型 (NORMAL:一般, EINVOICE:電子發票, CHANGE:變更)
    /// </summary>
    public string InvoiceType { get; set; } = string.Empty;

    /// <summary>
    /// 發票日期
    /// </summary>
    public DateTime InvoiceDate { get; set; }

    /// <summary>
    /// 客戶編號
    /// </summary>
    public string? CustomerId { get; set; }

    /// <summary>
    /// 店別代碼
    /// </summary>
    public string? StoreId { get; set; }

    /// <summary>
    /// 總金額
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 稅額
    /// </summary>
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// 金額
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 幣別
    /// </summary>
    public string CurrencyId { get; set; } = "TWD";

    /// <summary>
    /// 狀態 (DRAFT:草稿, ISSUED:已開立, CANCELLED:已作廢)
    /// </summary>
    public string Status { get; set; } = "DRAFT";

    /// <summary>
    /// 列印次數
    /// </summary>
    public int PrintCount { get; set; }

    /// <summary>
    /// 最後列印日期
    /// </summary>
    public DateTime? LastPrintDate { get; set; }

    /// <summary>
    /// 最後列印人員
    /// </summary>
    public string? LastPrintUser { get; set; }

    /// <summary>
    /// 列印格式
    /// </summary>
    public string? PrintFormat { get; set; }

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
/// 發票明細實體
/// </summary>
public class InvoiceDetail
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 發票號碼
    /// </summary>
    public string InvoiceNo { get; set; } = string.Empty;

    /// <summary>
    /// 行號
    /// </summary>
    public int LineNum { get; set; }

    /// <summary>
    /// 商品編號
    /// </summary>
    public string? GoodsId { get; set; }

    /// <summary>
    /// 商品名稱
    /// </summary>
    public string GoodsName { get; set; } = string.Empty;

    /// <summary>
    /// 數量
    /// </summary>
    public decimal Qty { get; set; }

    /// <summary>
    /// 單價
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 金額
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 稅率
    /// </summary>
    public decimal TaxRate { get; set; }

    /// <summary>
    /// 稅額
    /// </summary>
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// 單位
    /// </summary>
    public string? UnitId { get; set; }

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
}

/// <summary>
/// 發票列印記錄實體
/// </summary>
public class InvoicePrintLog
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 發票號碼
    /// </summary>
    public string InvoiceNo { get; set; } = string.Empty;

    /// <summary>
    /// 列印日期
    /// </summary>
    public DateTime PrintDate { get; set; }

    /// <summary>
    /// 列印人員
    /// </summary>
    public string? PrintUser { get; set; }

    /// <summary>
    /// 列印格式
    /// </summary>
    public string? PrintFormat { get; set; }

    /// <summary>
    /// 列印類型 (NORMAL:一般列印, RE_PRINT:重新列印)
    /// </summary>
    public string? PrintType { get; set; }

    /// <summary>
    /// 印表機名稱
    /// </summary>
    public string? PrinterName { get; set; }

    /// <summary>
    /// 列印份數
    /// </summary>
    public int PrintCount { get; set; }

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
}

