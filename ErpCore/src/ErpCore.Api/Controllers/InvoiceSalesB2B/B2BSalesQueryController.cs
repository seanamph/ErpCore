using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.InvoiceSalesB2B;
using ErpCore.Application.Services.InvoiceSalesB2B;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.InvoiceSalesB2B;

/// <summary>
/// B2B銷售查詢作業控制器 (SYSG000_B2B - B2B銷售查詢作業)
/// </summary>
[Route("api/v1/b2b-sales-orders/query")]
public class B2BSalesQueryController : BaseController
{
    private readonly IB2BSalesOrderQueryService _service;

    public B2BSalesQueryController(
        IB2BSalesOrderQueryService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢B2B銷售單列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<B2BSalesOrderQueryDto>>>> QueryB2BSalesOrders(
        [FromQuery] B2BSalesOrderQueryConditionDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.QueryAsync(query);
            return result;
        }, "查詢B2B銷售單列表失敗");
    }

    /// <summary>
    /// 查詢B2B銷售單統計
    /// </summary>
    [HttpGet("statistics")]
    public async Task<ActionResult<ApiResponse<B2BSalesOrderStatisticsDto>>> GetB2BSalesOrderStatistics(
        [FromQuery] B2BSalesOrderStatisticsQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetStatisticsAsync(query);
            return result;
        }, "查詢B2B銷售單統計失敗");
    }
}

