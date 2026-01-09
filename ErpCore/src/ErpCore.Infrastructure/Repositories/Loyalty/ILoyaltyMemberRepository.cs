using ErpCore.Domain.Entities.Loyalty;

namespace ErpCore.Infrastructure.Repositories.Loyalty;

/// <summary>
/// 忠誠度會員 Repository 介面 (LPS - 忠誠度系統維護)
/// </summary>
public interface ILoyaltyMemberRepository
{
    Task<LoyaltyMember?> GetByCardNoAsync(string cardNo);
    Task<IEnumerable<LoyaltyMember>> QueryAsync(LoyaltyMemberQuery query);
    Task<int> GetCountAsync(LoyaltyMemberQuery query);
    Task<string> CreateAsync(LoyaltyMember entity);
    Task UpdateAsync(LoyaltyMember entity);
    Task UpdatePointsAsync(string cardNo, decimal totalPoints, decimal availablePoints);
}

/// <summary>
/// 忠誠度會員查詢條件
/// </summary>
public class LoyaltyMemberQuery
{
    public string? CardNo { get; set; }
    public string? MemberName { get; set; }
    public string? Status { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

