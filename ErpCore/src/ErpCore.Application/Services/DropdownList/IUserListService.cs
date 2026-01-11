using ErpCore.Application.DTOs.DropdownList;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.DropdownList;

/// <summary>
/// 使用者列表服務介面 (USER_LIST)
/// </summary>
public interface IUserListService
{
    /// <summary>
    /// 查詢使用者列表 (USER_LIST_USER_LIST)
    /// </summary>
    Task<PagedResult<UserListDto>> GetUserListAsync(UserListQueryDto query);

    /// <summary>
    /// 查詢部門使用者列表 (USER_LIST_DEPT_LIST)
    /// </summary>
    Task<PagedResult<UserListDto>> GetDeptUserListAsync(DeptUserListQueryDto query);

    /// <summary>
    /// 查詢其他使用者列表 (USER_LIST_OTHER_LIST)
    /// </summary>
    Task<PagedResult<UserListDto>> GetOtherUserListAsync(UserListQueryDto query);
}
