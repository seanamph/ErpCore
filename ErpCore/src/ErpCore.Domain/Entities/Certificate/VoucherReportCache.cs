namespace ErpCore.Domain.Entities.Certificate;

/// <summary>
/// 憑證報表快取 (SYSK310-SYSK500)
/// </summary>
public class VoucherReportCache
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 報表類型
    /// </summary>
    public string ReportType { get; set; } = string.Empty;

    /// <summary>
    /// 報表參數（JSON格式）
    /// </summary>
    public string? ReportParams { get; set; }

    /// <summary>
    /// 報表資料（JSON格式）
    /// </summary>
    public string? ReportData { get; set; }

    /// <summary>
    /// 快取過期時間
    /// </summary>
    public DateTime CacheExpireTime { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

