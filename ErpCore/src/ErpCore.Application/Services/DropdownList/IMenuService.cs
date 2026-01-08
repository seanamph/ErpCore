using ErpCore.Application.DTOs.DropdownList;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.DropdownList;

/// <summary>
/// 選單服務介面
/// </summary>
public interface IMenuService
{
    Task<MenuDto?> GetMenuAsync(string menuId);
    Task<PagedResult<MenuDto>> GetMenusAsync(MenuQueryDto query);
    Task<IEnumerable<MenuOptionDto>> GetMenuOptionsAsync(string? systemId = null, string? status = "1");
}

