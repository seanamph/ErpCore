using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.InvoiceSales;
using ErpCore.Application.Services.InvoiceSales;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.InvoiceSales;

/// <summary>
/// 銷售查詢控制器 (SYSG510-SYSG5D0 - 銷售查詢作業)
/// </summary>
[Route("api/v1/sales-orders/query")]
public class SalesQueryController : BaseController
{
    private readonly ISalesOrderQueryService _service;

    public SalesQueryController(
        ISalesOrderQueryService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢銷售單列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<SalesOrderQueryDto>>>> Query(
        [FromQuery] SalesOrderQueryConditionDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.QueryAsync(query);
            return result;
        }, "查詢銷售單列表失敗");
    }

    /// <summary>
    /// 查詢銷售單統計
    /// </summary>
    [HttpGet("statistics")]
    public async Task<ActionResult<ApiResponse<SalesOrderStatisticsDto>>> GetStatistics(
        [FromQuery] SalesOrderStatisticsQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetStatisticsAsync(query);
            return result;
        }, "查詢銷售單統計失敗");
    }
}

