using ErpCore.Domain.Entities.System;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 使用者欄位權限 Repository 介面 (SYS0340)
/// </summary>
public interface IUserFieldPermissionRepository
{
    /// <summary>
    /// 查詢使用者欄位權限列表
    /// </summary>
    Task<PagedResult<UserFieldPermission>> QueryAsync(UserFieldPermissionQuery query);

    /// <summary>
    /// 根據 ID 查詢
    /// </summary>
    Task<UserFieldPermission?> GetByIdAsync(Guid id);

    /// <summary>
    /// 新增
    /// </summary>
    Task<Guid> CreateAsync(UserFieldPermission entity);

    /// <summary>
    /// 修改
    /// </summary>
    Task<bool> UpdateAsync(UserFieldPermission entity);

    /// <summary>
    /// 刪除
    /// </summary>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// 批次新增
    /// </summary>
    Task<int> BatchCreateAsync(List<UserFieldPermission> entities);

    /// <summary>
    /// 批次刪除
    /// </summary>
    Task<int> BatchDeleteAsync(List<Guid> ids);
}

/// <summary>
/// 使用者欄位權限查詢條件
/// </summary>
public class UserFieldPermissionQuery
{
    public string? UserId { get; set; }
    public string? DbName { get; set; }
    public string? TableName { get; set; }
    public string? FieldName { get; set; }
    public string? PermissionType { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

