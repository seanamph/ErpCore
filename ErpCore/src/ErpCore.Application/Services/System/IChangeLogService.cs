using ErpCore.Application.DTOs.System;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 異動記錄服務介面 (SYS0610, SYS0620)
/// </summary>
public interface IChangeLogService
{
    /// <summary>
    /// 查詢使用者異動記錄 (SYS0610)
    /// </summary>
    Task<PagedResult<ChangeLogDto>> GetUserChangeLogsAsync(UserChangeLogQueryDto query);

    /// <summary>
    /// 查詢角色異動記錄 (SYS0620)
    /// </summary>
    Task<PagedResult<ChangeLogDto>> GetRoleChangeLogsAsync(RoleChangeLogQueryDto query);

    /// <summary>
    /// 查詢單筆異動記錄
    /// </summary>
    Task<ChangeLogDto?> GetChangeLogByIdAsync(long logId);

    /// <summary>
    /// 查詢使用者角色對應設定異動記錄 (SYS0630)
    /// </summary>
    Task<PagedResult<ChangeLogDto>> GetUserRoleChangeLogsAsync(UserRoleChangeLogQueryDto query);

    /// <summary>
    /// 查詢系統權限異動記錄 (SYS0640)
    /// </summary>
    Task<PagedResult<ChangeLogDto>> GetSystemPermissionChangeLogsAsync(SystemPermissionChangeLogQueryDto query);

    /// <summary>
    /// 查詢可管控欄位異動記錄 (SYS0650)
    /// </summary>
    Task<PagedResult<ChangeLogDto>> GetControllableFieldChangeLogsAsync(ControllableFieldChangeLogQueryDto query);

    /// <summary>
    /// 查詢其他異動記錄 (SYS0660)
    /// </summary>
    Task<PagedResult<ChangeLogDto>> GetOtherChangeLogsAsync(OtherChangeLogQueryDto query);
}

