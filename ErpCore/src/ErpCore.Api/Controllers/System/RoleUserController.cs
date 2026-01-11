using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.System;

/// <summary>
/// 角色之使用者列表控制器 (SYS0750)
/// </summary>
[Route("api/v1/role-users")]
public class RoleUserController : BaseController
{
    private readonly IRoleUserService _service;

    public RoleUserController(
        IRoleUserService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢角色之使用者列表
    /// </summary>
    [HttpGet("list")]
    public async Task<ActionResult<ApiResponse<RoleUserListResponseDto>>> GetRoleUserList(
        [FromQuery] RoleUserListRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetRoleUserListAsync(request);
            return result;
        }, "查詢角色之使用者列表失敗");
    }

    /// <summary>
    /// 刪除使用者角色對應
    /// </summary>
    [HttpDelete("{roleId}/{userId}")]
    public async Task<ActionResult<ApiResponse>> DeleteRoleUser(
        string roleId,
        string userId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteRoleUserAsync(roleId, userId);
            return (object?)null;
        }, "刪除使用者角色對應失敗");
    }

    /// <summary>
    /// 批次刪除使用者角色對應
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ActionResult<ApiResponse>> BatchDeleteRoleUsers(
        [FromBody] BatchDeleteRoleUsersRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.BatchDeleteRoleUsersAsync(
                request.RoleId,
                request.UserIds);
            return (object?)null;
        }, "批次刪除使用者角色對應失敗");
    }

    /// <summary>
    /// 匯出角色之使用者報表
    /// </summary>
    [HttpPost("export")]
    public async Task<IActionResult> ExportRoleUserReport(
        [FromBody] RoleUserExportRequestDto request)
    {
        try
        {
            var fileBytes = await _service.ExportRoleUserReportAsync(
                request.Request,
                request.ExportFormat);

            var contentType = request.ExportFormat == "Excel"
                ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                : "application/pdf";

            var fileName = $"角色之使用者列表_{request.Request.RoleId}_{DateTime.Now:yyyyMMddHHmmss}.{(request.ExportFormat == "Excel" ? "xlsx" : "pdf")}";

            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError($"匯出角色之使用者報表失敗: {ex.Message}", ex);
            return BadRequest(ApiResponse<object>.Fail($"匯出角色之使用者報表失敗: {ex.Message}"));
        }
    }
}
