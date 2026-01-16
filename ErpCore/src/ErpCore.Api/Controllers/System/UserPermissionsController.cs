using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.System;

/// <summary>
/// 使用者系統權限設定控制器 (SYS0320)
/// </summary>
[Route("api/v1/users/{userId}/permissions")]
public class UserPermissionsController : BaseController
{
    private readonly IUserPermissionService _service;

    public UserPermissionsController(
        IUserPermissionService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢使用者權限列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<UserPermissionDto>>>> GetUserPermissions(
        string userId,
        [FromQuery] UserPermissionQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetUserPermissionsAsync(userId, query);
            return result;
        }, $"查詢使用者權限列表失敗: {userId}");
    }

    /// <summary>
    /// 查詢使用者系統權限統計
    /// </summary>
    [HttpGet("systems/stats")]
    public async Task<ActionResult<ApiResponse<List<UserPermissionStatsDto>>>> GetSystemStats(string userId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSystemStatsAsync(userId);
            return result;
        }, $"查詢使用者權限統計失敗: {userId}");
    }

    /// <summary>
    /// 新增使用者權限
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<BatchOperationResult>>> CreateUserPermissions(
        string userId,
        [FromBody] CreateUserPermissionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateUserPermissionsAsync(userId, dto);
            return result;
        }, $"新增使用者權限失敗: {userId}");
    }

    /// <summary>
    /// 批量設定使用者系統權限
    /// </summary>
    [HttpPost("systems")]
    public async Task<ActionResult<ApiResponse<BatchOperationResult>>> BatchSetSystemPermissions(
        string userId,
        [FromBody] BatchSetUserSystemPermissionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.BatchSetSystemPermissionsAsync(userId, dto);
            return result;
        }, $"批量設定使用者系統權限失敗: {userId}");
    }

    /// <summary>
    /// 批量設定使用者選單權限
    /// </summary>
    [HttpPost("menus")]
    public async Task<ActionResult<ApiResponse<BatchOperationResult>>> BatchSetMenuPermissions(
        string userId,
        [FromBody] BatchSetUserMenuPermissionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.BatchSetMenuPermissionsAsync(userId, dto);
            return result;
        }, $"批量設定使用者選單權限失敗: {userId}");
    }

    /// <summary>
    /// 批量設定使用者作業權限
    /// </summary>
    [HttpPost("programs")]
    public async Task<ActionResult<ApiResponse<BatchOperationResult>>> BatchSetProgramPermissions(
        string userId,
        [FromBody] BatchSetUserProgramPermissionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.BatchSetProgramPermissionsAsync(userId, dto);
            return result;
        }, $"批量設定使用者作業權限失敗: {userId}");
    }

    /// <summary>
    /// 批量設定使用者按鈕權限
    /// </summary>
    [HttpPost("buttons")]
    public async Task<ActionResult<ApiResponse<BatchOperationResult>>> BatchSetButtonPermissions(
        string userId,
        [FromBody] BatchSetUserButtonPermissionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.BatchSetButtonPermissionsAsync(userId, dto);
            return result;
        }, $"批量設定使用者按鈕權限失敗: {userId}");
    }

    /// <summary>
    /// 刪除使用者權限
    /// </summary>
    [HttpDelete("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteUserPermission(
        string userId,
        long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteUserPermissionAsync(userId, tKey);
        }, $"刪除使用者權限失敗: {userId} - {tKey}");
    }

    /// <summary>
    /// 批量刪除使用者權限
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ActionResult<ApiResponse<BatchOperationResult>>> BatchDeleteUserPermissions(
        string userId,
        [FromBody] BatchDeleteUserPermissionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.BatchDeleteUserPermissionsAsync(userId, dto);
            return result;
        }, $"批量刪除使用者權限失敗: {userId}");
    }

    /// <summary>
    /// 查詢使用者系統列表 (SYS0320)
    /// </summary>
    [HttpGet("systems")]
    public async Task<ActionResult<ApiResponse<List<UserSystemListDto>>>> GetUserSystems(string userId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetUserSystemsAsync(userId);
            return result;
        }, $"查詢使用者系統列表失敗: {userId}");
    }

    /// <summary>
    /// 查詢使用者選單列表 (SYS0320)
    /// </summary>
    [HttpGet("menus")]
    public async Task<ActionResult<ApiResponse<List<UserMenuListDto>>>> GetUserMenus(
        string userId,
        [FromQuery] string? systemId = null)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetUserMenusAsync(userId, systemId);
            return result;
        }, $"查詢使用者選單列表失敗: {userId}");
    }

    /// <summary>
    /// 查詢使用者作業列表 (SYS0320)
    /// </summary>
    [HttpGet("programs")]
    public async Task<ActionResult<ApiResponse<List<UserProgramListDto>>>> GetUserPrograms(
        string userId,
        [FromQuery] string? menuId = null)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetUserProgramsAsync(userId, menuId);
            return result;
        }, $"查詢使用者作業列表失敗: {userId}");
    }

    /// <summary>
    /// 查詢使用者按鈕列表 (SYS0320)
    /// </summary>
    [HttpGet("buttons")]
    public async Task<ActionResult<ApiResponse<List<UserButtonListDto>>>> GetUserButtons(
        string userId,
        [FromQuery] string? programId = null)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetUserButtonsAsync(userId, programId);
            return result;
        }, $"查詢使用者按鈕列表失敗: {userId}");
    }

    /// <summary>
    /// 修改使用者權限
    /// </summary>
    [HttpPut("{tKey}")]
    public async Task<ActionResult<ApiResponse<UserPermissionDto>>> UpdateUserPermission(
        string userId,
        long tKey,
        [FromBody] UpdateUserPermissionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.UpdateUserPermissionAsync(userId, tKey, dto);
            return result;
        }, $"修改使用者權限失敗: {userId} - {tKey}");
    }
}

