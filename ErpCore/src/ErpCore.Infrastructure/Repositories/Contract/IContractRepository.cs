using ErpCore.Domain.Entities.Contract;

namespace ErpCore.Infrastructure.Repositories.Contract;

/// <summary>
/// 合同 Repository 介面 (SYSF110-SYSF140)
/// </summary>
public interface IContractRepository
{
    Task<Contract?> GetByIdAsync(string contractId, int version);
    Task<IEnumerable<Contract>> QueryAsync(ContractQuery query);
    Task<int> GetCountAsync(ContractQuery query);
    Task<bool> ExistsAsync(string contractId, int version);
    Task<Contract> CreateAsync(Contract contract);
    Task<Contract> UpdateAsync(Contract contract);
    Task DeleteAsync(string contractId, int version);
    Task<int> GetNextVersionAsync(string contractId);
}

/// <summary>
/// 合同查詢條件
/// </summary>
public class ContractQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ContractId { get; set; }
    public string? ContractType { get; set; }
    public string? VendorId { get; set; }
    public string? Status { get; set; }
    public DateTime? EffectiveDateFrom { get; set; }
    public DateTime? EffectiveDateTo { get; set; }
}

