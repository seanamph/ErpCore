using ErpCore.Application.DTOs.CustomerCustom;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.CustomerCustom;

/// <summary>
/// CUS3000 促銷活動服務介面 (SYS3310-SYS3399 - 促銷活動管理)
/// </summary>
public interface ICus3000PromotionService
{
    Task<PagedResult<Cus3000PromotionDto>> GetCus3000PromotionListAsync(Cus3000PromotionQueryDto query);
    Task<Cus3000PromotionDto?> GetCus3000PromotionByIdAsync(long tKey);
    Task<Cus3000PromotionDto?> GetCus3000PromotionByPromotionIdAsync(string promotionId);
    Task<long> CreateCus3000PromotionAsync(CreateCus3000PromotionDto dto);
    Task UpdateCus3000PromotionAsync(long tKey, UpdateCus3000PromotionDto dto);
    Task DeleteCus3000PromotionAsync(long tKey);
}

