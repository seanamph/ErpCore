using ErpCore.Application.DTOs.System;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 使用者權限服務介面 (SYS0320)
/// </summary>
public interface IUserPermissionService
{
    /// <summary>
    /// 查詢使用者權限列表
    /// </summary>
    Task<PagedResult<UserPermissionDto>> GetUserPermissionsAsync(string userId, UserPermissionQueryDto query);

    /// <summary>
    /// 查詢使用者系統權限統計
    /// </summary>
    Task<List<UserPermissionStatsDto>> GetSystemStatsAsync(string userId);

    /// <summary>
    /// 新增使用者權限
    /// </summary>
    Task<BatchOperationResult> CreateUserPermissionsAsync(string userId, CreateUserPermissionDto dto);

    /// <summary>
    /// 批量設定使用者系統權限
    /// </summary>
    Task<BatchOperationResult> BatchSetSystemPermissionsAsync(string userId, BatchSetUserSystemPermissionDto dto);

    /// <summary>
    /// 批量設定使用者選單權限
    /// </summary>
    Task<BatchOperationResult> BatchSetMenuPermissionsAsync(string userId, BatchSetUserMenuPermissionDto dto);

    /// <summary>
    /// 批量設定使用者作業權限
    /// </summary>
    Task<BatchOperationResult> BatchSetProgramPermissionsAsync(string userId, BatchSetUserProgramPermissionDto dto);

    /// <summary>
    /// 批量設定使用者按鈕權限
    /// </summary>
    Task<BatchOperationResult> BatchSetButtonPermissionsAsync(string userId, BatchSetUserButtonPermissionDto dto);

    /// <summary>
    /// 刪除使用者權限
    /// </summary>
    Task DeleteUserPermissionAsync(string userId, long tKey);

    /// <summary>
    /// 批量刪除使用者權限
    /// </summary>
    Task<BatchOperationResult> BatchDeleteUserPermissionsAsync(string userId, BatchDeleteUserPermissionDto dto);
}

