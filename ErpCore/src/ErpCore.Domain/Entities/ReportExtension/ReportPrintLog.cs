namespace ErpCore.Domain.Entities.ReportExtension;

/// <summary>
/// 報表列印記錄實體
/// </summary>
public class ReportPrintLog
{
    /// <summary>
    /// 列印記錄ID
    /// </summary>
    public long PrintLogId { get; set; }

    /// <summary>
    /// 報表代碼
    /// </summary>
    public string ReportCode { get; set; } = string.Empty;

    /// <summary>
    /// 報表名稱
    /// </summary>
    public string ReportName { get; set; } = string.Empty;

    /// <summary>
    /// 列印類型
    /// </summary>
    public string PrintType { get; set; } = string.Empty;

    /// <summary>
    /// 列印格式
    /// </summary>
    public string? PrintFormat { get; set; }

    /// <summary>
    /// 檔案路徑
    /// </summary>
    public string? FilePath { get; set; }

    /// <summary>
    /// 檔案名稱
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// 檔案大小
    /// </summary>
    public long? FileSize { get; set; }

    /// <summary>
    /// 列印狀態
    /// </summary>
    public string PrintStatus { get; set; } = "Pending";

    /// <summary>
    /// 列印次數
    /// </summary>
    public int PrintCount { get; set; } = 1;

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 列印時間
    /// </summary>
    public DateTime? PrintedAt { get; set; }
}

