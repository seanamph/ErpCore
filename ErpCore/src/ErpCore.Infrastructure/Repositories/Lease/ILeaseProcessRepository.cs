using ErpCore.Domain.Entities.Lease;

namespace ErpCore.Infrastructure.Repositories.Lease;

/// <summary>
/// 租賃處理 Repository 介面 (SYS8B50-SYS8B90)
/// </summary>
public interface ILeaseProcessRepository
{
    Task<LeaseProcess?> GetByIdAsync(string processId);
    Task<IEnumerable<LeaseProcess>> QueryAsync(LeaseProcessQuery query);
    Task<int> GetCountAsync(LeaseProcessQuery query);
    Task<IEnumerable<LeaseProcess>> GetByLeaseIdAsync(string leaseId);
    Task<bool> ExistsAsync(string processId);
    Task<LeaseProcess> CreateAsync(LeaseProcess process);
    Task<LeaseProcess> UpdateAsync(LeaseProcess process);
    Task DeleteAsync(string processId);
    Task UpdateStatusAsync(string processId, string processStatus);
}

/// <summary>
/// 租賃處理查詢條件
/// </summary>
public class LeaseProcessQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ProcessId { get; set; }
    public string? LeaseId { get; set; }
    public string? ProcessType { get; set; }
    public string? ProcessStatus { get; set; }
    public DateTime? ProcessDateFrom { get; set; }
    public DateTime? ProcessDateTo { get; set; }
}

