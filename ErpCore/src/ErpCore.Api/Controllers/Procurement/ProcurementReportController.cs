using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Procurement;
using ErpCore.Application.Services.Procurement;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Procurement;

/// <summary>
/// 採購報表控制器 (SYSP410-SYSP4I0)
/// </summary>
[ApiController]
[Route("api/v1/purchase-reports")]
public class ProcurementReportController : BaseController
{
    private readonly IProcurementReportService _service;

    public ProcurementReportController(
        IProcurementReportService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢採購報表
    /// </summary>
    [HttpGet("query")]
    public async Task<ActionResult<ApiResponse<PagedResult<ProcurementReportDto>>>> QueryReport(
        [FromQuery] ProcurementReportQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.QueryReportAsync(query);
            return result;
        }, "查詢採購報表失敗");
    }

    /// <summary>
    /// 匯出採購報表
    /// </summary>
    [HttpPost("export")]
    public async Task<ActionResult> ExportReport(
        [FromBody] ExportProcurementReportDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var fileBytes = await _service.ExportReportAsync(dto);
            var fileName = dto.FileName ?? $"採購報表_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            var contentType = dto.ExportType == "PDF" ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            
            return File(fileBytes, contentType, fileName);
        }, "匯出採購報表失敗");
    }
}

