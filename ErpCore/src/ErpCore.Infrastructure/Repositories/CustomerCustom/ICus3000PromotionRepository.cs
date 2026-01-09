using ErpCore.Domain.Entities.CustomerCustom;

namespace ErpCore.Infrastructure.Repositories.CustomerCustom;

/// <summary>
/// CUS3000 促銷活動 Repository 介面 (SYS3310-SYS3399 - 促銷活動管理)
/// </summary>
public interface ICus3000PromotionRepository
{
    Task<Cus3000Promotion?> GetByIdAsync(long tKey);
    Task<Cus3000Promotion?> GetByPromotionIdAsync(string promotionId);
    Task<IEnumerable<Cus3000Promotion>> QueryAsync(Cus3000PromotionQuery query);
    Task<int> GetCountAsync(Cus3000PromotionQuery query);
    Task<long> CreateAsync(Cus3000Promotion entity);
    Task UpdateAsync(Cus3000Promotion entity);
    Task DeleteAsync(long tKey);
}

/// <summary>
/// CUS3000 促銷活動查詢條件
/// </summary>
public class Cus3000PromotionQuery
{
    public string? PromotionId { get; set; }
    public string? PromotionName { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public DateTime? EndDateFrom { get; set; }
    public DateTime? EndDateTo { get; set; }
    public string? Status { get; set; }
    public string? Keyword { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

