using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.System;

/// <summary>
/// 權限分類報表控制器 (SYS0770)
/// </summary>
[Route("api/v1/permission-category-reports")]
public class PermissionCategoryReportController : BaseController
{
    private readonly IPermissionCategoryReportService _service;

    public PermissionCategoryReportController(
        IPermissionCategoryReportService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢權限分類報表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PermissionCategoryReportResponseDto>>> GetPermissionCategoryReport(
        [FromQuery] PermissionCategoryReportRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPermissionCategoryReportAsync(request);
            return result;
        }, "查詢權限分類報表失敗");
    }

    /// <summary>
    /// 匯出權限分類報表
    /// </summary>
    [HttpPost("export")]
    public async Task<IActionResult> ExportPermissionCategoryReport(
        [FromBody] PermissionCategoryReportExportRequestDto request)
    {
        try
        {
            var fileBytes = await _service.ExportPermissionCategoryReportAsync(
                request.Request,
                request.ExportFormat);

            var contentType = request.ExportFormat == "Excel"
                ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                : "application/pdf";

            var fileName = $"權限分類報表_{DateTime.Now:yyyyMMddHHmmss}.{(request.ExportFormat == "Excel" ? "xlsx" : "pdf")}";

            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError($"匯出權限分類報表失敗: {ex.Message}", ex);
            return BadRequest(ApiResponse<object>.Fail($"匯出權限分類報表失敗: {ex.Message}"));
        }
    }
}

