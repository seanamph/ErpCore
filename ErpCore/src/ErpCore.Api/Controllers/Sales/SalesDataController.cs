using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Sales;
using ErpCore.Application.Services.Sales;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Sales;

/// <summary>
/// 銷售資料維護控制器 (SYSD110-SYSD140)
/// </summary>
[Route("api/v1/sales-orders")]
public class SalesDataController : BaseController
{
    private readonly ISalesOrderService _service;

    public SalesDataController(
        ISalesOrderService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢銷售單列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<SalesOrderDto>>>> GetSalesOrders(
        [FromQuery] SalesOrderQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSalesOrdersAsync(query);
            return result;
        }, "查詢銷售單列表失敗");
    }

    /// <summary>
    /// 查詢單筆銷售單
    /// </summary>
    [HttpGet("{orderId}")]
    public async Task<ActionResult<ApiResponse<SalesOrderDto>>> GetSalesOrder(string orderId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSalesOrderByIdAsync(orderId);
            return result;
        }, $"查詢銷售單失敗: {orderId}");
    }

    /// <summary>
    /// 新增銷售單
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateSalesOrder(
        [FromBody] CreateSalesOrderDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateSalesOrderAsync(dto);
            return result;
        }, "新增銷售單失敗");
    }

    /// <summary>
    /// 修改銷售單
    /// </summary>
    [HttpPut("{orderId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateSalesOrder(
        string orderId,
        [FromBody] UpdateSalesOrderDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateSalesOrderAsync(orderId, dto);
        }, $"修改銷售單失敗: {orderId}");
    }

    /// <summary>
    /// 刪除銷售單
    /// </summary>
    [HttpDelete("{orderId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteSalesOrder(string orderId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteSalesOrderAsync(orderId);
        }, $"刪除銷售單失敗: {orderId}");
    }
}

