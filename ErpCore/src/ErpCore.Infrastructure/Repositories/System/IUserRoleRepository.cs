using ErpCore.Domain.Entities.System;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 角色使用者查詢條件 (SYS0230)
/// </summary>
public class RoleUserQuery
{
    public string RoleId { get; set; } = string.Empty;
    public string? OrgId { get; set; }
    public string? StoreId { get; set; }
    public string? UserType { get; set; }
    public string? Filter { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// 角色使用者列表項目 (SYS0230)
/// </summary>
public class RoleUserListItem
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? OrgId { get; set; }
    public string? OrgName { get; set; }
    public bool IsAssigned { get; set; }
}

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

    /// <summary>
    /// 根據角色編號查詢使用者列表（分頁，SYS0230）
    /// </summary>
    Task<PagedResult<RoleUserListItem>> GetRoleUsersAsync(RoleUserQuery query);

    /// <summary>
    /// 根據角色編號查詢使用者列表
    /// </summary>
    Task<IEnumerable<UserRole>> GetByRoleIdAsync(string roleId);

    /// <summary>
    /// 根據角色ID刪除所有使用者分配 (SYS0240)
    /// </summary>
    Task DeleteByRoleIdAsync(string roleId, System.Data.IDbTransaction? transaction = null);

    /// <summary>
    /// 從來源角色複製使用者分配到目的角色 (SYS0240)
    /// </summary>
    Task<int> CopyFromRoleAsync(string sourceRoleId, string targetRoleId, string createdBy, System.Data.IDbTransaction? transaction = null);
}

