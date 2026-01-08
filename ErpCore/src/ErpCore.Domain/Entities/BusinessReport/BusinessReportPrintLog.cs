namespace ErpCore.Domain.Entities.BusinessReport;

/// <summary>
/// 業務報表列印記錄資料實體 (SYSL161)
/// </summary>
public class BusinessReportPrintLog
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 報表代碼
    /// </summary>
    public string ReportId { get; set; } = string.Empty;

    /// <summary>
    /// 報表名稱
    /// </summary>
    public string? ReportName { get; set; }

    /// <summary>
    /// 報表類型
    /// </summary>
    public string? ReportType { get; set; }

    /// <summary>
    /// 列印日期
    /// </summary>
    public DateTime PrintDate { get; set; }

    /// <summary>
    /// 列印使用者
    /// </summary>
    public string? PrintUserId { get; set; }

    /// <summary>
    /// 列印使用者名稱
    /// </summary>
    public string? PrintUserName { get; set; }

    /// <summary>
    /// 列印參數 (JSON格式)
    /// </summary>
    public string? PrintParams { get; set; }

    /// <summary>
    /// 列印格式 (PDF, Excel, Print)
    /// </summary>
    public string? PrintFormat { get; set; }

    /// <summary>
    /// 狀態 (1:成功, 0:失敗)
    /// </summary>
    public string Status { get; set; } = "1";

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 檔案路徑（相對路徑）
    /// </summary>
    public string? FilePath { get; set; }

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

