using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.InvoiceSales;
using ErpCore.Application.Services.InvoiceSales;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.InvoiceSales;

/// <summary>
/// 銷售資料維護控制器 (SYSG410-SYSG460)
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
    /// 查詢單筆銷售單（含明細）
    /// </summary>
    [HttpGet("{tKey}")]
    public async Task<ActionResult<ApiResponse<SalesOrderDto>>> GetSalesOrder(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSalesOrderByIdAsync(tKey);
            return result;
        }, $"查詢銷售單失敗: {tKey}");
    }

    /// <summary>
    /// 根據銷售單號查詢銷售單（含明細）
    /// </summary>
    [HttpGet("by-order-id/{orderId}")]
    public async Task<ActionResult<ApiResponse<SalesOrderDto>>> GetSalesOrderByOrderId(string orderId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSalesOrderByOrderIdAsync(orderId);
            return result;
        }, $"查詢銷售單失敗: {orderId}");
    }

    /// <summary>
    /// 新增銷售單（含明細）
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<long>>> CreateSalesOrder(
        [FromBody] CreateSalesOrderDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateSalesOrderAsync(dto);
            return result;
        }, "新增銷售單失敗");
    }

    /// <summary>
    /// 修改銷售單（含明細）
    /// </summary>
    [HttpPut("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateSalesOrder(
        long tKey,
        [FromBody] UpdateSalesOrderDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            dto.TKey = tKey;
            await _service.UpdateSalesOrderAsync(dto);
            return new object();
        }, $"修改銷售單失敗: {tKey}");
    }

    /// <summary>
    /// 刪除銷售單
    /// </summary>
    [HttpDelete("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteSalesOrder(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteSalesOrderAsync(tKey);
            return new object();
        }, $"刪除銷售單失敗: {tKey}");
    }
}

