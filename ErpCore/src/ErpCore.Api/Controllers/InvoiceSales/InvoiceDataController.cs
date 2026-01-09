using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.InvoiceSales;
using ErpCore.Application.Services.InvoiceSales;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.InvoiceSales;

/// <summary>
/// 發票資料維護控制器 (SYSG110-SYSG190)
/// </summary>
[Route("api/v1/invoices")]
public class InvoiceDataController : BaseController
{
    private readonly IInvoiceService _service;

    public InvoiceDataController(
        IInvoiceService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢發票列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<InvoiceDto>>>> GetInvoices(
        [FromQuery] InvoiceQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetInvoicesAsync(query);
            return result;
        }, "查詢發票列表失敗");
    }

    /// <summary>
    /// 查詢單筆發票
    /// </summary>
    [HttpGet("{tKey}")]
    public async Task<ActionResult<ApiResponse<InvoiceDto>>> GetInvoice(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetInvoiceByIdAsync(tKey);
            return result;
        }, $"查詢發票失敗: {tKey}");
    }

    /// <summary>
    /// 新增發票
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<long>>> CreateInvoice(
        [FromBody] CreateInvoiceDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateInvoiceAsync(dto);
            return result;
        }, "新增發票失敗");
    }

    /// <summary>
    /// 修改發票
    /// </summary>
    [HttpPut("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateInvoice(
        long tKey,
        [FromBody] UpdateInvoiceDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            dto.TKey = tKey;
            await _service.UpdateInvoiceAsync(dto);
            return new object();
        }, $"修改發票失敗: {tKey}");
    }

    /// <summary>
    /// 刪除發票
    /// </summary>
    [HttpDelete("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteInvoice(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteInvoiceAsync(tKey);
            return new object();
        }, $"刪除發票失敗: {tKey}");
    }
}

