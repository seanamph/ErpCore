namespace ErpCore.Domain.Interfaces;

/// <summary>
/// 工作單元介面
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// 開始交易
    /// </summary>
    Task BeginTransactionAsync();

    /// <summary>
    /// 提交交易
    /// </summary>
    Task CommitAsync();

    /// <summary>
    /// 回滾交易
    /// </summary>
    Task RollbackAsync();
}

