using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.System;

/// <summary>
/// 作業權限之使用者列表控制器 (SYS0720)
/// </summary>
[Route("api/v1/program-user-permissions")]
public class ProgramUserPermissionController : BaseController
{
    private readonly IProgramUserPermissionService _service;

    public ProgramUserPermissionController(
        IProgramUserPermissionService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢作業權限之使用者列表
    /// </summary>
    [HttpGet("list")]
    public async Task<ActionResult<ApiResponse<ProgramUserPermissionListResponseDto>>> GetProgramUserPermissionList(
        [FromQuery] ProgramUserPermissionListRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetProgramUserPermissionListAsync(request);
            return result;
        }, "查詢作業權限之使用者列表失敗");
    }

    /// <summary>
    /// 匯出作業權限之使用者報表
    /// </summary>
    [HttpPost("export")]
    public async Task<IActionResult> ExportProgramUserPermissionReport(
        [FromBody] ProgramUserPermissionExportRequestDto request)
    {
        try
        {
            var fileBytes = await _service.ExportProgramUserPermissionReportAsync(
                request.Request,
                request.ExportFormat);

            var contentType = request.ExportFormat == "Excel"
                ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                : "application/pdf";

            var fileName = $"作業權限之使用者列表_{request.Request.ProgramId}_{DateTime.Now:yyyyMMddHHmmss}.{(request.ExportFormat == "Excel" ? "xlsx" : "pdf")}";

            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError($"匯出作業權限之使用者報表失敗: {ex.Message}", ex);
            return BadRequest(ApiResponse<object>.Fail($"匯出作業權限之使用者報表失敗: {ex.Message}"));
        }
    }
}

