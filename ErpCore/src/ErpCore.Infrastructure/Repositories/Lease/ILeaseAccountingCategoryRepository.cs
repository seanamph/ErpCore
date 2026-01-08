using ErpCore.Domain.Entities.Lease;

namespace ErpCore.Infrastructure.Repositories.Lease;

/// <summary>
/// 租賃會計分類 Repository 介面 (SYSE110-SYSE140)
/// </summary>
public interface ILeaseAccountingCategoryRepository
{
    Task<LeaseAccountingCategory?> GetByIdAsync(long tKey);
    Task<IEnumerable<LeaseAccountingCategory>> GetByLeaseIdAndVersionAsync(string leaseId, string version);
    Task<IEnumerable<LeaseAccountingCategory>> QueryAsync(LeaseAccountingCategoryQuery query);
    Task<int> GetCountAsync(LeaseAccountingCategoryQuery query);
    Task<bool> ExistsAsync(long tKey);
    Task<LeaseAccountingCategory> CreateAsync(LeaseAccountingCategory category);
    Task<LeaseAccountingCategory> UpdateAsync(LeaseAccountingCategory category);
    Task DeleteAsync(long tKey);
    Task DeleteByLeaseIdAndVersionAsync(string leaseId, string version);
}

/// <summary>
/// 租賃會計分類查詢條件
/// </summary>
public class LeaseAccountingCategoryQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? LeaseId { get; set; }
    public string? Version { get; set; }
    public string? CategoryId { get; set; }
}

