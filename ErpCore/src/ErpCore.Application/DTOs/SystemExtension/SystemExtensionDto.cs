namespace ErpCore.Application.DTOs.SystemExtension;

/// <summary>
/// 系統擴展 DTO (SYSX110)
/// </summary>
public class SystemExtensionDto
{
    public long TKey { get; set; }
    public string ExtensionId { get; set; } = string.Empty;
    public string ExtensionName { get; set; } = string.Empty;
    public string? ExtensionType { get; set; }
    public string? ExtensionValue { get; set; }
    public string? ExtensionConfig { get; set; }
    public int? SeqNo { get; set; }
    public string Status { get; set; } = "1";
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int? CreatedPriority { get; set; }
    public string? CreatedGroup { get; set; }
}

/// <summary>
/// 系統擴展查詢 DTO
/// </summary>
public class SystemExtensionQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? ExtensionId { get; set; }
    public string? ExtensionName { get; set; }
    public string? ExtensionType { get; set; }
    public string? Status { get; set; }
    public DateTime? CreatedDateFrom { get; set; }
    public DateTime? CreatedDateTo { get; set; }
}

/// <summary>
/// 新增系統擴展 DTO
/// </summary>
public class CreateSystemExtensionDto
{
    public string ExtensionId { get; set; } = string.Empty;
    public string ExtensionName { get; set; } = string.Empty;
    public string? ExtensionType { get; set; }
    public string? ExtensionValue { get; set; }
    public string? ExtensionConfig { get; set; }
    public int? SeqNo { get; set; }
    public string Status { get; set; } = "1";
    public string? Notes { get; set; }
}

/// <summary>
/// 修改系統擴展 DTO
/// </summary>
public class UpdateSystemExtensionDto
{
    public string ExtensionName { get; set; } = string.Empty;
    public string? ExtensionType { get; set; }
    public string? ExtensionValue { get; set; }
    public string? ExtensionConfig { get; set; }
    public int? SeqNo { get; set; }
    public string Status { get; set; } = "1";
    public string? Notes { get; set; }
}

/// <summary>
/// 系統擴展統計 DTO (SYSX120)
/// </summary>
public class SystemExtensionStatisticsDto
{
    public int TotalCount { get; set; }
    public int ActiveCount { get; set; }
    public int InactiveCount { get; set; }
    public List<SystemExtensionTypeCountDto> ByType { get; set; } = new();
}

/// <summary>
/// 系統擴展類型統計 DTO
/// </summary>
public class SystemExtensionTypeCountDto
{
    public string ExtensionType { get; set; } = string.Empty;
    public int Count { get; set; }
}

/// <summary>
/// 系統擴展統計查詢 DTO
/// </summary>
public class SystemExtensionStatisticsQueryDto
{
    public string? ExtensionType { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 系統擴展報表記錄 DTO (SYSX140)
/// </summary>
public class SystemExtensionReportDto
{
    public long ReportId { get; set; }
    public string ReportName { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public string? ReportTemplate { get; set; }
    public string? QueryConditions { get; set; }
    public DateTime GeneratedDate { get; set; }
    public string? GeneratedBy { get; set; }
    public string? FileUrl { get; set; }
    public long? FileSize { get; set; }
    public string Status { get; set; } = "COMPLETED";
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 系統擴展報表查詢 DTO
/// </summary>
public class SystemExtensionReportQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ReportName { get; set; }
    public string? ReportType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

/// <summary>
/// 產生報表請求 DTO
/// </summary>
public class GenerateSystemExtensionReportDto
{
    public string ReportName { get; set; } = string.Empty;
    public SystemExtensionQueryDto? Filters { get; set; }
    public string Template { get; set; } = "default";
}

/// <summary>
/// 系統擴展報表查詢請求 DTO
/// </summary>
public class SystemExtensionReportQueryRequestDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public SystemExtensionQueryDto? Filters { get; set; }
    public List<string>? GroupBy { get; set; }
    public List<string>? OrderBy { get; set; }
}

/// <summary>
/// 系統擴展報表查詢結果 DTO
/// </summary>
public class SystemExtensionReportQueryResultDto
{
    public List<SystemExtensionDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public SystemExtensionStatisticsDto? Statistics { get; set; }
}

