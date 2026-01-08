using ErpCore.Application.DTOs.DropdownList;
using ErpCore.Domain.Entities.DropdownList;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.DropdownList;

/// <summary>
/// 選單 Repository 介面
/// </summary>
public interface IMenuRepository
{
    Task<Menu?> GetByIdAsync(string menuId);
    Task<PagedResult<Menu>> QueryAsync(MenuQueryDto query);
    Task<IEnumerable<MenuOptionDto>> GetOptionsAsync(string? systemId = null, string? status = "1");
}

