using ErpCore.Domain.Entities.Procurement;

namespace ErpCore.Infrastructure.Repositories.Procurement;

/// <summary>
/// 採購其他功能 Repository 介面 (SYSP510-SYSP530)
/// </summary>
public interface IProcurementOtherRepository
{
    Task<ProcurementOther?> GetByIdAsync(long tKey);
    Task<ProcurementOther?> GetByFunctionIdAsync(string functionId);
    Task<IEnumerable<ProcurementOther>> QueryAsync(ProcurementOtherQuery query);
    Task<int> GetCountAsync(ProcurementOtherQuery query);
    Task<bool> ExistsAsync(string functionId);
    Task<ProcurementOther> CreateAsync(ProcurementOther procurementOther);
    Task<ProcurementOther> UpdateAsync(ProcurementOther procurementOther);
    Task DeleteAsync(long tKey);
}

/// <summary>
/// 採購其他功能查詢條件
/// </summary>
public class ProcurementOtherQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? FunctionId { get; set; }
    public string? FunctionName { get; set; }
    public string? FunctionType { get; set; }
    public string? Status { get; set; }
}

