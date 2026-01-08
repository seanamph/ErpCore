namespace ErpCore.Application.DTOs.ReportExtension;

/// <summary>
/// 報表查詢設定 DTO
/// </summary>
public class ReportQueryDto
{
    public Guid QueryId { get; set; }
    public string ReportCode { get; set; } = string.Empty;
    public string ReportName { get; set; } = string.Empty;
    public string? QueryName { get; set; }
    public string? QueryParams { get; set; }
    public string? QuerySql { get; set; }
    public string Status { get; set; } = "1";
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 新增報表查詢 DTO
/// </summary>
public class CreateReportQueryDto
{
    public string ReportCode { get; set; } = string.Empty;
    public string ReportName { get; set; } = string.Empty;
    public string? QueryName { get; set; }
    public string? QueryParams { get; set; }
    public string? QuerySql { get; set; }
    public string Status { get; set; } = "1";
}

/// <summary>
/// 修改報表查詢 DTO
/// </summary>
public class UpdateReportQueryDto
{
    public string ReportName { get; set; } = string.Empty;
    public string? QueryName { get; set; }
    public string? QueryParams { get; set; }
    public string? QuerySql { get; set; }
    public string Status { get; set; } = "1";
}

/// <summary>
/// 報表查詢請求 DTO
/// </summary>
public class ReportQueryRequestDto
{
    public string ReportCode { get; set; } = string.Empty;
    public Dictionary<string, object>? QueryParams { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

/// <summary>
/// 報表查詢結果 DTO
/// </summary>
public class ReportQueryResultDto
{
    public List<Dictionary<string, object>> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int? ExecutionTime { get; set; }
}

/// <summary>
/// 報表查詢記錄 DTO
/// </summary>
public class ReportQueryLogDto
{
    public Guid LogId { get; set; }
    public Guid? QueryId { get; set; }
    public string ReportCode { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string? QueryParams { get; set; }
    public DateTime QueryTime { get; set; }
    public int? ExecutionTime { get; set; }
    public int? RecordCount { get; set; }
    public string Status { get; set; } = "1";
}

/// <summary>
/// 報表列印記錄 DTO
/// </summary>
public class ReportPrintLogDto
{
    public long PrintLogId { get; set; }
    public string ReportCode { get; set; } = string.Empty;
    public string ReportName { get; set; } = string.Empty;
    public string PrintType { get; set; } = string.Empty;
    public string? PrintFormat { get; set; }
    public string? FilePath { get; set; }
    public string? FileName { get; set; }
    public long? FileSize { get; set; }
    public string PrintStatus { get; set; } = "Pending";
    public int PrintCount { get; set; } = 1;
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PrintedAt { get; set; }
}

/// <summary>
/// 報表列印請求 DTO
/// </summary>
public class ReportPrintRequestDto
{
    public string ReportCode { get; set; } = string.Empty;
    public string ReportName { get; set; } = string.Empty;
    public string PrintType { get; set; } = string.Empty;
    public string PrintFormat { get; set; } = "PDF";
    public Dictionary<string, object>? QueryParams { get; set; }
}

/// <summary>
/// 報表統計記錄 DTO
/// </summary>
public class ReportStatisticDto
{
    public long StatisticId { get; set; }
    public string ReportCode { get; set; } = string.Empty;
    public string ReportName { get; set; } = string.Empty;
    public string StatisticType { get; set; } = string.Empty;
    public DateTime StatisticDate { get; set; }
    public decimal? StatisticValue { get; set; }
    public string? StatisticData { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 報表統計查詢 DTO
/// </summary>
public class ReportStatisticQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ReportCode { get; set; }
    public string? StatisticType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

/// <summary>
/// 報表查詢列表請求 DTO
/// </summary>
public class ReportQueryListDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ReportCode { get; set; }
    public string? Status { get; set; }
}

