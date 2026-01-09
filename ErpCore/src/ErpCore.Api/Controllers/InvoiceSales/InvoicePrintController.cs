using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.InvoiceSales;
using ErpCore.Application.Services.InvoiceSales;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.InvoiceSales;

/// <summary>
/// 電子發票列印控制器 (SYSG210-SYSG2B0)
/// </summary>
[Route("api/v1/electronic-invoices")]
public class InvoicePrintController : BaseController
{
    private readonly IElectronicInvoiceService _service;
    private readonly IElectronicInvoicePrintSettingService _settingService;

    public InvoicePrintController(
        IElectronicInvoiceService service,
        IElectronicInvoicePrintSettingService settingService,
        ILoggerService logger) : base(logger)
    {
        _service = service;
        _settingService = settingService;
    }

    /// <summary>
    /// 查詢電子發票列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ElectronicInvoiceDto>>>> GetElectronicInvoices(
        [FromQuery] ElectronicInvoiceQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetElectronicInvoicesAsync(query);
            return result;
        }, "查詢電子發票列表失敗");
    }

    /// <summary>
    /// 查詢單筆電子發票
    /// </summary>
    [HttpGet("{tKey}")]
    public async Task<ActionResult<ApiResponse<ElectronicInvoiceDto>>> GetElectronicInvoice(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetElectronicInvoiceByIdAsync(tKey);
            return result;
        }, $"查詢電子發票失敗: {tKey}");
    }

    /// <summary>
    /// 新增電子發票
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<long>>> CreateElectronicInvoice(
        [FromBody] CreateElectronicInvoiceDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateElectronicInvoiceAsync(dto);
            return result;
        }, "新增電子發票失敗");
    }

    /// <summary>
    /// 修改電子發票
    /// </summary>
    [HttpPut("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateElectronicInvoice(
        long tKey,
        [FromBody] UpdateElectronicInvoiceDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            dto.TKey = tKey;
            await _service.UpdateElectronicInvoiceAsync(dto);
            return new object();
        }, $"修改電子發票失敗: {tKey}");
    }

    /// <summary>
    /// 刪除電子發票
    /// </summary>
    [HttpDelete("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteElectronicInvoice(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteElectronicInvoiceAsync(tKey);
            return new object();
        }, $"刪除電子發票失敗: {tKey}");
    }

    /// <summary>
    /// 電子發票手動取號列印
    /// </summary>
    [HttpPost("manual-print")]
    public async Task<ActionResult<ApiResponse<PrintDataDto>>> ManualPrint(
        [FromBody] ManualPrintDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.ManualPrintAsync(dto);
            return result;
        }, "電子發票手動取號列印失敗");
    }

    /// <summary>
    /// 查詢中獎清冊
    /// </summary>
    [HttpPost("award-list")]
    public async Task<ActionResult<ApiResponse<PagedResult<ElectronicInvoiceAwardDto>>>> GetAwardList(
        [FromBody] AwardListQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetAwardListAsync(query);
            return result;
        }, "查詢中獎清冊失敗");
    }

    /// <summary>
    /// 中獎清冊列印
    /// </summary>
    [HttpPost("award-print")]
    public async Task<ActionResult<ApiResponse<PrintDataDto>>> AwardPrint(
        [FromBody] AwardPrintDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.AwardPrintAsync(dto);
            return result;
        }, "中獎清冊列印失敗");
    }

    /// <summary>
    /// 查詢電子發票列印設定
    /// </summary>
    [HttpGet("print-settings")]
    public async Task<ActionResult<ApiResponse<List<ElectronicInvoicePrintSettingDto>>>> GetPrintSettings()
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _settingService.GetAllActiveSettingsAsync();
            return result;
        }, "查詢電子發票列印設定失敗");
    }

    /// <summary>
    /// 查詢單筆電子發票列印設定
    /// </summary>
    [HttpGet("print-settings/{settingId}")]
    public async Task<ActionResult<ApiResponse<ElectronicInvoicePrintSettingDto>>> GetPrintSetting(string settingId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _settingService.GetSettingAsync(settingId);
            if (result == null)
            {
                throw new KeyNotFoundException($"電子發票列印設定不存在: {settingId}");
            }
            return result;
        }, $"查詢電子發票列印設定失敗: {settingId}");
    }

    /// <summary>
    /// 更新電子發票列印設定
    /// </summary>
    [HttpPut("print-settings/{settingId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdatePrintSetting(
        string settingId,
        [FromBody] UpdateElectronicInvoicePrintSettingDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            dto.SettingId = settingId;
            await _settingService.UpdateSettingAsync(dto);
            return new object();
        }, $"更新電子發票列印設定失敗: {settingId}");
    }
}

