using ErpCore.Domain.Entities.Lease;

namespace ErpCore.Infrastructure.Repositories.Lease;

/// <summary>
/// 租賃條件 Repository 介面 (SYSE110-SYSE140)
/// </summary>
public interface ILeaseTermRepository
{
    Task<LeaseTerm?> GetByIdAsync(long tKey);
    Task<IEnumerable<LeaseTerm>> GetByLeaseIdAndVersionAsync(string leaseId, string version);
    Task<IEnumerable<LeaseTerm>> QueryAsync(LeaseTermQuery query);
    Task<int> GetCountAsync(LeaseTermQuery query);
    Task<bool> ExistsAsync(long tKey);
    Task<LeaseTerm> CreateAsync(LeaseTerm leaseTerm);
    Task<LeaseTerm> UpdateAsync(LeaseTerm leaseTerm);
    Task DeleteAsync(long tKey);
    Task DeleteByLeaseIdAndVersionAsync(string leaseId, string version);
}

/// <summary>
/// 租賃條件查詢條件
/// </summary>
public class LeaseTermQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? LeaseId { get; set; }
    public string? Version { get; set; }
    public string? TermType { get; set; }
}

