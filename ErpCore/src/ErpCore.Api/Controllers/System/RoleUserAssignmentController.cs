using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.System;

/// <summary>
/// 角色之使用者設定維護作業控制器 (SYS0230)
/// </summary>
[Route("api/v1/roles/{roleId}/users")]
public class RoleUserAssignmentController : BaseController
{
    private readonly IRoleUserAssignmentService _service;

    public RoleUserAssignmentController(
        IRoleUserAssignmentService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢角色使用者列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<RoleUserListItemDto>>>> GetRoleUsers(
        string roleId,
        [FromQuery] RoleUserQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            query.RoleId = roleId;
            var result = await _service.GetRoleUsersAsync(query);
            return result;
        }, $"查詢角色使用者列表失敗: {roleId}");
    }

    /// <summary>
    /// 查詢可用使用者列表（排除已分配的使用者）
    /// </summary>
    [HttpGet("available")]
    public async Task<ActionResult<ApiResponse<PagedResult<RoleUserListItemDto>>>> GetAvailableUsers(
        string roleId,
        [FromQuery] RoleUserQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            query.RoleId = roleId;
            var result = await _service.GetRoleUsersAsync(query);
            // 只返回未分配的使用者
            result.Items = result.Items.Where(x => !x.IsAssigned).ToList();
            result.TotalCount = result.Items.Count;
            return result;
        }, $"查詢可用使用者列表失敗: {roleId}");
    }

    /// <summary>
    /// 批量設定角色使用者
    /// </summary>
    [HttpPost("assign")]
    public async Task<ActionResult<ApiResponse<BatchAssignRoleUsersResultDto>>> BatchAssignRoleUsers(
        string roleId,
        [FromBody] BatchAssignRoleUsersDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.BatchAssignRoleUsersAsync(roleId, dto);
            return result;
        }, $"批量設定角色使用者失敗: {roleId}");
    }

    /// <summary>
    /// 新增角色使用者
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<object>>> AssignUserToRole(
        string roleId,
        [FromBody] AssignUserToRoleDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.AssignUserToRoleAsync(roleId, dto.UserId);
            return (object?)null;
        }, $"新增角色使用者失敗: {roleId}, {dto.UserId}");
    }

    /// <summary>
    /// 移除角色使用者
    /// </summary>
    [HttpDelete("{userId}")]
    public async Task<ActionResult<ApiResponse<object>>> RemoveUserFromRole(
        string roleId,
        string userId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.RemoveUserFromRoleAsync(roleId, userId);
            return (object?)null;
        }, $"移除角色使用者失敗: {roleId}, {userId}");
    }
}
