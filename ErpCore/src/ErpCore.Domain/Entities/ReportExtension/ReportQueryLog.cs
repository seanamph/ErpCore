namespace ErpCore.Domain.Entities.ReportExtension;

/// <summary>
/// 報表查詢記錄實體
/// </summary>
public class ReportQueryLog
{
    /// <summary>
    /// 記錄ID
    /// </summary>
    public Guid LogId { get; set; }

    /// <summary>
    /// 查詢ID
    /// </summary>
    public Guid? QueryId { get; set; }

    /// <summary>
    /// 報表代碼
    /// </summary>
    public string ReportCode { get; set; } = string.Empty;

    /// <summary>
    /// 使用者ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 查詢參數 (JSON格式)
    /// </summary>
    public string? QueryParams { get; set; }

    /// <summary>
    /// 查詢時間
    /// </summary>
    public DateTime QueryTime { get; set; }

    /// <summary>
    /// 執行時間(毫秒)
    /// </summary>
    public int? ExecutionTime { get; set; }

    /// <summary>
    /// 記錄數
    /// </summary>
    public int? RecordCount { get; set; }

    /// <summary>
    /// 狀態
    /// </summary>
    public string Status { get; set; } = "1";
}

