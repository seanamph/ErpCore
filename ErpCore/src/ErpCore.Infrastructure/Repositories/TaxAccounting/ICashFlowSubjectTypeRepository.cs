using ErpCore.Domain.Entities.TaxAccounting;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.TaxAccounting;

/// <summary>
/// 現金流量科目設定 Repository 介面 (SYST133)
/// </summary>
public interface ICashFlowSubjectTypeRepository
{
    Task<CashFlowSubjectType?> GetByIdAsync(string cashMTypeId, string cashSTypeId);
    Task<PagedResult<CashFlowSubjectType>> QueryAsync(CashFlowSubjectTypeQuery query);
    Task<CashFlowSubjectType> CreateAsync(CashFlowSubjectType entity);
    Task<CashFlowSubjectType> UpdateAsync(CashFlowSubjectType entity);
    Task DeleteAsync(string cashMTypeId, string cashSTypeId);
    Task<bool> ExistsAsync(string cashMTypeId, string cashSTypeId);
}

/// <summary>
/// 現金流量科目設定查詢條件
/// </summary>
public class CashFlowSubjectTypeQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? CashMTypeId { get; set; }
    public string? CashSTypeId { get; set; }
}

