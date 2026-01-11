using ErpCore.Application.DTOs.System;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 子系統項目服務介面 (SYS0420)
/// </summary>
public interface IMenuService
{
    /// <summary>
    /// 查詢子系統列表
    /// </summary>
    Task<PagedResult<MenuDto>> GetMenusAsync(MenuQueryDto query);

    /// <summary>
    /// 查詢單筆子系統
    /// </summary>
    Task<MenuDto> GetMenuAsync(string menuId);

    /// <summary>
    /// 新增子系統
    /// </summary>
    Task<string> CreateMenuAsync(CreateMenuDto dto);

    /// <summary>
    /// 修改子系統
    /// </summary>
    Task UpdateMenuAsync(string menuId, UpdateMenuDto dto);

    /// <summary>
    /// 刪除子系統
    /// </summary>
    Task DeleteMenuAsync(string menuId);

    /// <summary>
    /// 批次刪除子系統
    /// </summary>
    Task DeleteMenusBatchAsync(BatchDeleteMenusDto dto);
}
