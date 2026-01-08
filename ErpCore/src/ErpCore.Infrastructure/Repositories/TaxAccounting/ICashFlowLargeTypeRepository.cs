using ErpCore.Domain.Entities.TaxAccounting;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.TaxAccounting;

/// <summary>
/// 現金流量大分類 Repository 介面 (SYST131)
/// </summary>
public interface ICashFlowLargeTypeRepository
{
    Task<CashFlowLargeType?> GetByIdAsync(string cashLTypeId);
    Task<PagedResult<CashFlowLargeType>> QueryAsync(CashFlowLargeTypeQuery query);
    Task<CashFlowLargeType> CreateAsync(CashFlowLargeType entity);
    Task<CashFlowLargeType> UpdateAsync(CashFlowLargeType entity);
    Task DeleteAsync(string cashLTypeId);
    Task<bool> ExistsAsync(string cashLTypeId);
    Task<bool> HasMediumTypesAsync(string cashLTypeId);
}

/// <summary>
/// 現金流量大分類查詢條件
/// </summary>
public class CashFlowLargeTypeQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? CashLTypeId { get; set; }
    public string? CashLTypeName { get; set; }
    public string? AbItem { get; set; }
}

