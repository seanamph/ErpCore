using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.System;

/// <summary>
/// 作業權限之角色列表控制器 (SYS0740)
/// </summary>
[Route("api/v1/program-role-permissions")]
public class ProgramRolePermissionController : BaseController
{
    private readonly IProgramRolePermissionService _service;

    public ProgramRolePermissionController(
        IProgramRolePermissionService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢作業權限之角色列表
    /// </summary>
    [HttpGet("list")]
    public async Task<ActionResult<ApiResponse<ProgramRolePermissionListResponseDto>>> GetProgramRolePermissionList(
        [FromQuery] ProgramRolePermissionListRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetProgramRolePermissionListAsync(request);
            return result;
        }, "查詢作業權限之角色列表失敗");
    }

    /// <summary>
    /// 匯出作業權限之角色報表
    /// </summary>
    [HttpPost("export")]
    public async Task<IActionResult> ExportProgramRolePermissionReport(
        [FromBody] ProgramRolePermissionExportRequestDto request)
    {
        try
        {
            var fileBytes = await _service.ExportProgramRolePermissionReportAsync(
                request.Request,
                request.ExportFormat);

            var contentType = request.ExportFormat == "Excel"
                ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                : "application/pdf";

            var fileName = $"作業權限之角色列表_{request.Request.ProgramId}_{DateTime.Now:yyyyMMddHHmmss}.{(request.ExportFormat == "Excel" ? "xlsx" : "pdf")}";

            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError($"匯出作業權限之角色報表失敗: {ex.Message}", ex);
            return BadRequest(ApiResponse<object>.Fail($"匯出作業權限之角色報表失敗: {ex.Message}"));
        }
    }
}
