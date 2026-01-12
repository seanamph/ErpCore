using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.AnalysisReport;
using ErpCore.Application.Services.AnalysisReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.AnalysisReport;

/// <summary>
/// 耗材管理報表控制器 (SYSA255)
/// </summary>
[Route("api/v1/consumables/report")]
public class ConsumableReportController : BaseController
{
    private readonly IConsumableReportService _service;

    public ConsumableReportController(
        IConsumableReportService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢耗材管理報表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<ConsumableReportResponseDto>>> GetReport(
        [FromQuery] ConsumableReportQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetReportAsync(query);
            return result;
        }, "查詢耗材管理報表失敗");
    }

    /// <summary>
    /// 匯出耗材管理報表
    /// </summary>
    [HttpPost("export")]
    public async Task<ActionResult> ExportReport(
        [FromBody] ConsumableReportExportDto exportDto)
    {
        return await ExecuteAsync(async () =>
        {
            var fileBytes = await _service.ExportReportAsync(exportDto);
            var contentType = exportDto.ExportType == "Excel" 
                ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" 
                : "application/pdf";
            var fileName = $"耗材管理報表_{DateTime.Now:yyyyMMddHHmmss}.{(exportDto.ExportType == "Excel" ? "xlsx" : "pdf")}";
            
            return File(fileBytes, contentType, fileName);
        }, "匯出耗材管理報表失敗");
    }

    /// <summary>
    /// 查詢耗材使用明細
    /// </summary>
    [HttpGet("{consumableId}/transactions")]
    public async Task<ActionResult<ApiResponse<PagedResult<ConsumableTransactionDto>>>> GetTransactions(
        string consumableId,
        [FromQuery] ConsumableTransactionQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            query.ConsumableId = consumableId;
            var result = await _service.GetTransactionsAsync(query);
            return result;
        }, "查詢耗材使用明細失敗");
    }
}
