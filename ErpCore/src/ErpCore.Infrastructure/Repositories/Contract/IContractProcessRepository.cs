using ErpCore.Domain.Entities.Contract;

namespace ErpCore.Infrastructure.Repositories.Contract;

/// <summary>
/// 合同處理 Repository 介面 (SYSF210-SYSF220)
/// </summary>
public interface IContractProcessRepository
{
    Task<ContractProcess?> GetByIdAsync(string processId);
    Task<IEnumerable<ContractProcess>> QueryAsync(ContractProcessQuery query);
    Task<int> GetCountAsync(ContractProcessQuery query);
    Task<bool> ExistsAsync(string processId);
    Task<ContractProcess> CreateAsync(ContractProcess process);
    Task<ContractProcess> UpdateAsync(ContractProcess process);
    Task DeleteAsync(string processId);
}

/// <summary>
/// 合同處理查詢條件
/// </summary>
public class ContractProcessQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ProcessId { get; set; }
    public string? ContractId { get; set; }
    public int? Version { get; set; }
    public string? ProcessType { get; set; }
    public string? Status { get; set; }
    public DateTime? ProcessDateFrom { get; set; }
    public DateTime? ProcessDateTo { get; set; }
}

