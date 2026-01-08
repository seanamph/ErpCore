using ErpCore.Application.DTOs.System;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 使用者欄位權限服務介面 (SYS0340)
/// </summary>
public interface IUserFieldPermissionService
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
    /// 查詢使用者欄位權限列表
    /// </summary>
    Task<PagedResult<UserFieldPermissionDto>> GetPermissionsAsync(UserFieldPermissionQueryDto query);

    /// <summary>
    /// 新增使用者欄位權限
    /// </summary>
    Task<Guid> CreatePermissionAsync(CreateUserFieldPermissionDto dto);

    /// <summary>
    /// 修改使用者欄位權限
    /// </summary>
    Task<bool> UpdatePermissionAsync(Guid id, UpdateUserFieldPermissionDto dto);

    /// <summary>
    /// 刪除使用者欄位權限
    /// </summary>
    Task<bool> DeletePermissionAsync(Guid id);

    /// <summary>
    /// 批次設定使用者欄位權限
    /// </summary>
    Task<int> BatchSetPermissionsAsync(BatchSetUserFieldPermissionDto dto);
}

