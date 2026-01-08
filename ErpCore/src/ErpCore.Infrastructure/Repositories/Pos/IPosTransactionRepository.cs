using ErpCore.Domain.Entities.Pos;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.Pos;

/// <summary>
/// POS交易 Repository 介面
/// </summary>
public interface IPosTransactionRepository
{
    /// <summary>
    /// 根據交易編號查詢
    /// </summary>
    Task<PosTransaction?> GetByTransactionIdAsync(string transactionId);

    /// <summary>
    /// 查詢POS交易列表（分頁）
    /// </summary>
    Task<PagedResult<PosTransaction>> QueryAsync(PosTransactionQuery query);

    /// <summary>
    /// 新增POS交易
    /// </summary>
    Task<PosTransaction> CreateAsync(PosTransaction transaction);

    /// <summary>
    /// 修改POS交易
    /// </summary>
    Task<PosTransaction> UpdateAsync(PosTransaction transaction);

    /// <summary>
    /// 批次新增POS交易
    /// </summary>
    Task<int> BatchCreateAsync(IEnumerable<PosTransaction> transactions);

    /// <summary>
    /// 更新同步狀態
    /// </summary>
    Task UpdateSyncStatusAsync(string transactionId, string status, DateTime? syncAt, string? errorMessage);
}

/// <summary>
/// POS交易查詢條件
/// </summary>
public class PosTransactionQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? TransactionId { get; set; }
    public string? StoreId { get; set; }
    public string? PosId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? TransactionType { get; set; }
    public string? Status { get; set; }
}

