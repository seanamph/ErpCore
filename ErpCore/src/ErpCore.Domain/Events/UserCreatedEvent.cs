using ErpCore.Domain.Interfaces;

namespace ErpCore.Domain.Events;

/// <summary>
/// 使用者建立事件
/// </summary>
public class UserCreatedEvent : IDomainEvent
{
    /// <summary>
    /// 使用者編號
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 使用者名稱
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 事件發生時間
    /// </summary>
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    /// <summary>
    /// 建構函式
    /// </summary>
    public UserCreatedEvent(string userId, string userName)
    {
        UserId = userId;
        UserName = userName;
    }
}

