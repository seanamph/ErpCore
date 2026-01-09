using ErpCore.Application.DTOs.StoreMember;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.StoreMember;

/// <summary>
/// 促銷活動服務介面 (SYS3510-SYS3600 - 促銷活動維護)
/// </summary>
public interface IPromotionService
{
    /// <summary>
    /// 查詢促銷活動列表
    /// </summary>
    Task<PagedResult<PromotionDto>> GetPromotionsAsync(PromotionQueryDto query);

    /// <summary>
    /// 查詢單筆促銷活動
    /// </summary>
    Task<PromotionDto> GetPromotionByIdAsync(string promotionId);

    /// <summary>
    /// 新增促銷活動
    /// </summary>
    Task<string> CreatePromotionAsync(CreatePromotionDto dto);

    /// <summary>
    /// 修改促銷活動
    /// </summary>
    Task UpdatePromotionAsync(string promotionId, UpdatePromotionDto dto);

    /// <summary>
    /// 刪除促銷活動
    /// </summary>
    Task DeletePromotionAsync(string promotionId);

    /// <summary>
    /// 更新促銷活動狀態
    /// </summary>
    Task UpdateStatusAsync(string promotionId, string status);

    /// <summary>
    /// 查詢促銷活動使用統計
    /// </summary>
    Task<PromotionStatisticsDto> GetStatisticsAsync(string promotionId);
}

