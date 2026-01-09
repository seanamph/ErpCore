using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.InvoiceSalesB2B;
using ErpCore.Application.Services.InvoiceSalesB2B;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.InvoiceSalesB2B;

/// <summary>
/// B2B發票資料維護控制器 (SYSG000_B2B - B2B發票資料維護)
/// </summary>
[Route("api/v1/b2b-invoices")]
public class B2BInvoiceDataController : BaseController
{
    private readonly IB2BInvoiceService _service;

    public B2BInvoiceDataController(
        IB2BInvoiceService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢B2B發票列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<B2BInvoiceDto>>>> GetB2BInvoices(
        [FromQuery] B2BInvoiceQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetB2BInvoicesAsync(query);
            return result;
        }, "查詢B2B發票列表失敗");
    }

    /// <summary>
    /// 查詢單筆B2B發票
    /// </summary>
    [HttpGet("{tKey}")]
    public async Task<ActionResult<ApiResponse<B2BInvoiceDto>>> GetB2BInvoice(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetB2BInvoiceByIdAsync(tKey);
            return result;
        }, $"查詢B2B發票失敗: {tKey}");
    }

    /// <summary>
    /// 新增B2B發票
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<long>>> CreateB2BInvoice(
        [FromBody] CreateB2BInvoiceDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateB2BInvoiceAsync(dto);
            return result;
        }, "新增B2B發票失敗");
    }

    /// <summary>
    /// 修改B2B發票
    /// </summary>
    [HttpPut("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateB2BInvoice(
        long tKey,
        [FromBody] UpdateB2BInvoiceDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            dto.TKey = tKey;
            await _service.UpdateB2BInvoiceAsync(dto);
            return new object();
        }, $"修改B2B發票失敗: {tKey}");
    }

    /// <summary>
    /// 刪除B2B發票
    /// </summary>
    [HttpDelete("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteB2BInvoice(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteB2BInvoiceAsync(tKey);
            return new object();
        }, $"刪除B2B發票失敗: {tKey}");
    }
}

