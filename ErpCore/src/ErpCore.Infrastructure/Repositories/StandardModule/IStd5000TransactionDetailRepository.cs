using ErpCore.Domain.Entities.StandardModule;

namespace ErpCore.Infrastructure.Repositories.StandardModule;

/// <summary>
/// STD5000 交易明細 Repository 介面 (SYS5310-SYS53C6 - 交易明細管理)
/// </summary>
public interface IStd5000TransactionDetailRepository
{
    Task<Std5000TransactionDetail?> GetByIdAsync(long tKey);
    Task<IEnumerable<Std5000TransactionDetail>> GetByTransIdAsync(string transId);
    Task<long> CreateAsync(Std5000TransactionDetail entity);
    Task UpdateAsync(Std5000TransactionDetail entity);
    Task DeleteAsync(long tKey);
    Task DeleteByTransIdAsync(string transId);
}

