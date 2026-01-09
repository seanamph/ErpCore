namespace ErpCore.Domain.Entities.CustomerCustom;

/// <summary>
/// CUS3000 活動實體 (SYS3510-SYS3580 - 活動管理)
/// </summary>
public class Cus3000Activity
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 活動編號
    /// </summary>
    public string ActivityId { get; set; } = string.Empty;

    /// <summary>
    /// 活動名稱
    /// </summary>
    public string ActivityName { get; set; } = string.Empty;

    /// <summary>
    /// 活動日期
    /// </summary>
    public DateTime ActivityDate { get; set; }

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

