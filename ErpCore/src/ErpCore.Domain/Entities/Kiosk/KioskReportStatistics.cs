namespace ErpCore.Domain.Entities.Kiosk;

/// <summary>
/// Kiosk報表統計實體
/// </summary>
public class KioskReportStatistics
{
    /// <summary>
    /// 主鍵ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 報表日期
    /// </summary>
    public DateTime ReportDate { get; set; }

    /// <summary>
    /// Kiosk機號（NULL表示全部）
    /// </summary>
    public string? KioskId { get; set; }

    /// <summary>
    /// 功能代碼（NULL表示全部）
    /// </summary>
    public string? FunctionCode { get; set; }

    /// <summary>
    /// 總交易數
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 成功交易數
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// 失敗交易數
    /// </summary>
    public int FailedCount { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

