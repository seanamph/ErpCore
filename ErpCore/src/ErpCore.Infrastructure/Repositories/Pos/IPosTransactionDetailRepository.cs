using ErpCore.Domain.Entities.Pos;

namespace ErpCore.Infrastructure.Repositories.Pos;

/// <summary>
/// POS交易明細 Repository 介面
/// </summary>
public interface IPosTransactionDetailRepository
{
    /// <summary>
    /// 根據交易編號查詢明細
    /// </summary>
    Task<IEnumerable<PosTransactionDetail>> GetByTransactionIdAsync(string transactionId);

    /// <summary>
    /// 新增POS交易明細
    /// </summary>
    Task<PosTransactionDetail> CreateAsync(PosTransactionDetail detail);

    /// <summary>
    /// 批次新增POS交易明細
    /// </summary>
    Task<int> BatchCreateAsync(IEnumerable<PosTransactionDetail> details);
}

