using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.SystemExtension;
using ErpCore.Application.Services.SystemExtension;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.SystemExtension;

/// <summary>
/// 系統擴展報表控制器 (SYSX140)
/// </summary>
[Route("api/v1/system-extensions/reports")]
public class SystemExtensionReportController : BaseController
{
    private readonly ISystemExtensionReportService _service;

    public SystemExtensionReportController(
        ISystemExtensionReportService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢報表資料
    /// </summary>
    [HttpPost("query")]
    public async Task<ActionResult<ApiResponse<SystemExtensionReportQueryResultDto>>> QueryReportData(
        [FromBody] SystemExtensionReportQueryRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.QueryReportDataAsync(request);
            return result;
        }, "查詢系統擴展報表資料失敗");
    }

    /// <summary>
    /// 產生 PDF 報表
    /// </summary>
    [HttpPost("pdf")]
    public async Task<ActionResult<ApiResponse<SystemExtensionReportDto>>> GeneratePdfReport(
        [FromBody] GenerateSystemExtensionReportDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GeneratePdfReportAsync(request);
            return result;
        }, "產生 PDF 報表失敗");
    }

    /// <summary>
    /// 產生 Excel 報表
    /// </summary>
    [HttpPost("excel")]
    public async Task<ActionResult<ApiResponse<SystemExtensionReportDto>>> GenerateExcelReport(
        [FromBody] GenerateSystemExtensionReportDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GenerateExcelReportAsync(request);
            return result;
        }, "產生 Excel 報表失敗");
    }

    /// <summary>
    /// 查詢報表記錄列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<SystemExtensionReportDto>>>> GetReports(
        [FromQuery] SystemExtensionReportQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetReportsAsync(query);
            return result;
        }, "查詢報表記錄失敗");
    }

    /// <summary>
    /// 下載報表檔案
    /// </summary>
    [HttpGet("{reportId}/download")]
    public async Task<IActionResult> DownloadReport(long reportId)
    {
        try
        {
            // 先取得報表資訊
            var reports = await _service.GetReportsAsync(new SystemExtensionReportQueryDto
            {
                PageIndex = 1,
                PageSize = int.MaxValue
            });
            
            var report = reports.Items.FirstOrDefault(x => x.ReportId == reportId);
            if (report == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Code = 404,
                    Message = "報表記錄不存在"
                });
            }

            var fileBytes = await _service.DownloadReportAsync(reportId);

            var contentType = report.ReportType == "PDF" 
                ? "application/pdf" 
                : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            
            var fileExtension = report.ReportType == "PDF" ? "pdf" : "xlsx";
            var fileName = $"{report.ReportName}_{report.GeneratedDate:yyyyMMddHHmmss}.{fileExtension}";

            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError($"下載報表失敗: {reportId}", ex);
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Code = 500,
                Message = $"下載報表失敗: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// 刪除報表記錄
    /// </summary>
    [HttpDelete("{reportId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteReport(long reportId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteReportAsync(reportId);
        }, $"刪除報表記錄失敗: {reportId}");
    }
}

