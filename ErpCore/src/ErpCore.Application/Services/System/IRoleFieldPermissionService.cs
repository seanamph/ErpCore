using ErpCore.Application.DTOs.System;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 角色欄位權限服務介面 (SYS0330)
/// </summary>
public interface IRoleFieldPermissionService
{
    /// <summary>
    /// 查詢資料庫列表
    /// </summary>
    Task<List<DatabaseDto>> GetDatabasesAsync();

    /// <summary>
    /// 查詢表格列表
    /// </summary>
    Task<List<TableDto>> GetTablesAsync(string dbName);

    /// <summary>
    /// 查詢欄位列表
    /// </summary>
    Task<List<FieldDto>> GetFieldsAsync(string dbName, string tableName);

    /// <summary>
    /// 查詢角色欄位權限列表
    /// </summary>
    Task<PagedResult<RoleFieldPermissionDto>> GetPermissionsAsync(RoleFieldPermissionQueryDto query);

    /// <summary>
    /// 新增角色欄位權限
    /// </summary>
    Task<Guid> CreatePermissionAsync(CreateRoleFieldPermissionDto dto);

    /// <summary>
    /// 修改角色欄位權限
    /// </summary>
    Task<bool> UpdatePermissionAsync(Guid id, UpdateRoleFieldPermissionDto dto);

    /// <summary>
    /// 刪除角色欄位權限
    /// </summary>
    Task<bool> DeletePermissionAsync(Guid id);

    /// <summary>
    /// 批次設定角色欄位權限
    /// </summary>
    Task<int> BatchSetPermissionsAsync(BatchSetRoleFieldPermissionDto dto);
}

