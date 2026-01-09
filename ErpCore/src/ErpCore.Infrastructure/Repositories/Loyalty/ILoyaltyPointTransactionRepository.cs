using ErpCore.Domain.Entities.Loyalty;

namespace ErpCore.Infrastructure.Repositories.Loyalty;

/// <summary>
/// 忠誠度點數交易 Repository 介面 (LPS - 忠誠度系統維護)
/// </summary>
public interface ILoyaltyPointTransactionRepository
{
    Task<LoyaltyPointTransaction?> GetByRrnAsync(string rrn);
    Task<IEnumerable<LoyaltyPointTransaction>> QueryAsync(LoyaltyPointTransactionQuery query);
    Task<int> GetCountAsync(LoyaltyPointTransactionQuery query);
    Task<long> CreateAsync(LoyaltyPointTransaction entity);
    Task UpdateAsync(LoyaltyPointTransaction entity);
    Task VoidTransactionAsync(string rrn, string reversalFlag, string voidFlag);
    Task<string> GenerateRrnAsync();
}

/// <summary>
/// 忠誠度點數交易查詢條件
/// </summary>
public class LoyaltyPointTransactionQuery
{
    public string? RRN { get; set; }
    public string? CardNo { get; set; }
    public string? TransType { get; set; }
    public string? Status { get; set; }
    public DateTime? TransTimeFrom { get; set; }
    public DateTime? TransTimeTo { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

