using ErpCore.Application.DTOs.StoreMember;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.StoreMember;

/// <summary>
/// 商店報表服務介面 (SYS3410-SYS3440 - 報表查詢作業)
/// </summary>
public interface IStoreReportService
{
    /// <summary>
    /// 查詢會員統計報表
    /// </summary>
    Task<PagedResult<MemberStatisticsReportDto>> GetMemberStatisticsAsync(StoreReportQueryDto query);

    /// <summary>
    /// 查詢商店銷售報表
    /// </summary>
    Task<PagedResult<ShopSalesReportDto>> GetShopSalesAsync(StoreReportQueryDto query);

    /// <summary>
    /// 查詢促銷活動效果報表
    /// </summary>
    Task<PagedResult<PromotionEffectReportDto>> GetPromotionEffectAsync(StoreReportQueryDto query);

    /// <summary>
    /// 匯出報表
    /// </summary>
    Task<byte[]> ExportReportAsync(StoreReportExportDto request, string exportType);

    /// <summary>
    /// 列印報表
    /// </summary>
    Task<byte[]> PrintReportAsync(StoreReportExportDto request);
}

