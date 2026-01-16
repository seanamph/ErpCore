using ErpCore.Domain.Entities.BasicData;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.BasicData;

/// <summary>
/// 地區 Repository 介面
/// </summary>
public interface IRegionRepository
{
    /// <summary>
    /// 根據地區編號查詢地區
    /// </summary>
    Task<Region?> GetByIdAsync(string regionId);

    /// <summary>
    /// 查詢地區列表（分頁）
    /// </summary>
    Task<PagedResult<Region>> QueryAsync(RegionQuery query);

    /// <summary>
    /// 新增地區
    /// </summary>
    Task<Region> CreateAsync(Region region);

    /// <summary>
    /// 修改地區
    /// </summary>
    Task<Region> UpdateAsync(Region region);

    /// <summary>
    /// 刪除地區
    /// </summary>
    Task DeleteAsync(string regionId);

    /// <summary>
    /// 檢查地區是否存在
    /// </summary>
    Task<bool> ExistsAsync(string regionId);
}

/// <summary>
/// 地區查詢條件
/// </summary>
public class RegionQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? RegionId { get; set; }
    public string? RegionName { get; set; }
}
