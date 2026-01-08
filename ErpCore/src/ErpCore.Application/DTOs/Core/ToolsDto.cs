namespace ErpCore.Application.DTOs.Core;

/// <summary>
/// Excel匯出設定 DTO
/// </summary>
public class ExcelExportConfigDto
{
    public long ConfigId { get; set; }
    public string ModuleCode { get; set; } = string.Empty;
    public string ExportName { get; set; } = string.Empty;
    public string? ExportFields { get; set; }
    public string? ExportSettings { get; set; }
    public string? TemplatePath { get; set; }
    public string Status { get; set; } = "1";
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Excel匯出請求 DTO
/// </summary>
public class ExcelExportRequestDto
{
    public long? ConfigId { get; set; }
    public string ModuleCode { get; set; } = string.Empty;
    public Dictionary<string, object>? Filters { get; set; }
    public List<ExportColumnDto>? ExportFields { get; set; }
    public bool Async { get; set; } = false;
    public string? Title { get; set; }
    public string? SheetName { get; set; }
}

/// <summary>
/// Excel匯出欄位 DTO
/// </summary>
public class ExportColumnDto
{
    public string PropertyName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string DataType { get; set; } = "String";
}

/// <summary>
/// Excel匯出任務 DTO
/// </summary>
public class ExcelExportTaskDto
{
    public string TaskId { get; set; } = string.Empty;
    public string Status { get; set; } = "PENDING";
    public string? FileName { get; set; }
    public string? DownloadUrl { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

/// <summary>
/// 新增Excel匯出設定 DTO
/// </summary>
public class CreateExcelExportConfigDto
{
    public string ModuleCode { get; set; } = string.Empty;
    public string ExportName { get; set; } = string.Empty;
    public string? ExportFields { get; set; }
    public string? ExportSettings { get; set; }
    public string? TemplatePath { get; set; }
    public string Status { get; set; } = "1";
}

/// <summary>
/// 字串編碼請求 DTO
/// </summary>
public class EncodeStringRequestDto
{
    public string Text { get; set; } = string.Empty;
    public string EncodeType { get; set; } = "1"; // 1: User ID, 2: Password
}

/// <summary>
/// 字串編碼結果 DTO
/// </summary>
public class EncodeStringResultDto
{
    public string EncodedText { get; set; } = string.Empty;
    public string OriginalText { get; set; } = string.Empty;
}

/// <summary>
/// 字串解碼請求 DTO
/// </summary>
public class DecodeStringRequestDto
{
    public string EncodedText { get; set; } = string.Empty;
    public string EncodeType { get; set; } = "1";
}

/// <summary>
/// 字串解碼結果 DTO
/// </summary>
public class DecodeStringResultDto
{
    public string DecodedText { get; set; } = string.Empty;
    public string EncodedText { get; set; } = string.Empty;
}

/// <summary>
/// 頁面轉換 DTO (ASPXTOASP)
/// </summary>
public class PageTransitionDto
{
    public string TargetUrl { get; set; } = string.Empty;
    public Dictionary<string, string>? QueryParams { get; set; }
    public Dictionary<string, object>? FormData { get; set; }
    public Dictionary<string, object>? SessionData { get; set; }
    public Dictionary<string, string>? CookieData { get; set; }
}

/// <summary>
/// 頁面轉換記錄 DTO
/// </summary>
public class PageTransitionLogDto
{
    public long TransitionId { get; set; }
    public string SourceUrl { get; set; } = string.Empty;
    public string TargetUrl { get; set; } = string.Empty;
    public string SourceType { get; set; } = string.Empty;
    public string TargetType { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? SessionId { get; set; }
    public string Status { get; set; } = "SUCCESS";
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 頁面轉換查詢 DTO
/// </summary>
public class PageTransitionQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? UserId { get; set; }
    public string? SourceUrl { get; set; }
    public string? TargetUrl { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

/// <summary>
/// 頁面轉換對應設定 DTO
/// </summary>
public class PageTransitionMappingDto
{
    public long MappingId { get; set; }
    public string SourcePage { get; set; } = string.Empty;
    public string TargetPage { get; set; } = string.Empty;
    public string SourceType { get; set; } = string.Empty;
    public string TargetType { get; set; } = string.Empty;
    public string? ParameterMapping { get; set; }
    public string? SessionMapping { get; set; }
    public string Status { get; set; } = "1";
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 新增頁面轉換對應設定 DTO
/// </summary>
public class CreatePageTransitionMappingDto
{
    public string SourcePage { get; set; } = string.Empty;
    public string TargetPage { get; set; } = string.Empty;
    public string SourceType { get; set; } = string.Empty;
    public string TargetType { get; set; } = string.Empty;
    public string? ParameterMapping { get; set; }
    public string? SessionMapping { get; set; }
    public string Status { get; set; } = "1";
}

