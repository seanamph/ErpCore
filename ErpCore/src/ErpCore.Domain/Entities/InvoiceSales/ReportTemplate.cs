namespace ErpCore.Domain.Entities.InvoiceSales;

/// <summary>
/// 報表模板實體 (SYSG710-SYSG7I0 - 報表列印作業)
/// </summary>
public class ReportTemplate
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
    /// 模板類型 (PDF, EXCEL, WORD)
    /// </summary>
    public string TemplateType { get; set; } = string.Empty;

    /// <summary>
    /// 報表類型 (SALES_ORDER, SALES_SUMMARY等)
    /// </summary>
    public string ReportType { get; set; } = string.Empty;

    /// <summary>
    /// 模板內容（HTML、XML等）
    /// </summary>
    public string? TemplateContent { get; set; }

    /// <summary>
    /// 模板檔案路徑
    /// </summary>
    public string? TemplateFile { get; set; }

    /// <summary>
    /// 報表參數JSON
    /// </summary>
    public string? Parameters { get; set; }

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";

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

/// <summary>
/// 報表列印記錄實體
/// </summary>
public class ReportPrintLog
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
    /// 列印格式 (PDF, EXCEL, WORD)
    /// </summary>
    public string PrintFormat { get; set; } = string.Empty;

    /// <summary>
    /// 檔案下載連結
    /// </summary>
    public string? FileUrl { get; set; }

    /// <summary>
    /// 報表參數JSON
    /// </summary>
    public string? Parameters { get; set; }

    /// <summary>
    /// 狀態 (S:成功, F:失敗)
    /// </summary>
    public string Status { get; set; } = "S";

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

