using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.InvoiceSalesB2B;
using ErpCore.Application.Services.InvoiceSalesB2B;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.InvoiceSalesB2B;

/// <summary>
/// B2B銷售資料維護控制器 (SYSG000_B2B - B2B銷售資料維護)
/// </summary>
[Route("api/v1/b2b-sales-orders")]
public class B2BSalesDataController : BaseController
{
    private readonly IB2BSalesOrderService _service;

    public B2BSalesDataController(
        IB2BSalesOrderService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢B2B銷售單列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<B2BSalesOrderDto>>>> GetB2BSalesOrders(
        [FromQuery] B2BSalesOrderQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetB2BSalesOrdersAsync(query);
            return result;
        }, "查詢B2B銷售單列表失敗");
    }

    /// <summary>
    /// 查詢單筆B2B銷售單
    /// </summary>
    [HttpGet("{tKey}")]
    public async Task<ActionResult<ApiResponse<B2BSalesOrderDto>>> GetB2BSalesOrder(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetB2BSalesOrderByIdAsync(tKey);
            return result;
        }, $"查詢B2B銷售單失敗: {tKey}");
    }

    /// <summary>
    /// 根據銷售單號查詢B2B銷售單
    /// </summary>
    [HttpGet("order/{orderId}")]
    public async Task<ActionResult<ApiResponse<B2BSalesOrderDto>>> GetB2BSalesOrderByOrderId(string orderId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetB2BSalesOrderByOrderIdAsync(orderId);
            return result;
        }, $"查詢B2B銷售單失敗: {orderId}");
    }

    /// <summary>
    /// 新增B2B銷售單
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<long>>> CreateB2BSalesOrder(
        [FromBody] CreateB2BSalesOrderDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateB2BSalesOrderAsync(dto);
            return result;
        }, "新增B2B銷售單失敗");
    }

    /// <summary>
    /// 修改B2B銷售單
    /// </summary>
    [HttpPut("{tKey}")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateB2BSalesOrder(
        long tKey,
        [FromBody] UpdateB2BSalesOrderDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            dto.TKey = tKey;
            await _service.UpdateB2BSalesOrderAsync(dto);
            return true;
        }, $"修改B2B銷售單失敗: {tKey}");
    }

    /// <summary>
    /// 刪除B2B銷售單
    /// </summary>
    [HttpDelete("{tKey}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteB2BSalesOrder(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteB2BSalesOrderAsync(tKey);
            return true;
        }, $"刪除B2B銷售單失敗: {tKey}");
    }

    /// <summary>
    /// B2B銷售單傳輸
    /// </summary>
    [HttpPost("{orderId}/transfer")]
    public async Task<ActionResult<ApiResponse<bool>>> TransferB2BSalesOrder(string orderId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.TransferB2BSalesOrderAsync(orderId);
            return true;
        }, $"B2B銷售單傳輸失敗: {orderId}");
    }
}

