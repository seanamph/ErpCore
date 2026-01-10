using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Accounting;
using ErpCore.Application.Services.Accounting;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Accounting;

/// <summary>
/// 財務報表控制器 (SYSN510-SYSN540)
/// 提供財務報表查詢、列印、統計、分析等功能
/// </summary>
[Route("api/v1/accounting/financial-reports")]
public class FinancialReportController : BaseController
{
    private readonly IFinancialReportService _financialReportService;

    public FinancialReportController(
        IFinancialReportService financialReportService,
        ILoggerService logger) : base(logger)
    {
        _financialReportService = financialReportService;
    }

    /// <summary>
    /// 查詢財務報表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<FinancialReportDto>>>> GetFinancialReports(
        [FromQuery] FinancialReportQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _financialReportService.GetFinancialReportsAsync(query);
            return result;
        }, "查詢財務報表失敗");
    }

    /// <summary>
    /// 匯出財務報表
    /// </summary>
    [HttpPost("export")]
    public async Task<ActionResult> ExportFinancialReports([FromBody] ExportFinancialReportDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var fileData = await _financialReportService.ExportFinancialReportsAsync(dto);
            
            var contentType = dto.ExportFormat.ToUpper() == "PDF" ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileExtension = dto.ExportFormat.ToUpper() == "PDF" ? ".pdf" : ".xlsx";
            var fileName = $"financial_report_{DateTime.Now:yyyyMMddHHmmss}{fileExtension}";

            return File(fileData, contentType, fileName);
        }, "匯出財務報表失敗");
    }
}

