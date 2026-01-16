using ErpCore.Domain.Entities.DropdownList;
using ErpCore.Infrastructure.Repositories.Queries;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.DropdownList;

/// <summary>
/// 選單 Repository 介面
/// </summary>
public interface IMenuRepository
{
    Task<Menu?> GetByIdAsync(string menuId);
    Task<PagedResult<Menu>> QueryAsync(MenuQuery query);
    Task<IEnumerable<MenuOption>> GetOptionsAsync(string? systemId = null, string? status = "1");
}

