using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.System;

/// <summary>
/// 角色系統權限設定控制器 (SYS0310)
/// </summary>
[Route("api/v1/roles/{roleId}/permissions")]
public class RolePermissionsController : BaseController
{
    private readonly IRolePermissionService _service;

    public RolePermissionsController(
        IRolePermissionService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢角色權限列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<RolePermissionDto>>>> GetRolePermissions(
        string roleId,
        [FromQuery] RolePermissionQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetRolePermissionsAsync(roleId, query);
            return result;
        }, $"查詢角色權限列表失敗: {roleId}");
    }

    /// <summary>
    /// 查詢角色系統權限統計
    /// </summary>
    [HttpGet("systems/stats")]
    public async Task<ActionResult<ApiResponse<List<RolePermissionStatsDto>>>> GetSystemStats(string roleId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSystemStatsAsync(roleId);
            return result;
        }, $"查詢角色權限統計失敗: {roleId}");
    }

    /// <summary>
    /// 新增角色權限
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<BatchOperationResult>>> CreateRolePermissions(
        string roleId,
        [FromBody] CreateRolePermissionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateRolePermissionsAsync(roleId, dto);
            return result;
        }, $"新增角色權限失敗: {roleId}");
    }

    /// <summary>
    /// 批量設定角色系統權限
    /// </summary>
    [HttpPost("systems")]
    public async Task<ActionResult<ApiResponse<BatchOperationResult>>> BatchSetSystemPermissions(
        string roleId,
        [FromBody] BatchSetRoleSystemPermissionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.BatchSetSystemPermissionsAsync(roleId, dto);
            return result;
        }, $"批量設定角色系統權限失敗: {roleId}");
    }

    /// <summary>
    /// 批量設定角色選單權限
    /// </summary>
    [HttpPost("menus")]
    public async Task<ActionResult<ApiResponse<BatchOperationResult>>> BatchSetMenuPermissions(
        string roleId,
        [FromBody] BatchSetRoleMenuPermissionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.BatchSetMenuPermissionsAsync(roleId, dto);
            return result;
        }, $"批量設定角色選單權限失敗: {roleId}");
    }

    /// <summary>
    /// 批量設定角色作業權限
    /// </summary>
    [HttpPost("programs")]
    public async Task<ActionResult<ApiResponse<BatchOperationResult>>> BatchSetProgramPermissions(
        string roleId,
        [FromBody] BatchSetRoleProgramPermissionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.BatchSetProgramPermissionsAsync(roleId, dto);
            return result;
        }, $"批量設定角色作業權限失敗: {roleId}");
    }

    /// <summary>
    /// 批量設定角色按鈕權限
    /// </summary>
    [HttpPost("buttons")]
    public async Task<ActionResult<ApiResponse<BatchOperationResult>>> BatchSetButtonPermissions(
        string roleId,
        [FromBody] BatchSetRoleButtonPermissionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.BatchSetButtonPermissionsAsync(roleId, dto);
            return result;
        }, $"批量設定角色按鈕權限失敗: {roleId}");
    }

    /// <summary>
    /// 刪除角色權限
    /// </summary>
    [HttpDelete("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteRolePermission(
        string roleId,
        long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteRolePermissionAsync(roleId, tKey);
        }, $"刪除角色權限失敗: {roleId} - {tKey}");
    }

    /// <summary>
    /// 批量刪除角色權限
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ActionResult<ApiResponse<BatchOperationResult>>> BatchDeleteRolePermissions(
        string roleId,
        [FromBody] BatchDeleteRolePermissionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.BatchDeleteRolePermissionsAsync(roleId, dto);
            return result;
        }, $"批量刪除角色權限失敗: {roleId}");
    }
}

