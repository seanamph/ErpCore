using ErpCore.Domain.Entities.System;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 角色欄位權限 Repository 介面 (SYS0330)
/// </summary>
public interface IRoleFieldPermissionRepository
{
    /// <summary>
    /// 查詢角色欄位權限列表
    /// </summary>
    Task<PagedResult<RoleFieldPermission>> QueryAsync(RoleFieldPermissionQuery query);

    /// <summary>
    /// 根據 ID 查詢
    /// </summary>
    Task<RoleFieldPermission?> GetByIdAsync(Guid id);

    /// <summary>
    /// 新增
    /// </summary>
    Task<Guid> CreateAsync(RoleFieldPermission entity);

    /// <summary>
    /// 修改
    /// </summary>
    Task<bool> UpdateAsync(RoleFieldPermission entity);

    /// <summary>
    /// 刪除
    /// </summary>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// 批次新增
    /// </summary>
    Task<int> BatchCreateAsync(List<RoleFieldPermission> entities);

    /// <summary>
    /// 批次刪除
    /// </summary>
    Task<int> BatchDeleteAsync(List<Guid> ids);
}

/// <summary>
/// 角色欄位權限查詢條件
/// </summary>
public class RoleFieldPermissionQuery
{
    public string? RoleId { get; set; }
    public string? DbName { get; set; }
    public string? TableName { get; set; }
    public string? FieldName { get; set; }
    public string? PermissionType { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

