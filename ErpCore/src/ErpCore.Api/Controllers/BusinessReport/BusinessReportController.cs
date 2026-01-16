using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.BusinessReport;
using ErpCore.Application.Services.BusinessReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.BusinessReport;

/// <summary>
/// 業務報表查詢作業控制器 (SYSL135)
/// </summary>
[Route("api/v1/business-reports/sysl135")]
public class BusinessReportController : BaseController
{
    private readonly IBusinessReportService _service;

    public BusinessReportController(
        IBusinessReportService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢業務報表列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<BusinessReportDto>>>> GetBusinessReports(
        [FromQuery] BusinessReportQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetBusinessReportsAsync(query);
            return result;
        }, "查詢業務報表列表失敗");
    }

    /// <summary>
    /// 匯出業務報表
    /// </summary>
    [HttpPost("export")]
    public async Task<IActionResult> ExportBusinessReports(
        [FromBody] BusinessReportQueryDto query,
        [FromQuery] string format = "excel")
    {
        try
        {
            var fileBytes = await _service.ExportBusinessReportsAsync(query, format);
            var fileName = $"業務報表查詢_{DateTime.Now:yyyyMMddHHmmss}.{(format.ToLower() == "excel" ? "xlsx" : "pdf")}";
            var contentType = format.ToLower() == "excel"
                ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                : "application/pdf";
            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出業務報表失敗", ex);
            return BadRequest(ApiResponse<object>.Fail($"匯出業務報表失敗: {ex.Message}"));
        }
    }

    /// <summary>
    /// 列印業務報表
    /// </summary>
    [HttpPost("print")]
    public async Task<IActionResult> PrintBusinessReports([FromBody] BusinessReportQueryDto query)
    {
        try
        {
            var fileBytes = await _service.PrintBusinessReportsAsync(query);
            var fileName = $"業務報表查詢_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            return File(fileBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("列印業務報表失敗", ex);
            return BadRequest(ApiResponse<object>.Fail($"列印業務報表失敗: {ex.Message}"));
        }
    }
}

