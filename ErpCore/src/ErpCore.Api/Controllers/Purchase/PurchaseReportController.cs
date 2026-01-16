using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Purchase;
using ErpCore.Application.Services.Purchase;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Purchase;

/// <summary>
/// 採購報表查詢控制器 (SYSP410-SYSP4I0)
/// </summary>
[ApiController]
[Route("api/v1/purchase-reports")]
public class PurchaseReportController : BaseController
{
    private readonly IPurchaseReportService _service;

    public PurchaseReportController(
        IPurchaseReportService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢採購報表列表
    /// </summary>
    [HttpGet("query")]
    public async Task<ActionResult<ApiResponse<PagedResult<PurchaseReportResultDto>>>> QueryPurchaseReports(
        [FromQuery] PurchaseReportQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.QueryPurchaseReportsAsync(query);
            return result;
        }, "查詢採購報表列表失敗");
    }

    /// <summary>
    /// 查詢採購報表明細列表
    /// </summary>
    [HttpGet("details")]
    public async Task<ActionResult<ApiResponse<PagedResult<PurchaseReportDetailResultDto>>>> QueryPurchaseReportDetails(
        [FromQuery] PurchaseReportQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.QueryPurchaseReportDetailsAsync(query);
            return result;
        }, "查詢採購報表明細列表失敗");
    }

    /// <summary>
    /// 匯出採購報表
    /// </summary>
    [HttpPost("export")]
    public async Task<IActionResult> ExportPurchaseReport([FromBody] PurchaseReportExportDto exportDto)
    {
        try
        {
            var fileBytes = await _service.ExportPurchaseReportAsync(exportDto);
            var fileName = exportDto.FileName ?? $"採購報表_{DateTime.Now:yyyyMMddHHmmss}.{exportDto.ExportType.ToLower()}";
            var contentType = exportDto.ExportType.ToLower() switch
            {
                "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "pdf" => "application/pdf",
                "csv" => "text/csv",
                _ => "application/octet-stream"
            };

            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出採購報表失敗", ex);
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Code = 400,
                Message = "匯出採購報表失敗: " + ex.Message
            });
        }
    }
}
