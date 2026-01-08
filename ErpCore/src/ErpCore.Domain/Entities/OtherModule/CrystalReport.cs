namespace ErpCore.Domain.Entities.OtherModule;

/// <summary>
/// Crystal Reports設定實體
/// </summary>
public class CrystalReport
{
    public long ReportId { get; set; }
    public string ReportCode { get; set; } = string.Empty;
    public string ReportName { get; set; } = string.Empty;
    public string ReportPath { get; set; } = string.Empty;
    public string? MdbName { get; set; }
    public string? Parameters { get; set; }
    public string? ExportOptions { get; set; }
    public string Status { get; set; } = "1";
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Crystal Reports操作記錄實體
/// </summary>
public class CrystalReportLog
{
    public long LogId { get; set; }
    public long ReportId { get; set; }
    public string ReportCode { get; set; } = string.Empty;
    public string OperationType { get; set; } = string.Empty;
    public string? Parameters { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
    public long? FileSize { get; set; }
    public int? Duration { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

