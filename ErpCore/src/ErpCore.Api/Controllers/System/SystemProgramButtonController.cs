using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.System;

/// <summary>
/// 系統作業與功能列表控制器 (SYS0810, SYS0780, SYS0999)
/// </summary>
[Route("api/v1/systems")]
public class SystemProgramButtonController : BaseController
{
    private readonly ISystemProgramButtonService _service;

    public SystemProgramButtonController(
        ISystemProgramButtonService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢系統作業與功能列表 (SYS0810)
    /// </summary>
    [HttpGet("{systemId}/programs-and-buttons")]
    public async Task<ActionResult<ApiResponse<SystemProgramButtonResponseDto>>> GetSystemProgramButtons(string systemId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSystemProgramButtonsAsync(systemId);
            return result;
        }, $"查詢系統作業與功能列表失敗: {systemId}");
    }

    /// <summary>
    /// 匯出系統作業與功能列表報表 (SYS0810)
    /// </summary>
    [HttpPost("{systemId}/programs-and-buttons/export")]
    public async Task<IActionResult> ExportSystemProgramButtons(string systemId, [FromQuery] string exportFormat = "Excel")
    {
        try
        {
            var fileBytes = await _service.ExportSystemProgramButtonsAsync(systemId, exportFormat);

            var contentType = exportFormat == "Excel"
                ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                : "application/pdf";

            var fileName = $"系統作業與功能列表_{systemId}_{DateTime.Now:yyyyMMddHHmmss}.{(exportFormat == "Excel" ? "xlsx" : "pdf")}";

            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError($"匯出系統作業與功能列表報表失敗: {ex.Message}", ex);
            return BadRequest(ApiResponse<object>.Fail($"匯出系統作業與功能列表報表失敗: {ex.Message}"));
        }
    }

    /// <summary>
    /// 查詢系統作業與功能列表（出庫用）(SYS0999)
    /// </summary>
    [HttpGet("{systemId}/programs-and-buttons/export-query")]
    public async Task<ActionResult<ApiResponse<SystemProgramButtonResponseDto>>> GetSystemProgramButtonsForExport(string systemId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSystemProgramButtonsForExportAsync(systemId);
            return result;
        }, $"查詢系統作業與功能列表（出庫用）失敗: {systemId}");
    }

    /// <summary>
    /// 匯出系統作業與功能列表報表（出庫用）(SYS0999)
    /// </summary>
    [HttpPost("{systemId}/programs-and-buttons/export-report")]
    public async Task<IActionResult> ExportSystemProgramButtonsReport(string systemId, [FromQuery] string exportFormat = "Excel")
    {
        try
        {
            var fileBytes = await _service.ExportSystemProgramButtonsReportAsync(systemId, exportFormat);

            var contentType = exportFormat == "Excel"
                ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                : "application/pdf";

            var fileName = $"系統作業與功能列表（出庫用）_{systemId}_{DateTime.Now:yyyyMMddHHmmss}.{(exportFormat == "Excel" ? "xlsx" : "pdf")}";

            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError($"匯出系統作業與功能列表報表（出庫用）失敗: {ex.Message}", ex);
            return BadRequest(ApiResponse<object>.Fail($"匯出系統作業與功能列表報表（出庫用）失敗: {ex.Message}"));
        }
    }
}

