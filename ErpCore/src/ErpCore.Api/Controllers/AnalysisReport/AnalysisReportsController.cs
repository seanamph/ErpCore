using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.AnalysisReport;
using ErpCore.Application.Services.AnalysisReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.AnalysisReport;

/// <summary>
/// 分析報表控制器 (SYSA1000)
/// </summary>
[Route("api/v1/analysis-reports")]
public class AnalysisReportsController : BaseController
{
    private readonly IAnalysisReportService _service;

    public AnalysisReportsController(
        IAnalysisReportService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢商品分析報表 (SYSA1011)
    /// </summary>
    [HttpGet("sysa1011")]
    public async Task<ActionResult<ApiResponse<PagedResult<SYSA1011ReportDto>>>> GetSYSA1011Report(
        [FromQuery] SYSA1011QueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSYSA1011ReportAsync(query);
            return result;
        }, "查詢商品分析報表失敗");
    }

    /// <summary>
    /// 匯出商品分析報表 (SYSA1011)
    /// </summary>
    [HttpPost("sysa1011/export")]
    public async Task<IActionResult> ExportSYSA1011Report(
        [FromBody] SYSA1011QueryDto query,
        [FromQuery] string format = "excel")
    {
        try
        {
            var fileBytes = await _service.ExportSYSA1011ReportAsync(query, format);
            var contentType = format.ToLower() == "pdf" ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"商品分析報表_{DateTime.Now:yyyyMMddHHmmss}.{(format.ToLower() == "pdf" ? "pdf" : "xlsx")}";
            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出商品分析報表失敗", ex);
            return BadRequest(new ApiResponse { Success = false, Message = "匯出商品分析報表失敗" });
        }
    }

    /// <summary>
    /// 列印商品分析報表 (SYSA1011)
    /// </summary>
    [HttpPost("sysa1011/print")]
    public async Task<IActionResult> PrintSYSA1011Report([FromBody] SYSA1011QueryDto query)
    {
        try
        {
            var fileBytes = await _service.PrintSYSA1011ReportAsync(query);
            var fileName = $"商品分析報表_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            return File(fileBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("列印商品分析報表失敗", ex);
            return BadRequest(new ApiResponse { Success = false, Message = "列印商品分析報表失敗" });
        }
    }

    /// <summary>
    /// 查詢商品分類列表 (SYSA1011)
    /// </summary>
    [HttpGet("sysa1011/categories")]
    public async Task<ActionResult<ApiResponse<IEnumerable<GoodsCategoryDto>>>> GetGoodsCategories(
        [FromQuery] string categoryType,
        [FromQuery] string? parentId = null)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetGoodsCategoriesAsync(categoryType, parentId);
            return result;
        }, "查詢商品分類列表失敗");
    }

    /// <summary>
    /// 查詢進銷存月報表 (SYSA1012)
    /// </summary>
    [HttpGet("sysa1012")]
    public async Task<ActionResult<ApiResponse<PagedResult<SYSA1012ReportDto>>>> GetSYSA1012Report(
        [FromQuery] SYSA1012QueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSYSA1012ReportAsync(query);
            return result;
        }, "查詢進銷存月報表失敗");
    }

    /// <summary>
    /// 匯出進銷存月報表 (SYSA1012)
    /// </summary>
    [HttpPost("sysa1012/export")]
    public async Task<IActionResult> ExportSYSA1012Report(
        [FromBody] SYSA1012QueryDto query,
        [FromQuery] string format = "excel")
    {
        try
        {
            var fileBytes = await _service.ExportSYSA1012ReportAsync(query, format);
            var contentType = format.ToLower() == "pdf" ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"進銷存月報表_{DateTime.Now:yyyyMMddHHmmss}.{(format.ToLower() == "pdf" ? "pdf" : "xlsx")}";
            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出進銷存月報表失敗", ex);
            return BadRequest(new ApiResponse { Success = false, Message = "匯出進銷存月報表失敗" });
        }
    }

    /// <summary>
    /// 列印進銷存月報表 (SYSA1012)
    /// </summary>
    [HttpPost("sysa1012/print")]
    public async Task<IActionResult> PrintSYSA1012Report([FromBody] SYSA1012QueryDto query)
    {
        try
        {
            var fileBytes = await _service.PrintSYSA1012ReportAsync(query);
            var fileName = $"進銷存月報表_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            return File(fileBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("列印進銷存月報表失敗", ex);
            return BadRequest(new ApiResponse { Success = false, Message = "列印進銷存月報表失敗" });
        }
    }

    /// <summary>
    /// 查詢進銷存分析報表 (SYSA1000) - 通用查詢方法
    /// </summary>
    [HttpGet("{reportId}")]
    public async Task<ActionResult<ApiResponse<PagedResult<Dictionary<string, object>>>>> GetAnalysisReport(
        string reportId,
        [FromQuery] AnalysisReportQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetAnalysisReportAsync(reportId, query);
            return result;
        }, $"查詢進銷存分析報表失敗: {reportId}");
    }

    /// <summary>
    /// 匯出進銷存分析報表 (SYSA1000)
    /// </summary>
    [HttpPost("{reportId}/export")]
    public async Task<IActionResult> ExportAnalysisReport(
        string reportId,
        [FromBody] ExportReportDto dto,
        [FromQuery] string? format = null)
    {
        try
        {
            var exportFormat = format ?? dto.Format;
            var fileBytes = await _service.ExportAnalysisReportAsync(reportId, dto.QueryParams, exportFormat);
            var contentType = exportFormat.ToLower() == "pdf" ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"{reportId}_進銷存分析報表_{DateTime.Now:yyyyMMddHHmmss}.{(exportFormat.ToLower() == "pdf" ? "pdf" : "xlsx")}";
            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError($"匯出進銷存分析報表失敗: {reportId}", ex);
            return BadRequest(new ApiResponse { Success = false, Message = $"匯出進銷存分析報表失敗: {reportId}" });
        }
    }

    /// <summary>
    /// 列印進銷存分析報表 (SYSA1000)
    /// </summary>
    [HttpPost("{reportId}/print")]
    public async Task<IActionResult> PrintAnalysisReport(
        string reportId,
        [FromBody] AnalysisReportQueryDto query)
    {
        try
        {
            var fileBytes = await _service.PrintAnalysisReportAsync(reportId, query);
            var fileName = $"{reportId}_進銷存分析報表_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            return File(fileBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError($"列印進銷存分析報表失敗: {reportId}", ex);
            return BadRequest(new ApiResponse { Success = false, Message = $"列印進銷存分析報表失敗: {reportId}" });
        }
    }
}
