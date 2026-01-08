using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.System;

/// <summary>
/// 角色系統權限列表控制器 (SYS0731)
/// </summary>
[Route("api/v1/role-system-permissions")]
public class RoleSystemPermissionController : BaseController
{
    private readonly IRoleSystemPermissionService _service;

    public RoleSystemPermissionController(
        IRoleSystemPermissionService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢角色系統權限列表
    /// </summary>
    [HttpGet("list")]
    public async Task<ActionResult<ApiResponse<RoleSystemPermissionListResponseDto>>> GetRoleSystemPermissionList(
        [FromQuery] RoleSystemPermissionListRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetRoleSystemPermissionListAsync(request);
            return result;
        }, "查詢角色系統權限列表失敗");
    }

    /// <summary>
    /// 匯出角色系統權限報表
    /// </summary>
    [HttpPost("export")]
    public async Task<IActionResult> ExportRoleSystemPermissionReport(
        [FromBody] RoleSystemPermissionExportRequestDto request)
    {
        try
        {
            var fileBytes = await _service.ExportRoleSystemPermissionReportAsync(
                request.Request,
                request.ExportFormat);

            var contentType = request.ExportFormat == "Excel"
                ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                : "application/pdf";

            var fileName = $"角色系統權限列表_{request.Request.RoleId}_{DateTime.Now:yyyyMMddHHmmss}.{(request.ExportFormat == "Excel" ? "xlsx" : "pdf")}";

            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError($"匯出角色系統權限報表失敗: {ex.Message}", ex);
            return BadRequest(ApiResponse<object>.Fail($"匯出角色系統權限報表失敗: {ex.Message}"));
        }
    }
}

