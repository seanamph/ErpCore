using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.System;

/// <summary>
/// 系統權限列表控制器 (SYS0710)
/// </summary>
[Route("api/v1/system-permissions")]
public class SystemPermissionsController : BaseController
{
    private readonly ISystemPermissionService _service;

    public SystemPermissionsController(
        ISystemPermissionService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢系統權限列表
    /// </summary>
    [HttpGet("list")]
    public async Task<ActionResult<ApiResponse<SystemPermissionListResponseDto>>> GetSystemPermissionList(
        [FromQuery] SystemPermissionListRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSystemPermissionListAsync(request);
            return result;
        }, "查詢系統權限列表失敗");
    }

    /// <summary>
    /// 匯出系統權限報表
    /// </summary>
    [HttpPost("export")]
    public async Task<IActionResult> ExportSystemPermissionReport(
        [FromBody] SystemPermissionExportRequestDto request)
    {
        try
        {
            var fileBytes = await _service.ExportSystemPermissionReportAsync(
                request.Request,
                request.ExportFormat);

            var contentType = request.ExportFormat == "Excel"
                ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                : "application/pdf";

            var fileName = $"系統權限列表_{DateTime.Now:yyyyMMddHHmmss}.{(request.ExportFormat == "Excel" ? "xlsx" : "pdf")}";

            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError($"匯出系統權限報表失敗: {ex.Message}", ex);
            return BadRequest(ApiResponse<object>.Fail($"匯出系統權限報表失敗: {ex.Message}"));
        }
    }
}

