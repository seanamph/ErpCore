namespace ErpCore.Domain.Interfaces;

/// <summary>
/// 領域事件介面
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// 事件發生時間
    /// </summary>
    DateTime OccurredOn { get; }
}

