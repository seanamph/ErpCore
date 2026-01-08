using ErpCore.Application.DTOs.UiComponent;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.UiComponent;

/// <summary>
/// UI組件查詢與報表服務介面
/// </summary>
public interface IUiComponentQueryService
{
    /// <summary>
    /// 查詢UI組件使用情況
    /// </summary>
    Task<PagedResult<UIComponentUsageStatsDto>> GetUsageStatsAsync(UIComponentUsageQueryDto query);

    /// <summary>
    /// 取得UI組件使用統計資訊
    /// </summary>
    Task<UIComponentUsageSummaryDto> GetUsageSummaryAsync();
}

/// <summary>
/// UI組件使用統計摘要 DTO
/// </summary>
public class UIComponentUsageSummaryDto
{
    public int TotalComponents { get; set; }
    public int ActiveComponents { get; set; }
    public int TotalUsageCount { get; set; }
    public List<UIComponentUsageStatsDto> TopUsedComponents { get; set; } = new();
}

