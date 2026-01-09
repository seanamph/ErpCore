namespace ErpCore.Domain.Entities.InvoiceExtension;

/// <summary>
/// 電子發票擴展實體 (ECA1000)
/// </summary>
public class EInvoiceExtension
{
    /// <summary>
    /// 擴展ID
    /// </summary>
    public long ExtensionId { get; set; }

    /// <summary>
    /// 發票ID
    /// </summary>
    public long InvoiceId { get; set; }

    /// <summary>
    /// 擴展類型
    /// </summary>
    public string ExtensionType { get; set; } = string.Empty;

    /// <summary>
    /// 擴展資料 (JSON格式)
    /// </summary>
    public string? ExtensionData { get; set; }

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

