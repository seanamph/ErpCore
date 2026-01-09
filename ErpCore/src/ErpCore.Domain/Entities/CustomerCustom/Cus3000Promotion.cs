namespace ErpCore.Domain.Entities.CustomerCustom;

/// <summary>
/// CUS3000 促銷活動實體 (SYS3310-SYS3399 - 促銷活動管理)
/// </summary>
public class Cus3000Promotion
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 促銷活動編號
    /// </summary>
    public string PromotionId { get; set; } = string.Empty;

    /// <summary>
    /// 促銷活動名稱
    /// </summary>
    public string PromotionName { get; set; } = string.Empty;

    /// <summary>
    /// 開始日期
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// 結束日期
    /// </summary>
    public DateTime EndDate { get; set; }

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

