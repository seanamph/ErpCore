namespace ErpCore.Application.DTOs.InvoiceSales;

/// <summary>
/// 報表列印 DTO (SYSG710-SYSG7I0 - 報表列印作業)
/// </summary>
public class ReportPrintDto
{
    /// <summary>
    /// 報表類型
    /// </summary>
    public string ReportType { get; set; } = string.Empty;

    /// <summary>
    /// 模板編號
    /// </summary>
    public string? TemplateId { get; set; }

    /// <summary>
    /// 列印格式 (PDF, EXCEL, WORD)
    /// </summary>
    public string PrintFormat { get; set; } = "PDF";

    /// <summary>
    /// 報表參數
    /// </summary>
    public Dictionary<string, object>? Parameters { get; set; }
}

/// <summary>
/// 報表列印結果 DTO
/// </summary>
public class ReportPrintResultDto
{
    /// <summary>
    /// 報表編號
    /// </summary>
    public string ReportId { get; set; } = string.Empty;

    /// <summary>
    /// 檔案下載連結
    /// </summary>
    public string FileUrl { get; set; } = string.Empty;

    /// <summary>
    /// 檔案名稱
    /// </summary>
    public string FileName { get; set; } = string.Empty;
}

/// <summary>
/// 報表模板 DTO
/// </summary>
public class ReportTemplateDto
{
    /// <summary>
    /// 模板編號
    /// </summary>
    public string TemplateId { get; set; } = string.Empty;

    /// <summary>
    /// 模板名稱
    /// </summary>
    public string TemplateName { get; set; } = string.Empty;

    /// <summary>
    /// 模板類型
    /// </summary>
    public string TemplateType { get; set; } = string.Empty;

    /// <summary>
    /// 報表類型
    /// </summary>
    public string ReportType { get; set; } = string.Empty;

    /// <summary>
    /// 狀態
    /// </summary>
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// 報表列印記錄 DTO
/// </summary>
public class ReportPrintLogDto
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 報表編號
    /// </summary>
    public string ReportId { get; set; } = string.Empty;

    /// <summary>
    /// 報表類型
    /// </summary>
    public string ReportType { get; set; } = string.Empty;

    /// <summary>
    /// 模板編號
    /// </summary>
    public string? TemplateId { get; set; }

    /// <summary>
    /// 列印人員
    /// </summary>
    public string PrintUserId { get; set; } = string.Empty;

    /// <summary>
    /// 列印日期
    /// </summary>
    public DateTime PrintDate { get; set; }

    /// <summary>
    /// 列印格式
    /// </summary>
    public string PrintFormat { get; set; } = string.Empty;

    /// <summary>
    /// 檔案下載連結
    /// </summary>
    public string? FileUrl { get; set; }

    /// <summary>
    /// 狀態
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string? ErrorMessage { get; set; }
}

