using ErpCore.Domain.Entities.System;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 子系統項目 Repository 介面 (SYS0420)
/// </summary>
public interface IMenuRepository
{
    /// <summary>
    /// 根據子系統代碼查詢
    /// </summary>
    Task<Menu?> GetByIdAsync(string menuId);

    /// <summary>
    /// 查詢子系統列表（分頁）
    /// </summary>
    Task<PagedResult<Menu>> QueryAsync(MenuQuery query);

    /// <summary>
    /// 新增子系統
    /// </summary>
    Task<Menu> CreateAsync(Menu menu);

    /// <summary>
    /// 修改子系統
    /// </summary>
    Task<Menu> UpdateAsync(Menu menu);

    /// <summary>
    /// 刪除子系統
    /// </summary>
    Task DeleteAsync(string menuId);

    /// <summary>
    /// 檢查子系統是否存在
    /// </summary>
    Task<bool> ExistsAsync(string menuId);

    /// <summary>
    /// 檢查是否有下層子系統
    /// </summary>
    Task<bool> HasChildrenAsync(string menuId);

    /// <summary>
    /// 檢查是否有作業關聯
    /// </summary>
    Task<bool> HasProgramsAsync(string menuId);
}

/// <summary>
/// 子系統查詢條件
/// </summary>
public class MenuQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? MenuId { get; set; }
    public string? MenuName { get; set; }
    public string? SystemId { get; set; }
    public string? ParentMenuId { get; set; }
}
