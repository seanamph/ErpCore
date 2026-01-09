using ErpCore.Domain.Entities.CustomerCustom;

namespace ErpCore.Infrastructure.Repositories.CustomerCustom;

/// <summary>
/// CUS3000 活動 Repository 介面 (SYS3510-SYS3580 - 活動管理)
/// </summary>
public interface ICus3000ActivityRepository
{
    Task<Cus3000Activity?> GetByIdAsync(long tKey);
    Task<Cus3000Activity?> GetByActivityIdAsync(string activityId);
    Task<IEnumerable<Cus3000Activity>> QueryAsync(Cus3000ActivityQuery query);
    Task<int> GetCountAsync(Cus3000ActivityQuery query);
    Task<long> CreateAsync(Cus3000Activity entity);
    Task UpdateAsync(Cus3000Activity entity);
    Task DeleteAsync(long tKey);
}

/// <summary>
/// CUS3000 活動查詢條件
/// </summary>
public class Cus3000ActivityQuery
{
    public string? ActivityId { get; set; }
    public string? ActivityName { get; set; }
    public DateTime? ActivityDateFrom { get; set; }
    public DateTime? ActivityDateTo { get; set; }
    public string? Status { get; set; }
    public string? Keyword { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

