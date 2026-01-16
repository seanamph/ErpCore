using LeaseEntity = ErpCore.Domain.Entities.Lease.Lease;

namespace ErpCore.Infrastructure.Repositories.Lease;

/// <summary>
/// 租賃 Repository 介面 (SYS8110-SYS8220)
/// </summary>
public interface ILeaseRepository
{
    Task<LeaseEntity?> GetByIdAsync(string leaseId);
    Task<IEnumerable<LeaseEntity>> QueryAsync(LeaseQuery query);
    Task<int> GetCountAsync(LeaseQuery query);
    Task<bool> ExistsAsync(string leaseId);
    Task<LeaseEntity> CreateAsync(LeaseEntity lease);
    Task<LeaseEntity> UpdateAsync(LeaseEntity lease);
    Task DeleteAsync(string leaseId);
    Task UpdateStatusAsync(string leaseId, string status);
}

/// <summary>
/// 租賃查詢條件
/// </summary>
public class LeaseQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? LeaseId { get; set; }
    public string? TenantId { get; set; }
    public string? ShopId { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public DateTime? EndDateFrom { get; set; }
    public DateTime? EndDateTo { get; set; }
}

