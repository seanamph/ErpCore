using ErpCore.Application.DTOs.BasicData;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.BasicData;

/// <summary>
/// 地區服務介面
/// </summary>
public interface IRegionService
{
    /// <summary>
    /// 查詢地區列表
    /// </summary>
    Task<PagedResult<RegionDto>> GetRegionsAsync(RegionQueryDto query);

    /// <summary>
    /// 查詢單筆地區
    /// </summary>
    Task<RegionDto> GetRegionAsync(string regionId);

    /// <summary>
    /// 新增地區
    /// </summary>
    Task<string> CreateRegionAsync(CreateRegionDto dto);

    /// <summary>
    /// 修改地區
    /// </summary>
    Task UpdateRegionAsync(string regionId, UpdateRegionDto dto);

    /// <summary>
    /// 刪除地區
    /// </summary>
    Task DeleteRegionAsync(string regionId);

    /// <summary>
    /// 批次刪除地區
    /// </summary>
    Task DeleteRegionsBatchAsync(BatchDeleteRegionDto dto);
}
