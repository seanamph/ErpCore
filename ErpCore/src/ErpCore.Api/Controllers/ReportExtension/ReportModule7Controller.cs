using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.ReportExtension;
using ErpCore.Application.Services.ReportExtension;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.ReportExtension;

/// <summary>
/// 報表模組7控制器 (SYS7000)
/// </summary>
[Route("api/v1/reports/sys7000")]
public class ReportModule7Controller : BaseController
{
    private readonly IReportModule7Service _service;

    public ReportModule7Controller(
        IReportModule7Service service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢報表列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ReportQueryDto>>>> GetReports([FromQuery] ReportQueryListDto query)
    {
        return await ExecuteAsync(async () =>
        {
            return await _service.GetReportsAsync(query);
        }, "查詢報表列表失敗");
    }

    /// <summary>
    /// 執行報表查詢
    /// </summary>
    [HttpPost("{reportCode}/query")]
    public async Task<ActionResult<ApiResponse<ReportQueryResultDto>>> QueryReport(string reportCode, [FromBody] ReportQueryRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            request.ReportCode = reportCode;
            return await _service.QueryReportAsync(request);
        }, "執行報表查詢失敗");
    }

    /// <summary>
    /// 匯出報表資料
    /// </summary>
    [HttpPost("{reportCode}/export")]
    public async Task<ActionResult> ExportReport(string reportCode, [FromBody] ReportQueryRequestDto request, [FromQuery] string format = "Excel")
    {
        return await ExecuteAsync(async () =>
        {
            request.ReportCode = reportCode;
            var fileData = await _service.ExportReportAsync(reportCode, request, format);
            var contentType = format.ToUpper() == "PDF" ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"{reportCode}_{DateTime.Now:yyyyMMddHHmmss}.{(format.ToUpper() == "PDF" ? "pdf" : "xlsx")}";
            return File(fileData, contentType, fileName);
        }, "匯出報表資料失敗");
    }
}

