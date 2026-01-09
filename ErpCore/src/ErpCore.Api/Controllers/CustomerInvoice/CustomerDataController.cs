using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.CustomerInvoice;
using ErpCore.Application.Services.CustomerInvoice;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.CustomerInvoice;

/// <summary>
/// 客戶資料維護控制器 (SYS2000 - 客戶資料維護)
/// </summary>
[Route("api/v1/customer-data")]
public class CustomerDataController : BaseController
{
    private readonly ICustomerDataService _service;

    public CustomerDataController(
        ICustomerDataService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢客戶列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<CustomerDataDto>>>> GetCustomers(
        [FromQuery] CustomerDataQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetCustomersAsync(query);
            return result;
        }, "查詢客戶列表失敗");
    }

    /// <summary>
    /// 查詢單筆客戶
    /// </summary>
    [HttpGet("{customerId}")]
    public async Task<ActionResult<ApiResponse<CustomerDataDto>>> GetCustomer(string customerId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetCustomerByIdAsync(customerId);
            return result;
        }, $"查詢客戶失敗: {customerId}");
    }

    /// <summary>
    /// 新增客戶
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateCustomer(
        [FromBody] CreateCustomerDataDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateCustomerAsync(dto);
            return result;
        }, "新增客戶失敗");
    }

    /// <summary>
    /// 修改客戶
    /// </summary>
    [HttpPut("{customerId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateCustomer(
        string customerId,
        [FromBody] UpdateCustomerDataDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateCustomerAsync(customerId, dto);
        }, $"修改客戶失敗: {customerId}");
    }

    /// <summary>
    /// 刪除客戶
    /// </summary>
    [HttpDelete("{customerId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteCustomer(string customerId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteCustomerAsync(customerId);
        }, $"刪除客戶失敗: {customerId}");
    }

    /// <summary>
    /// 批次刪除客戶
    /// </summary>
    [HttpPost("batch-delete")]
    public async Task<ActionResult<ApiResponse<object>>> BatchDeleteCustomers(
        [FromBody] BatchDeleteCustomerDataDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.BatchDeleteCustomersAsync(dto);
        }, "批次刪除客戶失敗");
    }

    /// <summary>
    /// 檢查客戶編號是否存在
    /// </summary>
    [HttpGet("check/{customerId}")]
    public async Task<ActionResult<ApiResponse<bool>>> CheckCustomerExists(string customerId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.ExistsAsync(customerId);
            return result;
        }, $"檢查客戶編號是否存在失敗: {customerId}");
    }
}

