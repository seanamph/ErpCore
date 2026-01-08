using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.AnalysisReport;
using ErpCore.Application.Services.AnalysisReport;
using ErpCore.Domain.Entities.AnalysisReport;
using ErpCore.Infrastructure.Repositories.AnalysisReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.AnalysisReport;

/// <summary>
/// 進銷存分析報表控制器 (SYSA1000)
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
    /// 查詢進銷存分析報表
    /// </summary>
    [HttpGet("{reportId}")]
    public async Task<ActionResult<ApiResponse<AnalysisReportDto>>> GetAnalysisReport(
        string reportId,
        [FromQuery] AnalysisReportQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetAnalysisReportAsync(reportId, query);
            return result;
        }, $"查詢進銷存分析報表失敗: ReportId={reportId}");
    }

    /// <summary>
    /// 匯出進銷存分析報表
    /// </summary>
    [HttpPost("{reportId}/export")]
    public async Task<IActionResult> ExportAnalysisReport(
        string reportId,
        [FromBody] ExportAnalysisReportDto dto)
    {
        try
        {
            var fileBytes = await _service.ExportAnalysisReportAsync(reportId, dto);
            var reportName = GetReportName(reportId);
            var fileName = $"{reportName}_{DateTime.Now:yyyyMMddHHmmss}.{dto.Format.ToLower()}";
            var contentType = dto.Format == "Excel" 
                ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" 
                : "application/pdf";
            
            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError($"匯出進銷存分析報表失敗: ReportId={reportId}", ex);
            return BadRequest(ApiResponse<object>.Fail($"匯出進銷存分析報表失敗: {ex.Message}"));
        }
    }

    /// <summary>
    /// 列印進銷存分析報表
    /// </summary>
    [HttpPost("{reportId}/print")]
    public async Task<IActionResult> PrintAnalysisReport(
        string reportId,
        [FromBody] PrintAnalysisReportDto dto)
    {
        try
        {
            var fileBytes = await _service.PrintAnalysisReportAsync(reportId, dto);
            var reportName = GetReportName(reportId);
            var fileName = $"{reportName}_列印_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            var contentType = "application/pdf";
            
            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError($"列印進銷存分析報表失敗: ReportId={reportId}", ex);
            return BadRequest(ApiResponse<object>.Fail($"列印進銷存分析報表失敗: {ex.Message}"));
        }
    }

    /// <summary>
    /// 查詢耗材列表（用於列印）
    /// </summary>
    [HttpPost("consumables/print/list")]
    public async Task<ActionResult<ApiResponse<List<Consumable>>>> GetConsumablesForPrint(
        [FromBody] ConsumablePrintQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetConsumablesForPrintAsync(query);
            return result;
        }, "查詢耗材列表失敗");
    }

    /// <summary>
    /// 取得報表名稱
    /// </summary>
    private string GetReportName(string reportId)
    {
        return reportId switch
        {
            "SYSA1011" => "耗材庫存查詢表",
            "SYSA1012" => "耗材入庫明細表",
            "SYSA1013" => "耗材出庫明細表",
            "SYSA1014" => "耗材領用發料退回明細表",
            "SYSA1015" => "固定資產數量彙總表",
            "SYSA1016" => "庫房領料進價成本分攤表",
            "SYSA1017" => "工務修繕扣款報表",
            "SYSA1018" => "工務維修件數統計表",
            "SYSA1019" => "工務維修類別統計表",
            "SYSA1020" => "盤點差異明細表",
            "SYSA1021" => "耗材進銷存月報表",
            "SYSA1022" => "公務費用歸屬統計表",
            _ => "進銷存分析報表"
        };
    }
}

