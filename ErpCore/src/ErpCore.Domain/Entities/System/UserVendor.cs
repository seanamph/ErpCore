namespace ErpCore.Domain.Entities.System;

/// <summary>
/// 使用者廠商權限實體 (SYS0113)
/// </summary>
public class UserVendor
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
    /// 廠商代號
    /// </summary>
    public string VendorId { get; set; } = string.Empty;

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
