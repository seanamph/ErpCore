namespace ErpCore.Application.DTOs.OtherModule;

/// <summary>
/// Crystal Reports設定 DTO
/// </summary>
public class CrystalReportDto
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
/// 生成報表請求 DTO
/// </summary>
public class GenerateReportRequestDto
{
    public string ReportCode { get; set; } = string.Empty;
    public Dictionary<string, object>? Parameters { get; set; }
    public string ExportFormat { get; set; } = "PDF";
}

/// <summary>
/// 生成報表回應 DTO
/// </summary>
public class GenerateReportResponseDto
{
    public long ReportId { get; set; }
    public string DownloadUrl { get; set; } = string.Empty;
}

/// <summary>
/// Crystal Reports操作記錄 DTO
/// </summary>
public class CrystalReportLogDto
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

/// <summary>
/// 新增Crystal Reports設定 DTO
/// </summary>
public class CreateCrystalReportDto
{
    public string ReportCode { get; set; } = string.Empty;
    public string ReportName { get; set; } = string.Empty;
    public string ReportPath { get; set; } = string.Empty;
    public string? MdbName { get; set; }
    public string? Parameters { get; set; }
    public string? ExportOptions { get; set; }
    public string Status { get; set; } = "1";
}

