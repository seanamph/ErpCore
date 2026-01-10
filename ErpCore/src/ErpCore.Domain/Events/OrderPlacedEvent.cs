using ErpCore.Domain.Interfaces;

namespace ErpCore.Domain.Events;

/// <summary>
/// 訂單建立事件
/// </summary>
public class OrderPlacedEvent : IDomainEvent
{
    /// <summary>
    /// 訂單編號
    /// </summary>
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// 使用者編號
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 訂單金額
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 事件發生時間
    /// </summary>
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    /// <summary>
    /// 建構函式
    /// </summary>
    public OrderPlacedEvent(string orderId, string userId, decimal amount)
    {
        OrderId = orderId;
        UserId = userId;
        Amount = amount;
    }
}

