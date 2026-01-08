namespace ErpCore.Domain.Entities.Sales;

/// <summary>
/// 銷售報表快取 (SYSD310-SYSD430)
/// </summary>
public class SalesReportCache
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

