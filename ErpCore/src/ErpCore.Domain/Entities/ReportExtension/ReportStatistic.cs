namespace ErpCore.Domain.Entities.ReportExtension;

/// <summary>
/// 報表統計記錄實體
/// </summary>
public class ReportStatistic
{
    /// <summary>
    /// 統計ID
    /// </summary>
    public long StatisticId { get; set; }

    /// <summary>
    /// 報表代碼
    /// </summary>
    public string ReportCode { get; set; } = string.Empty;

    /// <summary>
    /// 報表名稱
    /// </summary>
    public string ReportName { get; set; } = string.Empty;

    /// <summary>
    /// 統計類型
    /// </summary>
    public string StatisticType { get; set; } = string.Empty;

    /// <summary>
    /// 統計日期
    /// </summary>
    public DateTime StatisticDate { get; set; }

    /// <summary>
    /// 統計值
    /// </summary>
    public decimal? StatisticValue { get; set; }

    /// <summary>
    /// 統計資料 (JSON格式)
    /// </summary>
    public string? StatisticData { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

