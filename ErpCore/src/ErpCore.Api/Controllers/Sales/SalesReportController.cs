using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Sales;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Sales;

/// <summary>
/// 銷售報表查詢控制器 (SYSD310-SYSD430)
/// </summary>
[Route("api/v1/sales-orders/reports")]
public class SalesReportController : BaseController
{
    public SalesReportController(ILoggerService logger) : base(logger)
    {
    }

    /// <summary>
    /// 銷售明細報表查詢
    /// </summary>
    [HttpPost("detail")]
    public async Task<ActionResult<ApiResponse<object>>> GetSalesDetailReport(
        [FromBody] SalesReportQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            // TODO: 實作銷售明細報表查詢邏輯
            await Task.CompletedTask;
            return new object();
        }, "查詢銷售明細報表失敗");
    }

    /// <summary>
    /// 銷售統計報表查詢
    /// </summary>
    [HttpPost("statistics")]
    public async Task<ActionResult<ApiResponse<object>>> GetSalesStatisticsReport(
        [FromBody] SalesReportQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            // TODO: 實作銷售統計報表查詢邏輯
            await Task.CompletedTask;
            return new object();
        }, "查詢銷售統計報表失敗");
    }

    /// <summary>
    /// 銷售分析報表查詢
    /// </summary>
    [HttpPost("analysis")]
    public async Task<ActionResult<ApiResponse<object>>> GetSalesAnalysisReport(
        [FromBody] SalesReportQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            // TODO: 實作銷售分析報表查詢邏輯
            await Task.CompletedTask;
            return new object();
        }, "查詢銷售分析報表失敗");
    }
}

