using ErpCore.Application.DTOs.System;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 角色之使用者設定維護服務介面 (SYS0230)
/// </summary>
public interface IRoleUserAssignmentService
{
    /// <summary>
    /// 查詢角色使用者列表
    /// </summary>
    Task<PagedResult<RoleUserListItemDto>> GetRoleUsersAsync(RoleUserQueryDto query);

    /// <summary>
    /// 批量設定角色使用者
    /// </summary>
    Task<BatchAssignRoleUsersResultDto> BatchAssignRoleUsersAsync(string roleId, BatchAssignRoleUsersDto dto);

    /// <summary>
    /// 新增角色使用者
    /// </summary>
    Task AssignUserToRoleAsync(string roleId, string userId);

    /// <summary>
    /// 移除角色使用者
    /// </summary>
    Task RemoveUserFromRoleAsync(string roleId, string userId);
}
