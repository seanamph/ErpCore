namespace ErpCore.Domain.Enums;

/// <summary>
/// 訂單狀態列舉
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// 待處理
    /// </summary>
    Pending = 0,

    /// <summary>
    /// 處理中
    /// </summary>
    Processing = 1,

    /// <summary>
    /// 已完成
    /// </summary>
    Completed = 2,

    /// <summary>
    /// 已取消
    /// </summary>
    Cancelled = 3
}

