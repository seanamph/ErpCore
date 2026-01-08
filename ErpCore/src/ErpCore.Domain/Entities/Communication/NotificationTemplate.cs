namespace ErpCore.Domain.Entities.Communication;

/// <summary>
/// 通知模板實體
/// </summary>
public class NotificationTemplate
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 模板代碼
    /// </summary>
    public string TemplateCode { get; set; } = string.Empty;

    /// <summary>
    /// 模板名稱
    /// </summary>
    public string TemplateName { get; set; } = string.Empty;

    /// <summary>
    /// 模板類型 (Email/SMS)
    /// </summary>
    public string TemplateType { get; set; } = string.Empty;

    /// <summary>
    /// 主旨（Email用）
    /// </summary>
    public string? Subject { get; set; }

    /// <summary>
    /// 內容
    /// </summary>
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// 內容類型
    /// </summary>
    public string? BodyType { get; set; } = "Text";

    /// <summary>
    /// 參數說明（JSON格式）
    /// </summary>
    public string? Parameters { get; set; }

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

