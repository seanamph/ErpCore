using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.System;

/// <summary>
/// 角色基本資料維護作業控制器 (SYS0210)
/// </summary>
[Route("api/v1/roles")]
public class RolesController : BaseController
{
    private readonly IRoleService _service;
    private readonly IRoleUserAssignmentService _roleUserAssignmentService;

    public RolesController(
        IRoleService service,
        IRoleUserAssignmentService roleUserAssignmentService,
        ILoggerService logger) : base(logger)
    {
        _service = service;
        _roleUserAssignmentService = roleUserAssignmentService;
    }

    /// <summary>
    /// 查詢角色列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<RoleDto>>>> GetRoles(
        [FromQuery] RoleQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetRolesAsync(query);
            return result;
        }, "查詢角色列表失敗");
    }

    /// <summary>
    /// 查詢單筆角色
    /// </summary>
    [HttpGet("{roleId}")]
    public async Task<ActionResult<ApiResponse<RoleDto>>> GetRole(string roleId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetRoleByIdAsync(roleId);
            return result;
        }, $"查詢角色失敗: {roleId}");
    }

    /// <summary>
    /// 新增角色
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateRole(
        [FromBody] CreateRoleDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateRoleAsync(dto);
            return result;
        }, "新增角色失敗");
    }

    /// <summary>
    /// 修改角色
    /// </summary>
    [HttpPut("{roleId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateRole(
        string roleId,
        [FromBody] UpdateRoleDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateRoleAsync(roleId, dto);
        }, $"修改角色失敗: {roleId}");
    }

    /// <summary>
    /// 刪除角色
    /// </summary>
    [HttpDelete("{roleId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteRole(string roleId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteRoleAsync(roleId);
        }, $"刪除角色失敗: {roleId}");
    }

    /// <summary>
    /// 批次刪除角色
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteRolesBatch(
        [FromBody] BatchDeleteRoleDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteRolesBatchAsync(dto);
        }, "批次刪除角色失敗");
    }

    /// <summary>
    /// 複製角色
    /// </summary>
    [HttpPost("{roleId}/copy")]
    public async Task<ActionResult<ApiResponse<string>>> CopyRole(
        string roleId,
        [FromBody] CopyRoleDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CopyRoleAsync(roleId, dto);
            return result;
        }, $"複製角色失敗: {roleId}");
    }

    // ========== SYS0230 - 角色之使用者設定維護 ==========

    /// <summary>
    /// 查詢角色使用者列表 (SYS0230)
    /// </summary>
    [HttpGet("{roleId}/users")]
    public async Task<ActionResult<ApiResponse<PagedResult<RoleUserListItemDto>>>> GetRoleUsers(
        string roleId,
        [FromQuery] RoleUserQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            query.RoleId = roleId;
            var result = await _roleUserAssignmentService.GetRoleUsersAsync(query);
            return result;
        }, $"查詢角色使用者列表失敗: {roleId}");
    }

    /// <summary>
    /// 批量設定角色使用者 (SYS0230)
    /// </summary>
    [HttpPost("{roleId}/users/assign")]
    public async Task<ActionResult<ApiResponse<BatchAssignRoleUsersResultDto>>> BatchAssignRoleUsers(
        string roleId,
        [FromBody] BatchAssignRoleUsersDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _roleUserAssignmentService.BatchAssignRoleUsersAsync(roleId, dto);
            return result;
        }, $"批量設定角色使用者失敗: {roleId}");
    }

    /// <summary>
    /// 新增角色使用者 (SYS0230)
    /// </summary>
    [HttpPost("{roleId}/users")]
    public async Task<ActionResult<ApiResponse<object>>> AssignUserToRole(
        string roleId,
        [FromBody] AssignUserToRoleDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _roleUserAssignmentService.AssignUserToRoleAsync(roleId, dto.UserId);
        }, $"新增角色使用者失敗: {roleId}");
    }

    /// <summary>
    /// 移除角色使用者 (SYS0230)
    /// </summary>
    [HttpDelete("{roleId}/users/{userId}")]
    public async Task<ActionResult<ApiResponse<object>>> RemoveUserFromRole(
        string roleId,
        string userId)
    {
        return await ExecuteAsync(async () =>
        {
            await _roleUserAssignmentService.RemoveUserFromRoleAsync(roleId, userId);
        }, $"移除角色使用者失敗: {roleId}, {userId}");
    }

    // ========== SYS0240 - 角色複製 ==========

    /// <summary>
    /// 複製角色 (SYS0240)
    /// </summary>
    [HttpPost("copy")]
    public async Task<ActionResult<ApiResponse<CopyRoleResultDto>>> CopyRoleToTarget(
        [FromBody] CopyRoleRequestDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CopyRoleToTargetAsync(dto);
            return result;
        }, "複製角色失敗");
    }

    /// <summary>
    /// 驗證角色複製 (SYS0240)
    /// </summary>
    [HttpPost("copy/validate")]
    public async Task<ActionResult<ApiResponse<ValidateCopyResultDto>>> ValidateCopyRole(
        [FromBody] ValidateCopyRequestDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.ValidateCopyRoleAsync(dto);
            return result;
        }, "驗證角色複製失敗");
    }
}

/// <summary>
/// 分配使用者到角色請求 DTO (SYS0230)
/// </summary>
public class AssignUserToRoleDto
{
    public string UserId { get; set; } = string.Empty;
}

