using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Certificate;
using ErpCore.Application.Services.Certificate;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Certificate;

/// <summary>
/// 憑證報表查詢控制器 (SYSK310-SYSK500)
/// 提供憑證報表查詢功能
/// </summary>
[Route("api/v1/vouchers/reports")]
public class CertificateReportController : BaseController
{
    private readonly ICertificateReportService _reportService;

    public CertificateReportController(
        ICertificateReportService reportService,
        ILoggerService logger) : base(logger)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// 憑證明細報表查詢
    /// </summary>
    [HttpPost("detail")]
    public async Task<ActionResult<ApiResponse<PagedResult<VoucherDto>>>> GetVoucherDetailReport(
        [FromBody] VoucherReportQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _reportService.GetVoucherDetailReportAsync(query);
            return result;
        }, "查詢憑證明細報表失敗");
    }

    /// <summary>
    /// 憑證統計報表查詢
    /// </summary>
    [HttpPost("statistics")]
    public async Task<ActionResult<ApiResponse<VoucherStatisticsReportDto>>> GetVoucherStatisticsReport(
        [FromBody] VoucherReportQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _reportService.GetVoucherStatisticsReportAsync(query);
            return result;
        }, "查詢憑證統計報表失敗");
    }

    /// <summary>
    /// 憑證分析報表查詢
    /// </summary>
    [HttpPost("analysis")]
    public async Task<ActionResult<ApiResponse<PagedResult<VoucherDto>>>> GetVoucherAnalysisReport(
        [FromBody] VoucherReportQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _reportService.GetVoucherAnalysisReportAsync(query);
            return result;
        }, "查詢憑證分析報表失敗");
    }
}

