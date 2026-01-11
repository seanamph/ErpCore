namespace ErpCore.Domain.Entities.System;

/// <summary>
/// 使用者7X承租分店權限實體 (SYS0111)
/// </summary>
public class UserStore
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 使用者編號
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 7X承租分店代號
    /// </summary>
    public string StoreId { get; set; } = string.Empty;

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
