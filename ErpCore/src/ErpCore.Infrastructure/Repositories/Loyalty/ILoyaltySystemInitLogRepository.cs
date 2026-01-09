using ErpCore.Domain.Entities.Loyalty;

namespace ErpCore.Infrastructure.Repositories.Loyalty;

/// <summary>
/// 忠誠度系統初始化記錄 Repository 介面 (WEBLOYALTYINI - 忠誠度系統初始化)
/// </summary>
public interface ILoyaltySystemInitLogRepository
{
    Task<LoyaltySystemInitLog?> GetByIdAsync(string initId);
    Task<IEnumerable<LoyaltySystemInitLog>> QueryAsync(LoyaltySystemInitLogQuery query);
    Task<int> GetCountAsync(LoyaltySystemInitLogQuery query);
    Task<string> CreateAsync(LoyaltySystemInitLog entity);
    Task<string> GenerateInitIdAsync();
}

/// <summary>
/// 忠誠度系統初始化記錄查詢條件
/// </summary>
public class LoyaltySystemInitLogQuery
{
    public string? InitId { get; set; }
    public string? InitStatus { get; set; }
    public DateTime? InitDateFrom { get; set; }
    public DateTime? InitDateTo { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

