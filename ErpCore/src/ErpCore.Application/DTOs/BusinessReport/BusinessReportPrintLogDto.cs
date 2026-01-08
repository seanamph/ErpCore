namespace ErpCore.Application.DTOs.BusinessReport;

/// <summary>
/// 業務報表列印記錄 DTO (SYSL161)
/// </summary>
public class BusinessReportPrintLogDto
{
    public long TKey { get; set; }
    public string ReportId { get; set; } = string.Empty;
    public string? ReportName { get; set; }
    public string? ReportType { get; set; }
    public DateTime PrintDate { get; set; }
    public string? PrintUserId { get; set; }
    public string? PrintUserName { get; set; }
    public string? PrintParams { get; set; }
    public string? PrintFormat { get; set; }
    public string Status { get; set; } = "1";
    public string? ErrorMessage { get; set; }
    public string? FilePath { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 業務報表列印記錄查詢 DTO (SYSL161)
/// </summary>
public class BusinessReportPrintLogQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public BusinessReportPrintLogFiltersDto Filters { get; set; } = new();
}

/// <summary>
/// 業務報表列印記錄篩選 DTO (SYSL161)
/// </summary>
public class BusinessReportPrintLogFiltersDto
{
    public string? ReportId { get; set; }
    public string? ReportName { get; set; }
    public string? PrintUserId { get; set; }
    public DateTime? PrintDateFrom { get; set; }
    public DateTime? PrintDateTo { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 業務報表列印請求 DTO (SYSL161)
/// </summary>
public class BusinessReportPrintRequestDto
{
    public string PrintFormat { get; set; } = "PDF";
    public Dictionary<string, object>? PrintParams { get; set; }
}

/// <summary>
/// 業務報表列印結果 DTO (SYSL161)
/// </summary>
public class BusinessReportPrintResultDto
{
    public long PrintLogId { get; set; }
    public string? FileUrl { get; set; }
    public string? FileName { get; set; }
}

/// <summary>
/// 業務報表匯出請求 DTO (SYSL161)
/// </summary>
public class BusinessReportExportRequestDto
{
    public string ExportFormat { get; set; } = "Excel";
    public Dictionary<string, object>? ExportParams { get; set; }
}

