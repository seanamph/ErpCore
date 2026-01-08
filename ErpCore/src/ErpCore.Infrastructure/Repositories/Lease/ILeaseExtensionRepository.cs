using ErpCore.Domain.Entities.Lease;

namespace ErpCore.Infrastructure.Repositories.Lease;

/// <summary>
/// 租賃擴展 Repository 介面 (SYS8A10-SYS8A45)
/// </summary>
public interface ILeaseExtensionRepository
{
    Task<LeaseExtension?> GetByIdAsync(string extensionId);
    Task<IEnumerable<LeaseExtension>> QueryAsync(LeaseExtensionQuery query);
    Task<int> GetCountAsync(LeaseExtensionQuery query);
    Task<IEnumerable<LeaseExtension>> GetByLeaseIdAsync(string leaseId);
    Task<bool> ExistsAsync(string extensionId);
    Task<LeaseExtension> CreateAsync(LeaseExtension extension);
    Task<LeaseExtension> UpdateAsync(LeaseExtension extension);
    Task DeleteAsync(string extensionId);
    Task UpdateStatusAsync(string extensionId, string status);
}

/// <summary>
/// 租賃擴展查詢條件
/// </summary>
public class LeaseExtensionQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ExtensionId { get; set; }
    public string? LeaseId { get; set; }
    public string? ExtensionType { get; set; }
    public string? Status { get; set; }
}

