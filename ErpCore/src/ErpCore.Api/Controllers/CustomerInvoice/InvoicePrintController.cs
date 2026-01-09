using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.CustomerInvoice;
using ErpCore.Application.Services.CustomerInvoice;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.CustomerInvoice;

/// <summary>
/// 發票列印作業控制器 (SYS2000 - 發票列印作業)
/// </summary>
[Route("api/v1/invoice-print")]
public class InvoicePrintController : BaseController
{
    private readonly IInvoicePrintService _service;

    public InvoicePrintController(
        IInvoicePrintService service,
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
    [HttpGet("{invoiceNo}")]
    public async Task<ActionResult<ApiResponse<InvoiceDto>>> GetInvoice(string invoiceNo)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetInvoiceByIdAsync(invoiceNo);
            return result;
        }, $"查詢發票失敗: {invoiceNo}");
    }

    /// <summary>
    /// 列印發票
    /// </summary>
    [HttpPost("{invoiceNo}/print")]
    public async Task<ActionResult<ApiResponse<object>>> PrintInvoice(
        string invoiceNo,
        [FromBody] InvoicePrintRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.PrintInvoiceAsync(invoiceNo, request);
        }, $"列印發票失敗: {invoiceNo}");
    }

    /// <summary>
    /// 批次列印發票
    /// </summary>
    [HttpPost("batch-print")]
    public async Task<ActionResult<ApiResponse<object>>> BatchPrintInvoices(
        [FromBody] BatchPrintInvoiceDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.BatchPrintInvoicesAsync(dto);
        }, "批次列印發票失敗");
    }

    /// <summary>
    /// 查詢列印記錄
    /// </summary>
    [HttpGet("{invoiceNo}/print-logs")]
    public async Task<ActionResult<ApiResponse<IEnumerable<InvoicePrintLogDto>>>> GetPrintLogs(string invoiceNo)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPrintLogsAsync(invoiceNo);
            return result;
        }, $"查詢列印記錄失敗: {invoiceNo}");
    }
}

