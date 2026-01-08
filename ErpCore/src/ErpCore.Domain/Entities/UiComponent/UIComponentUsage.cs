namespace ErpCore.Domain.Entities.UiComponent;

/// <summary>
/// UI組件使用記錄實體
/// </summary>
public class UIComponentUsage
{
    /// <summary>
    /// 使用記錄ID
    /// </summary>
    public long UsageId { get; set; }

    /// <summary>
    /// 組件ID
    /// </summary>
    public long ComponentId { get; set; }

    /// <summary>
    /// 使用模組代碼
    /// </summary>
    public string ModuleCode { get; set; } = string.Empty;

    /// <summary>
    /// 使用模組名稱
    /// </summary>
    public string? ModuleName { get; set; }

    /// <summary>
    /// 使用次數
    /// </summary>
    public int UsageCount { get; set; } = 0;

    /// <summary>
    /// 最後使用時間
    /// </summary>
    public DateTime? LastUsedAt { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

