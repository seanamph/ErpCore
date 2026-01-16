namespace ErpCore.Application.DTOs.Procurement;

/// <summary>
/// 採購報表列印 DTO
/// </summary>
public class PurchaseReportPrintDto
{
    public long TKey { get; set; }
    public string ReportType { get; set; } = string.Empty;
    public string? ReportTypeName { get; set; }
    public string ReportCode { get; set; } = string.Empty;
    public string ReportName { get; set; } = string.Empty;
    public DateTime PrintDate { get; set; }
    public string PrintUserId { get; set; } = string.Empty;
    public string? PrintUserName { get; set; }
    public string? FilterConditions { get; set; }
    public string? PrintSettings { get; set; }
    public string? FileFormat { get; set; }
    public string? FilePath { get; set; }
    public string? FileName { get; set; }
    public long? FileSize { get; set; }
    public string Status { get; set; } = "S";
    public string? StatusName { get; set; }
    public string? ErrorMessage { get; set; }
    public int? PageCount { get; set; }
    public int? RecordCount { get; set; }
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 建立採購報表列印 DTO
/// </summary>
public class CreatePurchaseReportPrintDto
{
    public string ReportType { get; set; } = string.Empty;
    public string ReportCode { get; set; } = string.Empty;
    public string ReportName { get; set; } = string.Empty;
    public Dictionary<string, object>? FilterConditions { get; set; }
    public PrintSettingsDto? PrintSettings { get; set; }
    public string FileFormat { get; set; } = "PDF";
}

/// <summary>
/// 列印設定 DTO
/// </summary>
public class PrintSettingsDto
{
    public string PageSize { get; set; } = "A4";
    public string Orientation { get; set; } = "Portrait";
    public MarginsDto? Margins { get; set; }
}

/// <summary>
/// 邊界設定 DTO
/// </summary>
public class MarginsDto
{
    public int Top { get; set; } = 20;
    public int Bottom { get; set; } = 20;
    public int Left { get; set; } = 20;
    public int Right { get; set; } = 20;
}

/// <summary>
/// 修改採購報表列印 DTO
/// </summary>
public class UpdatePurchaseReportPrintDto
{
    public string? Status { get; set; }
    public string? ErrorMessage { get; set; }
    public string? FilePath { get; set; }
    public string? FileName { get; set; }
    public long? FileSize { get; set; }
    public int? PageCount { get; set; }
    public int? RecordCount { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 查詢採購報表列印 DTO
/// </summary>
public class PurchaseReportPrintQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? ReportType { get; set; }
    public string? ReportCode { get; set; }
    public string? PrintUserId { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

/// <summary>
/// 採購報表列印結果 DTO
/// </summary>
public class PurchaseReportPrintResultDto
{
    public long TKey { get; set; }
    public string? FilePath { get; set; }
    public string? FileName { get; set; }
}

/// <summary>
/// 報表預覽 DTO
/// </summary>
public class ReportPreviewDto
{
    public string PreviewData { get; set; } = string.Empty; // Base64編碼的PDF或圖片
    public string ContentType { get; set; } = "application/pdf";
}

/// <summary>
/// 採購報表模板 DTO
/// </summary>
public class PurchaseReportTemplateDto
{
    public long TemplateId { get; set; }
    public string ReportType { get; set; } = string.Empty;
    public string ReportCode { get; set; } = string.Empty;
    public string TemplateName { get; set; } = string.Empty;
    public string? TemplatePath { get; set; }
    public string TemplateType { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public string Status { get; set; } = "1";
}
