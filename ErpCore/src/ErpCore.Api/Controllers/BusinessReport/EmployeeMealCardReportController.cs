using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.BusinessReport;
using ErpCore.Application.Services.BusinessReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.BusinessReport;

/// <summary>
/// 員餐卡報表控制器 (SYSL210)
/// </summary>
[Route("api/v1/employee-meal-cards")]
public class EmployeeMealCardReportController : BaseController
{
    private readonly IEmployeeMealCardReportService _service;

    public EmployeeMealCardReportController(
        IEmployeeMealCardReportService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢員餐卡報表資料
    /// </summary>
    [HttpGet("reports")]
    public async Task<ActionResult<ApiResponse<PagedResult<EmployeeMealCardReportDto>>>> GetReports(
        [FromQuery] EmployeeMealCardReportQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetReportsAsync(query);
            return result;
        }, "查詢員餐卡報表資料失敗");
    }

    /// <summary>
    /// 列印員餐卡報表
    /// </summary>
    [HttpPost("reports/{reportType}/print")]
    public async Task<ActionResult<ApiResponse<BusinessReportPrintResultDto>>> PrintReport(
        string reportType,
        [FromBody] BusinessReportPrintRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.PrintReportAsync(reportType, request);
            return result;
        }, "列印員餐卡報表失敗");
    }

    /// <summary>
    /// 匯出員餐卡報表
    /// </summary>
    [HttpPost("reports/{reportType}/export")]
    public async Task<ActionResult<ApiResponse<BusinessReportPrintResultDto>>> ExportReport(
        string reportType,
        [FromBody] BusinessReportExportRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.ExportReportAsync(reportType, request);
            return result;
        }, "匯出員餐卡報表失敗");
    }

    /// <summary>
    /// 取得報表類型選項
    /// </summary>
    [HttpGet("report-types")]
    public async Task<ActionResult<ApiResponse<List<ReportTypeOptionDto>>>> GetReportTypes()
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetReportTypesAsync();
            return result;
        }, "取得報表類型選項失敗");
    }
}

