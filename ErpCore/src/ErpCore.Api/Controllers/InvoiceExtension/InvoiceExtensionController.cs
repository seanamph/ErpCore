using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.Services.InvoiceExtension;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.InvoiceExtension;

/// <summary>
/// 電子發票擴展功能控制器 (ECA1000)
/// </summary>
[Route("api/v1/einvoices/extensions")]
public class InvoiceExtensionController : BaseController
{
    private readonly IEInvoiceExtensionService _service;

    public InvoiceExtensionController(
        IEInvoiceExtensionService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢電子發票擴展列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<EInvoiceExtensionDto>>>> GetExtensions(
        [FromQuery] EInvoiceExtensionQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetExtensionsAsync(query);
            return result;
        }, "查詢電子發票擴展列表失敗");
    }

    /// <summary>
    /// 查詢單筆電子發票擴展
    /// </summary>
    [HttpGet("{extensionId}")]
    public async Task<ActionResult<ApiResponse<EInvoiceExtensionDto>>> GetExtension(long extensionId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetExtensionByIdAsync(extensionId);
            return result;
        }, $"查詢電子發票擴展失敗: {extensionId}");
    }

    /// <summary>
    /// 新增電子發票擴展
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<long>>> CreateExtension(
        [FromBody] CreateEInvoiceExtensionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var id = await _service.CreateExtensionAsync(dto);
            return id;
        }, "新增電子發票擴展失敗");
    }

    /// <summary>
    /// 修改電子發票擴展
    /// </summary>
    [HttpPut("{extensionId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateExtension(
        long extensionId,
        [FromBody] UpdateEInvoiceExtensionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateExtensionAsync(extensionId, dto);
            return new object();
        }, $"修改電子發票擴展失敗: {extensionId}");
    }

    /// <summary>
    /// 刪除電子發票擴展
    /// </summary>
    [HttpDelete("{extensionId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteExtension(long extensionId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteExtensionAsync(extensionId);
            return new object();
        }, $"刪除電子發票擴展失敗: {extensionId}");
    }
}

