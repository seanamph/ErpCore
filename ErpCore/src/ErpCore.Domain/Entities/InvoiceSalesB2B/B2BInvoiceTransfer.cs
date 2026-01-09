namespace ErpCore.Domain.Entities.InvoiceSalesB2B;

/// <summary>
/// B2B發票傳輸記錄實體 (SYSG000_B2B - B2B發票資料維護)
/// </summary>
public class B2BInvoiceTransfer
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
    /// 傳輸類型 (BREG, CTSG, EINB, EING等)
    /// </summary>
    public string TransferType { get; set; } = string.Empty;

    /// <summary>
    /// 傳輸狀態 (PENDING, SUCCESS, FAILED)
    /// </summary>
    public string TransferStatus { get; set; } = string.Empty;

    /// <summary>
    /// 傳輸日期
    /// </summary>
    public DateTime? TransferDate { get; set; }

    /// <summary>
    /// 傳輸訊息
    /// </summary>
    public string? TransferMessage { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

