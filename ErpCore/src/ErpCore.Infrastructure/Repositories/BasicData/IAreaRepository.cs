using ErpCore.Domain.Entities.BasicData;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.BasicData;

/// <summary>
/// 區域 Repository 介面
/// </summary>
public interface IAreaRepository
{
    /// <summary>
    /// 根據區域代碼查詢區域
    /// </summary>
    Task<Area?> GetByIdAsync(string areaId);

    /// <summary>
    /// 查詢區域列表（分頁）
    /// </summary>
    Task<PagedResult<Area>> QueryAsync(AreaQuery query);

    /// <summary>
    /// 新增區域
    /// </summary>
    Task<Area> CreateAsync(Area area);

    /// <summary>
    /// 修改區域
    /// </summary>
    Task<Area> UpdateAsync(Area area);

    /// <summary>
    /// 刪除區域
    /// </summary>
    Task DeleteAsync(string areaId);

    /// <summary>
    /// 檢查區域是否存在
    /// </summary>
    Task<bool> ExistsAsync(string areaId);
}

/// <summary>
/// 區域查詢條件
/// </summary>
public class AreaQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? AreaId { get; set; }
    public string? AreaName { get; set; }
    public string? Status { get; set; }
}

