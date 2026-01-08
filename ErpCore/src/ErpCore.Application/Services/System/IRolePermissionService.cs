using ErpCore.Application.DTOs.System;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 角色權限服務介面 (SYS0310)
/// </summary>
public interface IRolePermissionService
{
    /// <summary>
    /// 查詢角色權限列表
    /// </summary>
    Task<PagedResult<RolePermissionDto>> GetRolePermissionsAsync(string roleId, RolePermissionQueryDto query);

    /// <summary>
    /// 查詢角色系統權限統計
    /// </summary>
    Task<List<RolePermissionStatsDto>> GetSystemStatsAsync(string roleId);

    /// <summary>
    /// 新增角色權限
    /// </summary>
    Task<BatchOperationResult> CreateRolePermissionsAsync(string roleId, CreateRolePermissionDto dto);

    /// <summary>
    /// 批量設定角色系統權限
    /// </summary>
    Task<BatchOperationResult> BatchSetSystemPermissionsAsync(string roleId, BatchSetRoleSystemPermissionDto dto);

    /// <summary>
    /// 批量設定角色選單權限
    /// </summary>
    Task<BatchOperationResult> BatchSetMenuPermissionsAsync(string roleId, BatchSetRoleMenuPermissionDto dto);

    /// <summary>
    /// 批量設定角色作業權限
    /// </summary>
    Task<BatchOperationResult> BatchSetProgramPermissionsAsync(string roleId, BatchSetRoleProgramPermissionDto dto);

    /// <summary>
    /// 批量設定角色按鈕權限
    /// </summary>
    Task<BatchOperationResult> BatchSetButtonPermissionsAsync(string roleId, BatchSetRoleButtonPermissionDto dto);

    /// <summary>
    /// 刪除角色權限
    /// </summary>
    Task DeleteRolePermissionAsync(string roleId, long tKey);

    /// <summary>
    /// 批量刪除角色權限
    /// </summary>
    Task<BatchOperationResult> BatchDeleteRolePermissionsAsync(string roleId, BatchDeleteRolePermissionDto dto);
}

/// <summary>
/// 批量操作結果
/// </summary>
public class BatchOperationResult
{
    public int UpdatedCount { get; set; }
    public int SkippedCount { get; set; }
    public int ClearedRoleCount { get; set; }
}

