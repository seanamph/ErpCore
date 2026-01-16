namespace ErpCore.Domain.Entities.Procurement;

/// <summary>
/// 採購報表模板 (採購報表列印)
/// </summary>
public class PurchaseReportTemplate
{
    /// <summary>
    /// 模板ID
    /// </summary>
    public long TemplateId { get; set; }

    /// <summary>
    /// 報表類型
    /// </summary>
    public string ReportType { get; set; } = string.Empty;

    /// <summary>
    /// 報表代碼
    /// </summary>
    public string ReportCode { get; set; } = string.Empty;

    /// <summary>
    /// 模板名稱
    /// </summary>
    public string TemplateName { get; set; } = string.Empty;

    /// <summary>
    /// 模板路徑 (RDLC, Crystal Reports等)
    /// </summary>
    public string? TemplatePath { get; set; }

    /// <summary>
    /// 模板類型 (RDLC, Crystal, HTML)
    /// </summary>
    public string TemplateType { get; set; } = string.Empty;

    /// <summary>
    /// 模板內容
    /// </summary>
    public string? TemplateContent { get; set; }

    /// <summary>
    /// 是否為預設模板
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// 狀態 (1:啟用, 0:停用)
    /// </summary>
    public string Status { get; set; } = "1";

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
