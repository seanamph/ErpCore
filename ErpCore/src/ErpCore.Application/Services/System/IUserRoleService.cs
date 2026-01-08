using ErpCore.Application.DTOs.System;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 使用者角色服務介面 (SYS0220)
/// </summary>
public interface IUserRoleService
{
    /// <summary>
    /// 查詢使用者已分配的角色列表
    /// </summary>
    Task<PagedResult<UserRoleDto>> GetUserRolesAsync(string userId, int pageIndex = 1, int pageSize = 20);

    /// <summary>
    /// 查詢可用角色列表（排除已分配的角色）
    /// </summary>
    Task<PagedResult<RoleDto>> GetAvailableRolesAsync(string userId, string? keyword = null, int pageIndex = 1, int pageSize = 20);

    /// <summary>
    /// 為使用者分配角色
    /// </summary>
    Task<AssignRoleResultDto> AssignRolesAsync(string userId, List<string> roleIds);

    /// <summary>
    /// 移除使用者的角色
    /// </summary>
    Task<RemoveRoleResultDto> RemoveRolesAsync(string userId, List<string> roleIds);

    /// <summary>
    /// 批量更新使用者角色
    /// </summary>
    Task<BatchUpdateRoleResultDto> BatchUpdateRolesAsync(string userId, List<string> roleIds);
}

