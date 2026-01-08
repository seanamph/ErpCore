using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Kiosk;
using ErpCore.Application.Services.Kiosk;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Kiosk;

/// <summary>
/// Kiosk報表查詢控制器
/// </summary>
[Route("api/v1/kiosk/reports")]
public class KioskReportController : BaseController
{
    private readonly IKioskReportService _service;

    public KioskReportController(
        IKioskReportService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢Kiosk交易記錄
    /// </summary>
    [HttpGet("transactions")]
    public async Task<ActionResult<ApiResponse<PagedResult<KioskTransactionDto>>>> GetTransactions(
        [FromQuery] KioskTransactionQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetTransactionsAsync(query);
            return result;
        }, "查詢Kiosk交易記錄失敗");
    }

    /// <summary>
    /// 查詢Kiosk交易統計
    /// </summary>
    [HttpGet("statistics")]
    public async Task<ActionResult<ApiResponse<List<KioskStatisticsDto>>>> GetStatistics(
        [FromQuery] KioskStatisticsQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetStatisticsAsync(query);
            return result;
        }, "查詢Kiosk交易統計失敗");
    }

    /// <summary>
    /// 查詢Kiosk功能代碼統計
    /// </summary>
    [HttpGet("function-statistics")]
    public async Task<ActionResult<ApiResponse<List<KioskFunctionStatisticsDto>>>> GetFunctionStatistics(
        [FromQuery] KioskStatisticsQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetFunctionStatisticsAsync(query);
            return result;
        }, "查詢Kiosk功能代碼統計失敗");
    }

    /// <summary>
    /// 查詢Kiosk錯誤分析
    /// </summary>
    [HttpGet("error-analysis")]
    public async Task<ActionResult<ApiResponse<List<KioskErrorAnalysisDto>>>> GetErrorAnalysis(
        [FromQuery] KioskStatisticsQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetErrorAnalysisAsync(query);
            return result;
        }, "查詢Kiosk錯誤分析失敗");
    }

    /// <summary>
    /// 匯出Kiosk交易報表
    /// </summary>
    [HttpPost("export")]
    public async Task<IActionResult> ExportReport([FromBody] KioskReportExportDto dto)
    {
        try
        {
            var fileBytes = await _service.ExportReportAsync(dto);
            var fileName = $"Kiosk報表_{DateTime.Now:yyyyMMdd}.{(dto.ExportType == "Excel" ? "xlsx" : "pdf")}";
            return File(fileBytes,
                dto.ExportType == "Excel"
                    ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                    : "application/pdf",
                fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出Kiosk交易報表失敗", ex);
            return BadRequest(ApiResponse<object>.Fail("匯出失敗：" + ex.Message, "EXPORT_ERROR"));
        }
    }
}

