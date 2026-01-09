namespace ErpCore.Domain.Entities.CustomerCustomJgjn;

/// <summary>
/// JGJN發票資料實體 (PnInvoice - 發票列印)
/// </summary>
public class JgjNInvoice
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 發票代碼
    /// </summary>
    public string InvoiceId { get; set; } = string.Empty;

    /// <summary>
    /// 發票號碼
    /// </summary>
    public string? InvoiceNo { get; set; }

    /// <summary>
    /// 發票日期
    /// </summary>
    public DateTime? InvoiceDate { get; set; }

    /// <summary>
    /// 客戶代碼
    /// </summary>
    public string? CustomerId { get; set; }

    /// <summary>
    /// 金額
    /// </summary>
    public decimal? Amount { get; set; }

    /// <summary>
    /// 幣別
    /// </summary>
    public string Currency { get; set; } = "TWD";

    /// <summary>
    /// 狀態 (PENDING:待處理, PROCESSED:已處理, FAILED:失敗)
    /// </summary>
    public string Status { get; set; } = "PENDING";

    /// <summary>
    /// 列印狀態
    /// </summary>
    public string? PrintStatus { get; set; }

    /// <summary>
    /// 列印日期
    /// </summary>
    public DateTime? PrintDate { get; set; }

    /// <summary>
    /// 檔案路徑
    /// </summary>
    public string? FilePath { get; set; }

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

