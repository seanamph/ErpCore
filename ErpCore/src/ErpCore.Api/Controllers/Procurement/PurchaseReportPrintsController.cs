using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Procurement;
using ErpCore.Application.Services.Procurement;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Procurement;

/// <summary>
/// 採購報表列印控制器
/// </summary>
[ApiController]
[Route("api/v1/purchase-report-prints")]
public class PurchaseReportPrintsController : BaseController
{
    private readonly IPurchaseReportPrintService _service;

    public PurchaseReportPrintsController(
        IPurchaseReportPrintService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢採購報表列印記錄列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<PurchaseReportPrintDto>>>> GetPurchaseReportPrints(
        [FromQuery] PurchaseReportPrintQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPurchaseReportPrintsAsync(query);
            return result;
        }, "查詢採購報表列印記錄列表失敗");
    }

    /// <summary>
    /// 查詢單筆採購報表列印記錄
    /// </summary>
    [HttpGet("{tKey}")]
    public async Task<ActionResult<ApiResponse<PurchaseReportPrintDto>>> GetPurchaseReportPrint(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPurchaseReportPrintByIdAsync(tKey);
            return result;
        }, $"查詢採購報表列印記錄失敗: {tKey}");
    }

    /// <summary>
    /// 新增採購報表列印記錄
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<PurchaseReportPrintResultDto>>> CreatePurchaseReportPrint(
        [FromBody] CreatePurchaseReportPrintDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreatePurchaseReportPrintAsync(dto);
            return result;
        }, "新增採購報表列印記錄失敗");
    }

    /// <summary>
    /// 修改採購報表列印記錄
    /// </summary>
    [HttpPut("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdatePurchaseReportPrint(
        long tKey,
        [FromBody] UpdatePurchaseReportPrintDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdatePurchaseReportPrintAsync(tKey, dto);
        }, $"修改採購報表列印記錄失敗: {tKey}");
    }

    /// <summary>
    /// 刪除採購報表列印記錄
    /// </summary>
    [HttpDelete("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeletePurchaseReportPrint(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeletePurchaseReportPrintAsync(tKey);
        }, $"刪除採購報表列印記錄失敗: {tKey}");
    }

    /// <summary>
    /// 批次刪除採購報表列印記錄
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ActionResult<ApiResponse<object>>> BatchDeletePurchaseReportPrints(
        [FromBody] BatchDeleteRequest request)
    {
        return await ExecuteAsync(async () =>
        {
            foreach (var tKey in request.TKeys)
            {
                await _service.DeletePurchaseReportPrintAsync(tKey);
            }
        }, "批次刪除採購報表列印記錄失敗");
    }

    /// <summary>
    /// 下載報表檔案
    /// </summary>
    [HttpGet("{tKey}/download")]
    public async Task<IActionResult> DownloadPurchaseReportPrint(long tKey)
    {
        try
        {
            var fileBytes = await _service.DownloadPurchaseReportPrintAsync(tKey);
            var entity = await _service.GetPurchaseReportPrintByIdAsync(tKey);
            
            return File(fileBytes, "application/pdf", entity.FileName ?? $"report_{tKey}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError($"下載報表檔案失敗: {tKey}", ex);
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Code = 400,
                Message = $"下載報表檔案失敗: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// 預覽報表
    /// </summary>
    [HttpPost("preview")]
    public async Task<ActionResult<ApiResponse<ReportPreviewDto>>> PreviewPurchaseReportPrint(
        [FromBody] CreatePurchaseReportPrintDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.PreviewPurchaseReportPrintAsync(dto);
            return result;
        }, "預覽報表失敗");
    }

    /// <summary>
    /// 取得報表模板列表
    /// </summary>
    [HttpGet("templates")]
    public async Task<ActionResult<ApiResponse<List<PurchaseReportTemplateDto>>>> GetPurchaseReportTemplates(
        [FromQuery] string? reportType,
        [FromQuery] string? reportCode)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPurchaseReportTemplatesAsync(reportType, reportCode);
            return result;
        }, "查詢報表模板列表失敗");
    }
}

/// <summary>
/// 批次刪除請求
/// </summary>
public class BatchDeleteRequest
{
    public List<long> TKeys { get; set; } = new();
}
