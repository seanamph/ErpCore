namespace ErpCore.Domain.Enums;

/// <summary>
/// 使用者狀態列舉
/// </summary>
public enum UserStatus
{
    /// <summary>
    /// 啟用
    /// </summary>
    Active = 1,

    /// <summary>
    /// 停用
    /// </summary>
    Inactive = 0,

    /// <summary>
    /// 鎖定
    /// </summary>
    Locked = 2
}

