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

    public RolesController(
        IRoleService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
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
}

