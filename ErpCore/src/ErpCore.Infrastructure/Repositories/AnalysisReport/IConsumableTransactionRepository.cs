using ErpCore.Domain.Entities.AnalysisReport;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.AnalysisReport;

/// <summary>
/// 耗材異動記錄 Repository 介面 (SYSA255)
/// </summary>
public interface IConsumableTransactionRepository
{
    /// <summary>
    /// 查詢耗材使用明細
    /// </summary>
    Task<PagedResult<ConsumableTransaction>> GetTransactionsAsync(ConsumableTransactionQuery query);
}

/// <summary>
/// 耗材異動記錄查詢條件
/// </summary>
public class ConsumableTransactionQuery : PagedQuery
{
    public string ConsumableId { get; set; } = string.Empty;
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? TransactionType { get; set; }
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}
