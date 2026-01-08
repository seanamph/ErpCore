namespace ErpCore.Domain.Entities.SystemExtension;

/// <summary>
/// 系統擴展報表記錄實體 (SYSX140)
/// </summary>
public class SystemExtensionReport
{
    /// <summary>
    /// 報表ID
    /// </summary>
    public long ReportId { get; set; }

    /// <summary>
    /// 報表名稱
    /// </summary>
    public string ReportName { get; set; } = string.Empty;

    /// <summary>
    /// 報表類型 (PDF, Excel, Word等)
    /// </summary>
    public string ReportType { get; set; } = string.Empty;

    /// <summary>
    /// 報表範本 (JSON格式)
    /// </summary>
    public string? ReportTemplate { get; set; }

    /// <summary>
    /// 查詢條件 (JSON格式)
    /// </summary>
    public string? QueryConditions { get; set; }

    /// <summary>
    /// 產生時間
    /// </summary>
    public DateTime GeneratedDate { get; set; }

    /// <summary>
    /// 產生者
    /// </summary>
    public string? GeneratedBy { get; set; }

    /// <summary>
    /// 檔案URL
    /// </summary>
    public string? FileUrl { get; set; }

    /// <summary>
    /// 檔案大小
    /// </summary>
    public long? FileSize { get; set; }

    /// <summary>
    /// 狀態
    /// </summary>
    public string Status { get; set; } = "COMPLETED";

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

