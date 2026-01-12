namespace ErpCore.Domain.Entities.System;

/// <summary>
/// 使用者部門權限實體 (SYS0113)
/// </summary>
public class UserDepartment
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
    /// 部門代號
    /// </summary>
    public string DeptId { get; set; } = string.Empty;

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
