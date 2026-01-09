using ErpCore.Domain.Entities.Loyalty;

namespace ErpCore.Infrastructure.Repositories.Loyalty;

/// <summary>
/// 忠誠度系統設定 Repository 介面 (WEBLOYALTYINI - 忠誠度系統初始化)
/// </summary>
public interface ILoyaltySystemConfigRepository
{
    Task<LoyaltySystemConfig?> GetByIdAsync(string configId);
    Task<IEnumerable<LoyaltySystemConfig>> QueryAsync(LoyaltySystemConfigQuery query);
    Task<int> GetCountAsync(LoyaltySystemConfigQuery query);
    Task<string> CreateAsync(LoyaltySystemConfig entity);
    Task UpdateAsync(LoyaltySystemConfig entity);
    Task DeleteAsync(string configId);
}

/// <summary>
/// 忠誠度系統設定查詢條件
/// </summary>
public class LoyaltySystemConfigQuery
{
    public string? ConfigId { get; set; }
    public string? ConfigType { get; set; }
    public string? Status { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

