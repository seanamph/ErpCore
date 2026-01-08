using ErpCore.Domain.Entities.System;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 使用者角色 Repository 介面 (SYS0220)
/// </summary>
public interface IUserRoleRepository
{
    /// <summary>
    /// 根據使用者編號查詢角色列表
    /// </summary>
    Task<IEnumerable<UserRole>> GetByUserIdAsync(string userId);

    /// <summary>
    /// 根據使用者編號和角色編號查詢
    /// </summary>
    Task<UserRole?> GetByUserIdAndRoleIdAsync(string userId, string roleId);

    /// <summary>
    /// 根據使用者編號和角色編號列表查詢
    /// </summary>
    Task<IEnumerable<UserRole>> GetByUserIdAndRoleIdsAsync(string userId, List<string> roleIds);

    /// <summary>
    /// 根據使用者編號查詢角色ID列表
    /// </summary>
    Task<List<string>> GetRoleIdsByUserIdAsync(string userId);

    /// <summary>
    /// 查詢使用者已分配的角色列表（分頁）
    /// </summary>
    Task<PagedResult<UserRole>> GetUserRolesAsync(string userId, int pageIndex = 1, int pageSize = 20);

    /// <summary>
    /// 新增使用者角色
    /// </summary>
    Task<UserRole> CreateAsync(UserRole userRole);

    /// <summary>
    /// 批量新增使用者角色
    /// </summary>
    Task CreateRangeAsync(IEnumerable<UserRole> userRoles);

    /// <summary>
    /// 刪除使用者角色
    /// </summary>
    Task DeleteAsync(string userId, string roleId);

    /// <summary>
    /// 批量刪除使用者角色
    /// </summary>
    Task DeleteRangeAsync(IEnumerable<UserRole> userRoles);

    /// <summary>
    /// 檢查使用者角色是否存在
    /// </summary>
    Task<bool> ExistsAsync(string userId, string roleId);
}

