using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.InvoiceSalesB2B;
using ErpCore.Application.Services.InvoiceSalesB2B;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.InvoiceSalesB2B;

/// <summary>
/// B2B電子發票列印控制器 (SYSG000_B2B - B2B電子發票列印)
/// </summary>
[Route("api/v1/b2b-electronic-invoices")]
public class B2BInvoicePrintController : BaseController
{
    private readonly IB2BElectronicInvoiceService _service;
    private readonly IB2BElectronicInvoicePrintSettingService _settingService;

    public B2BInvoicePrintController(
        IB2BElectronicInvoiceService service,
        IB2BElectronicInvoicePrintSettingService settingService,
        ILoggerService logger) : base(logger)
    {
        _service = service;
        _settingService = settingService;
    }

    /// <summary>
    /// 查詢B2B電子發票列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<B2BElectronicInvoiceDto>>>> GetB2BElectronicInvoices(
        [FromQuery] B2BElectronicInvoiceQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetB2BElectronicInvoicesAsync(query);
            return result;
        }, "查詢B2B電子發票列表失敗");
    }

    /// <summary>
    /// 查詢單筆B2B電子發票
    /// </summary>
    [HttpGet("{tKey}")]
    public async Task<ActionResult<ApiResponse<B2BElectronicInvoiceDto>>> GetB2BElectronicInvoice(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetB2BElectronicInvoiceByIdAsync(tKey);
            return result;
        }, $"查詢B2B電子發票失敗: {tKey}");
    }

    /// <summary>
    /// 新增B2B電子發票
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<long>>> CreateB2BElectronicInvoice(
        [FromBody] CreateB2BElectronicInvoiceDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateB2BElectronicInvoiceAsync(dto);
            return result;
        }, "新增B2B電子發票失敗");
    }

    /// <summary>
    /// 修改B2B電子發票
    /// </summary>
    [HttpPut("{tKey}")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateB2BElectronicInvoice(
        long tKey,
        [FromBody] UpdateB2BElectronicInvoiceDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            dto.TKey = tKey;
            await _service.UpdateB2BElectronicInvoiceAsync(dto);
            return true;
        }, $"修改B2B電子發票失敗: {tKey}");
    }

    /// <summary>
    /// 刪除B2B電子發票
    /// </summary>
    [HttpDelete("{tKey}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteB2BElectronicInvoice(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteB2BElectronicInvoiceAsync(tKey);
            return true;
        }, $"刪除B2B電子發票失敗: {tKey}");
    }

    /// <summary>
    /// B2B電子發票手動取號列印
    /// </summary>
    [HttpPost("manual-print")]
    public async Task<ActionResult<ApiResponse<B2BPrintDataDto>>> ManualPrint(
        [FromBody] B2BManualPrintDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.ManualPrintAsync(dto);
            return result;
        }, "B2B電子發票手動取號列印失敗");
    }

    /// <summary>
    /// 查詢B2B中獎清冊
    /// </summary>
    [HttpGet("award-list")]
    public async Task<ActionResult<ApiResponse<PagedResult<B2BElectronicInvoiceAwardDto>>>> GetAwardList(
        [FromQuery] B2BAwardListQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetAwardListAsync(query);
            return result;
        }, "查詢B2B中獎清冊失敗");
    }

    /// <summary>
    /// B2B中獎清冊列印
    /// </summary>
    [HttpPost("award-print")]
    public async Task<ActionResult<ApiResponse<B2BPrintDataDto>>> AwardPrint(
        [FromBody] B2BAwardPrintDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.AwardPrintAsync(dto);
            return result;
        }, "B2B中獎清冊列印失敗");
    }

    /// <summary>
    /// 查詢B2B電子發票列印設定
    /// </summary>
    [HttpGet("print-settings/{settingId}")]
    public async Task<ActionResult<ApiResponse<B2BElectronicInvoicePrintSettingDto>>> GetPrintSetting(string settingId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _settingService.GetSettingAsync(settingId);
            if (result == null)
            {
                throw new KeyNotFoundException($"B2B電子發票列印設定不存在: {settingId}");
            }
            return result;
        }, $"查詢B2B電子發票列印設定失敗: {settingId}");
    }

    /// <summary>
    /// 查詢所有啟用的B2B電子發票列印設定
    /// </summary>
    [HttpGet("print-settings")]
    public async Task<ActionResult<ApiResponse<List<B2BElectronicInvoicePrintSettingDto>>>> GetAllPrintSettings()
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _settingService.GetAllActiveSettingsAsync();
            return result;
        }, "查詢B2B電子發票列印設定列表失敗");
    }

    /// <summary>
    /// 更新B2B電子發票列印設定
    /// </summary>
    [HttpPut("print-settings/{settingId}")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdatePrintSetting(
        string settingId,
        [FromBody] UpdateB2BElectronicInvoicePrintSettingDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            dto.SettingId = settingId;
            await _settingService.UpdateSettingAsync(dto);
            return true;
        }, $"更新B2B電子發票列印設定失敗: {settingId}");
    }
}

