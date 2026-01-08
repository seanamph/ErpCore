namespace ErpCore.Domain.Entities.Customer;

/// <summary>
/// 查詢歷史記錄實體 (CUS5120)
/// </summary>
public class QueryHistory
{
    /// <summary>
    /// 歷史記錄ID
    /// </summary>
    public Guid HistoryId { get; set; }

    /// <summary>
    /// 使用者ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 模組代碼
    /// </summary>
    public string ModuleCode { get; set; } = string.Empty;

    /// <summary>
    /// 查詢名稱
    /// </summary>
    public string? QueryName { get; set; }

    /// <summary>
    /// 查詢條件(JSON格式)
    /// </summary>
    public string? QueryConditions { get; set; }

    /// <summary>
    /// 是否為常用查詢
    /// </summary>
    public bool IsFavorite { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

