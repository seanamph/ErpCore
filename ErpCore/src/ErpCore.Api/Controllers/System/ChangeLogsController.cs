using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.System;

/// <summary>
/// 異動記錄控制器 (SYS0610, SYS0620)
/// </summary>
[Route("api/v1/change-logs")]
public class ChangeLogsController : BaseController
{
    private readonly IChangeLogService _service;

    public ChangeLogsController(
        IChangeLogService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢使用者異動記錄
    /// </summary>
    [HttpPost("users/search")]
    public async Task<ActionResult<ApiResponse<PagedResult<ChangeLogDto>>>> GetUserChangeLogs(
        [FromBody] UserChangeLogQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetUserChangeLogsAsync(query);
            return result;
        }, "查詢使用者異動記錄失敗");
    }

    /// <summary>
    /// 查詢角色異動記錄 (SYS0620)
    /// </summary>
    [HttpPost("roles/search")]
    public async Task<ActionResult<ApiResponse<PagedResult<ChangeLogDto>>>> GetRoleChangeLogs(
        [FromBody] RoleChangeLogQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetRoleChangeLogsAsync(query);
            return result;
        }, "查詢角色異動記錄失敗");
    }

    /// <summary>
    /// 查詢單筆異動記錄
    /// </summary>
    [HttpGet("{logId}")]
    public async Task<ActionResult<ApiResponse<ChangeLogDto>>> GetChangeLogById(long logId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetChangeLogByIdAsync(logId);
            if (result == null)
            {
                throw new InvalidOperationException($"異動記錄不存在: {logId}");
            }
            return result;
        }, $"查詢異動記錄失敗: {logId}");
    }

    /// <summary>
    /// 查詢使用者角色對應設定異動記錄 (SYS0630)
    /// </summary>
    [HttpPost("user-roles/search")]
    public async Task<ActionResult<ApiResponse<PagedResult<ChangeLogDto>>>> GetUserRoleChangeLogs(
        [FromBody] UserRoleChangeLogQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetUserRoleChangeLogsAsync(query);
            return result;
        }, "查詢使用者角色對應設定異動記錄失敗");
    }

    /// <summary>
    /// 查詢系統權限異動記錄 (SYS0640)
    /// </summary>
    [HttpPost("system-permissions/search")]
    public async Task<ActionResult<ApiResponse<PagedResult<ChangeLogDto>>>> GetSystemPermissionChangeLogs(
        [FromBody] SystemPermissionChangeLogQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSystemPermissionChangeLogsAsync(query);
            return result;
        }, "查詢系統權限異動記錄失敗");
    }

    /// <summary>
    /// 查詢可管控欄位異動記錄 (SYS0650)
    /// </summary>
    [HttpPost("controllable-fields/search")]
    public async Task<ActionResult<ApiResponse<PagedResult<ChangeLogDto>>>> GetControllableFieldChangeLogs(
        [FromBody] ControllableFieldChangeLogQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetControllableFieldChangeLogsAsync(query);
            return result;
        }, "查詢可管控欄位異動記錄失敗");
    }

    /// <summary>
    /// 查詢其他異動記錄 (SYS0660)
    /// </summary>
    [HttpPost("others/search")]
    public async Task<ActionResult<ApiResponse<PagedResult<ChangeLogDto>>>> GetOtherChangeLogs(
        [FromBody] OtherChangeLogQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetOtherChangeLogsAsync(query);
            return result;
        }, "查詢其他異動記錄失敗");
    }
}

