using ErpCore.Domain.Entities.System;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 角色 Repository 介面 (SYS0210)
/// </summary>
public interface IRoleRepository
{
    /// <summary>
    /// 根據角色代碼查詢
    /// </summary>
    Task<Role?> GetByIdAsync(string roleId);

    /// <summary>
    /// 查詢角色列表（分頁）
    /// </summary>
    Task<PagedResult<Role>> QueryAsync(RoleQuery query);

    /// <summary>
    /// 新增角色
    /// </summary>
    Task<Role> CreateAsync(Role role);

    /// <summary>
    /// 修改角色
    /// </summary>
    Task<Role> UpdateAsync(Role role);

    /// <summary>
    /// 刪除角色
    /// </summary>
    Task DeleteAsync(string roleId);

    /// <summary>
    /// 檢查角色是否存在
    /// </summary>
    Task<bool> ExistsAsync(string roleId);

    /// <summary>
    /// 檢查是否有使用者使用此角色
    /// </summary>
    Task<bool> HasUsersAsync(string roleId);
}

/// <summary>
/// 角色查詢條件
/// </summary>
public class RoleQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? RoleId { get; set; }
    public string? RoleName { get; set; }
}
