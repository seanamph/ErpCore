using ErpCore.Domain.Entities.TaxAccounting;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.TaxAccounting;

/// <summary>
/// 現金流量中分類 Repository 介面 (SYST132)
/// </summary>
public interface ICashFlowMediumTypeRepository
{
    Task<CashFlowMediumType?> GetByIdAsync(string cashLTypeId, string cashMTypeId);
    Task<PagedResult<CashFlowMediumType>> QueryAsync(CashFlowMediumTypeQuery query);
    Task<CashFlowMediumType> CreateAsync(CashFlowMediumType entity);
    Task<CashFlowMediumType> UpdateAsync(CashFlowMediumType entity);
    Task DeleteAsync(string cashLTypeId, string cashMTypeId);
    Task<bool> ExistsAsync(string cashLTypeId, string cashMTypeId);
    Task<bool> HasSubjectTypesAsync(string cashMTypeId);
}

/// <summary>
/// 現金流量中分類查詢條件
/// </summary>
public class CashFlowMediumTypeQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? CashLTypeId { get; set; }
    public string? CashMTypeId { get; set; }
    public string? CashMTypeName { get; set; }
    public string? AbItem { get; set; }
}

