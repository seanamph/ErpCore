using ErpCore.Domain.Entities.StoreMember;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.StoreMember;

/// <summary>
/// 促銷活動 Repository 介面 (SYS3510-SYS3600 - 促銷活動維護)
/// </summary>
public interface IPromotionRepository
{
    /// <summary>
    /// 根據促銷活動編號查詢
    /// </summary>
    Task<Promotion?> GetByIdAsync(string promotionId);

    /// <summary>
    /// 查詢促銷活動列表（分頁）
    /// </summary>
    Task<PagedResult<Promotion>> QueryAsync(PromotionQuery query);

    /// <summary>
    /// 查詢促銷活動總數
    /// </summary>
    Task<int> GetCountAsync(PromotionQuery query);

    /// <summary>
    /// 新增促銷活動
    /// </summary>
    Task<Promotion> CreateAsync(Promotion promotion);

    /// <summary>
    /// 修改促銷活動
    /// </summary>
    Task<Promotion> UpdateAsync(Promotion promotion);

    /// <summary>
    /// 刪除促銷活動
    /// </summary>
    Task DeleteAsync(string promotionId);

    /// <summary>
    /// 檢查促銷活動編號是否存在
    /// </summary>
    Task<bool> ExistsAsync(string promotionId);

    /// <summary>
    /// 更新促銷活動狀態
    /// </summary>
    Task UpdateStatusAsync(string promotionId, string status);
}

/// <summary>
/// 促銷活動查詢條件
/// </summary>
public class PromotionQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? PromotionId { get; set; }
    public string? PromotionName { get; set; }
    public string? PromotionType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Status { get; set; }
    public string? ShopId { get; set; }
}

