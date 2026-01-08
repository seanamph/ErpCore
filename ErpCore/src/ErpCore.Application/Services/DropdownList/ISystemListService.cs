using ErpCore.Application.DTOs.DropdownList;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.DropdownList;

/// <summary>
/// 系統列表服務介面
/// </summary>
public interface ISystemListService
{
    // 系統列表 (SYSID_LIST)
    Task<IEnumerable<SystemListDto>> GetSystemListAsync(SystemListQueryDto query);
    Task<IEnumerable<OptionDto>> GetSystemOptionsAsync(string? status = null, string? excludeSystems = null);

    // 使用者列表 (USER_LIST)
    Task<PagedResult<UserListDto>> GetUserListAsync(UserListQueryDto query);
    Task<IEnumerable<OptionDto>> GetUserListOptionsAsync(string? orgId = null, string? status = "A", string? keyword = null);
}

