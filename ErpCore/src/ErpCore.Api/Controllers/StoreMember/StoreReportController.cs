using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.StoreMember;
using ErpCore.Application.Services.StoreMember;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.StoreMember;

/// <summary>
/// 報表查詢作業控制器 (SYS3410-SYS3440 - 報表查詢作業)
/// </summary>
[Route("api/v1/reports/store-member")]
public class StoreReportController : BaseController
{
    private readonly IStoreReportService _service;

    public StoreReportController(
        IStoreReportService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢會員統計報表
    /// </summary>
    [HttpGet("member-statistics")]
    public async Task<ActionResult<ApiResponse<PagedResult<MemberStatisticsReportDto>>>> GetMemberStatistics(
        [FromQuery] StoreReportQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetMemberStatisticsAsync(query);
            return result;
        }, "查詢會員統計報表失敗");
    }

    /// <summary>
    /// 查詢商店銷售報表
    /// </summary>
    [HttpGet("shop-sales")]
    public async Task<ActionResult<ApiResponse<PagedResult<ShopSalesReportDto>>>> GetShopSales(
        [FromQuery] StoreReportQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetShopSalesAsync(query);
            return result;
        }, "查詢商店銷售報表失敗");
    }

    /// <summary>
    /// 查詢促銷活動效果報表
    /// </summary>
    [HttpGet("promotion-effect")]
    public async Task<ActionResult<ApiResponse<PagedResult<PromotionEffectReportDto>>>> GetPromotionEffect(
        [FromQuery] StoreReportQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPromotionEffectAsync(query);
            return result;
        }, "查詢促銷活動效果報表失敗");
    }

    /// <summary>
    /// 匯出報表（Excel）
    /// </summary>
    [HttpPost("export-excel")]
    public async Task<ActionResult> ExportReportExcel([FromBody] StoreReportExportDto request)
    {
        try
        {
            var fileBytes = await _service.ExportReportAsync(request, "Excel");
            var fileName = $"{request.ReportType}_報表_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出報表失敗", ex);
            return BadRequest(ApiResponse<object>.Fail("匯出失敗: " + ex.Message));
        }
    }

    /// <summary>
    /// 匯出報表（PDF）
    /// </summary>
    [HttpPost("export-pdf")]
    public async Task<ActionResult> ExportReportPdf([FromBody] StoreReportExportDto request)
    {
        try
        {
            var fileBytes = await _service.ExportReportAsync(request, "PDF");
            var fileName = $"{request.ReportType}_報表_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            return File(fileBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出報表失敗", ex);
            return BadRequest(ApiResponse<object>.Fail("匯出失敗: " + ex.Message));
        }
    }

    /// <summary>
    /// 列印報表
    /// </summary>
    [HttpPost("print")]
    public async Task<ActionResult> PrintReport([FromBody] StoreReportExportDto request)
    {
        try
        {
            var fileBytes = await _service.PrintReportAsync(request);
            var fileName = $"{request.ReportType}_報表_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            return File(fileBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("列印報表失敗", ex);
            return BadRequest(ApiResponse<object>.Fail("列印失敗: " + ex.Message));
        }
    }
}

