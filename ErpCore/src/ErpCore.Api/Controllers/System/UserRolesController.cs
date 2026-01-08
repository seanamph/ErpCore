using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.System;

/// <summary>
/// 使用者之角色設定維護作業控制器 (SYS0220)
/// </summary>
[Route("api/v1/users/{userId}/roles")]
public class UserRolesController : BaseController
{
    private readonly IUserRoleService _service;

    public UserRolesController(
        IUserRoleService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢使用者已分配的角色列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<UserRoleDto>>>> GetUserRoles(
        string userId,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetUserRolesAsync(userId, pageIndex, pageSize);
            return result;
        }, $"查詢使用者角色列表失敗: {userId}");
    }

    /// <summary>
    /// 查詢所有可用角色列表（排除已分配的角色）
    /// </summary>
    [HttpGet("available")]
    public async Task<ActionResult<ApiResponse<PagedResult<RoleDto>>>> GetAvailableRoles(
        string userId,
        [FromQuery] string? keyword = null,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetAvailableRolesAsync(userId, keyword, pageIndex, pageSize);
            return result;
        }, $"查詢可用角色列表失敗: {userId}");
    }

    /// <summary>
    /// 為使用者分配角色（新增）
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<AssignRoleResultDto>>> AssignRoles(
        string userId,
        [FromBody] AssignRolesDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.AssignRolesAsync(userId, dto.RoleIds);
            return result;
        }, $"分配角色失敗: {userId}");
    }

    /// <summary>
    /// 移除使用者的角色（刪除）
    /// </summary>
    [HttpDelete]
    public async Task<ActionResult<ApiResponse<RemoveRoleResultDto>>> RemoveRoles(
        string userId,
        [FromBody] RemoveRolesDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.RemoveRolesAsync(userId, dto.RoleIds);
            return result;
        }, $"移除角色失敗: {userId}");
    }

    /// <summary>
    /// 批量更新使用者角色
    /// </summary>
    [HttpPut("batch")]
    public async Task<ActionResult<ApiResponse<BatchUpdateRoleResultDto>>> BatchUpdateRoles(
        string userId,
        [FromBody] BatchUpdateRolesDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.BatchUpdateRolesAsync(userId, dto.RoleIds);
            return result;
        }, $"批量更新角色失敗: {userId}");
    }
}

