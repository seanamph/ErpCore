using ErpCore.Domain.Entities.TaxAccounting;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.TaxAccounting;

/// <summary>
/// 現金流量小計設定 Repository 介面 (SYST134)
/// </summary>
public interface ICashFlowSubTotalRepository
{
    Task<CashFlowSubTotal?> GetByIdAsync(string cashLTypeId, string cashSubId);
    Task<PagedResult<CashFlowSubTotal>> QueryAsync(CashFlowSubTotalQuery query);
    Task<CashFlowSubTotal> CreateAsync(CashFlowSubTotal entity);
    Task<CashFlowSubTotal> UpdateAsync(CashFlowSubTotal entity);
    Task DeleteAsync(string cashLTypeId, string cashSubId);
    Task<bool> ExistsAsync(string cashLTypeId, string cashSubId);
}

/// <summary>
/// 現金流量小計設定查詢條件
/// </summary>
public class CashFlowSubTotalQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? CashLTypeId { get; set; }
    public string? CashSubId { get; set; }
}

