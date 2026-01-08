using ErpCore.Domain.Entities.Lease;

namespace ErpCore.Infrastructure.Repositories.Lease;

/// <summary>
/// 租賃合同資料 Repository 介面 (SYSM111-SYSM138)
/// </summary>
public interface ILeaseContractRepository
{
    Task<LeaseContract?> GetByIdAsync(string contractNo);
    Task<IEnumerable<LeaseContract>> GetByLeaseIdAsync(string leaseId);
    Task<IEnumerable<LeaseContract>> QueryAsync(LeaseContractQuery query);
    Task<int> GetCountAsync(LeaseContractQuery query);
    Task<bool> ExistsAsync(string contractNo);
    Task<LeaseContract> CreateAsync(LeaseContract contract);
    Task<LeaseContract> UpdateAsync(LeaseContract contract);
    Task DeleteAsync(string contractNo);
    Task UpdateStatusAsync(string contractNo, string status);
}

/// <summary>
/// 租賃合同查詢條件
/// </summary>
public class LeaseContractQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ContractNo { get; set; }
    public string? LeaseId { get; set; }
    public string? ContractType { get; set; }
    public string? Status { get; set; }
    public DateTime? ContractDateFrom { get; set; }
    public DateTime? ContractDateTo { get; set; }
}

