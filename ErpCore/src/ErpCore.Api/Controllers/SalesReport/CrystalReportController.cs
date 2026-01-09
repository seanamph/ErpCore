using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.SalesReport;
using ErpCore.Application.Services.SalesReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.SalesReport;

/// <summary>
/// Crystal Reports 報表控制器 (SYS1360 - Crystal Reports報表功能)
/// 提供銷售報表的查詢、產生、下載、列印功能
/// </summary>
[Route("api/v1/crystal-reports")]
public class CrystalReportController : BaseController
{
    private readonly ISalesReportService _salesReportService;

    public CrystalReportController(
        ISalesReportService salesReportService,
        ILoggerService logger) : base(logger)
    {
        _salesReportService = salesReportService;
    }

    /// <summary>
    /// 查詢 Crystal Reports 報表列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<SalesReportDto>>>> GetCrystalReports(
        [FromQuery] SalesReportQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            // 設定報表代碼為 SYS1360
            query.ReportCode = "SYS1360";
            var result = await _salesReportService.GetSalesReportsAsync(query);
            return result;
        }, "查詢 Crystal Reports 報表列表失敗");
    }

    /// <summary>
    /// 查詢單筆 Crystal Reports 報表
    /// </summary>
    [HttpGet("{reportId}")]
    public async Task<ActionResult<ApiResponse<SalesReportDto>>> GetCrystalReport(string reportId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _salesReportService.GetSalesReportByIdAsync(reportId);
            if (result == null)
            {
                throw new InvalidOperationException($"報表不存在: {reportId}");
            }
            return result;
        }, $"查詢 Crystal Reports 報表失敗: {reportId}");
    }

    /// <summary>
    /// 產生 Crystal Reports 報表
    /// </summary>
    [HttpPost("generate")]
    public async Task<ActionResult<ApiResponse<SalesReportDto>>> GenerateCrystalReport(
        [FromBody] CreateSalesReportDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            // 設定報表代碼為 SYS1360
            dto.ReportCode = "SYS1360";
            var result = await _salesReportService.CreateSalesReportAsync(dto);
            return result;
        }, "產生 Crystal Reports 報表失敗");
    }

    /// <summary>
    /// 修改 Crystal Reports 報表
    /// </summary>
    [HttpPut("{reportId}")]
    public async Task<ActionResult<ApiResponse<SalesReportDto>>> UpdateCrystalReport(
        string reportId,
        [FromBody] UpdateSalesReportDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            // 設定報表代碼為 SYS1360
            dto.ReportCode = "SYS1360";
            var result = await _salesReportService.UpdateSalesReportAsync(reportId, dto);
            return result;
        }, $"修改 Crystal Reports 報表失敗: {reportId}");
    }

    /// <summary>
    /// 刪除 Crystal Reports 報表
    /// </summary>
    [HttpDelete("{reportId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteCrystalReport(string reportId)
    {
        return await ExecuteAsync(async () =>
        {
            await _salesReportService.DeleteSalesReportAsync(reportId);
            return new { Message = "報表刪除成功" };
        }, $"刪除 Crystal Reports 報表失敗: {reportId}");
    }
}

