using ErpCore.Application.DTOs.System;
using ErpCore.Domain.Entities.System;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 異動記錄 Repository 介面 (SYS0610)
/// </summary>
public interface IChangeLogRepository
{
    /// <summary>
    /// 查詢使用者異動記錄
    /// </summary>
    Task<PagedResult<ChangeLog>> GetUserChangeLogsAsync(
        string programId,
        string? changeUserId,
        string? targetUserId,
        DateTime beginDate,
        DateTime endDate,
        int pageIndex,
        int pageSize);

    /// <summary>
    /// 查詢單筆異動記錄
    /// </summary>
    Task<ChangeLog?> GetChangeLogByIdAsync(long logId);

    /// <summary>
    /// 查詢使用者角色對應設定異動記錄 (SYS0630)
    /// </summary>
    Task<PagedResult<ChangeLog>> GetUserRoleChangeLogsAsync(
        string? changeUserId,
        string? searchUserId,
        string? searchRoleId,
        DateTime beginDate,
        DateTime endDate,
        int pageIndex,
        int pageSize);

    /// <summary>
    /// 查詢系統權限異動記錄 (SYS0640)
    /// </summary>
    Task<PagedResult<ChangeLog>> GetSystemPermissionChangeLogsAsync(
        string? changeUserId,
        string? searchUserId,
        string? searchRoleId,
        DateTime beginDate,
        DateTime endDate,
        int pageIndex,
        int pageSize);

    /// <summary>
    /// 查詢可管控欄位異動記錄 (SYS0650)
    /// </summary>
    Task<PagedResult<ChangeLog>> GetControllableFieldChangeLogsAsync(
        string? changeUserId,
        string? searchUserId,
        string? fieldId,
        DateTime beginDate,
        DateTime endDate,
        int pageIndex,
        int pageSize);

    /// <summary>
    /// 查詢其他異動記錄 (SYS0660)
    /// </summary>
    Task<PagedResult<ChangeLog>> GetOtherChangeLogsAsync(
        string? changeUserId,
        string? programId,
        DateTime beginDate,
        DateTime endDate,
        int pageIndex,
        int pageSize);
}

