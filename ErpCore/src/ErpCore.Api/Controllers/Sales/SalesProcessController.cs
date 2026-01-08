using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Sales;
using ErpCore.Application.Services.Sales;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Sales;

/// <summary>
/// 銷售處理作業控制器 (SYSD210-SYSD230)
/// </summary>
[Route("api/v1/sales-orders")]
public class SalesProcessController : BaseController
{
    private readonly ISalesOrderService _service;

    public SalesProcessController(
        ISalesOrderService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 審核銷售單
    /// </summary>
    [HttpPost("{orderId}/approve")]
    public async Task<ActionResult<ApiResponse<object>>> ApproveSalesOrder(
        string orderId,
        [FromBody] ApproveSalesOrderDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.ApproveSalesOrderAsync(orderId, dto);
        }, $"審核銷售單失敗: {orderId}");
    }

    /// <summary>
    /// 出貨銷售單
    /// </summary>
    [HttpPost("{orderId}/ship")]
    public async Task<ActionResult<ApiResponse<object>>> ShipSalesOrder(
        string orderId,
        [FromBody] ShipSalesOrderDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.ShipSalesOrderAsync(orderId, dto);
        }, $"出貨銷售單失敗: {orderId}");
    }

    /// <summary>
    /// 取消銷售單
    /// </summary>
    [HttpPost("{orderId}/cancel")]
    public async Task<ActionResult<ApiResponse<object>>> CancelSalesOrder(
        string orderId,
        [FromBody] CancelSalesOrderDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.CancelSalesOrderAsync(orderId, dto);
        }, $"取消銷售單失敗: {orderId}");
    }
}

