using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Inventory;
using ErpCore.Application.Services.Inventory;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Inventory;

/// <summary>
/// POP卡商品卡列印作業控制器 (SYSW170)
/// </summary>
[Route("api/v1/pop-print")]
public class PopPrintController : BaseController
{
    private readonly IPopPrintService _service;

    public PopPrintController(
        IPopPrintService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢商品列表（用於列印）
    /// </summary>
    [HttpGet("products")]
    public async Task<ActionResult<ApiResponse<PagedResult<PopPrintProductDto>>>> GetProducts(
        [FromQuery] PopPrintProductQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetProductsAsync(query);
            return result;
        }, "查詢商品列表失敗");
    }

    /// <summary>
    /// 產生列印資料
    /// </summary>
    [HttpPost("generate")]
    public async Task<ActionResult<ApiResponse<PopPrintDataDto>>> GeneratePrintData(
        [FromBody] GeneratePrintDataDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GeneratePrintDataAsync(dto);
            return result;
        }, "產生列印資料失敗");
    }

    /// <summary>
    /// 執行列印
    /// </summary>
    [HttpPost("print")]
    public async Task<ActionResult<ApiResponse<PrintJobDto>>> Print(
        [FromBody] PrintRequestDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.PrintAsync(dto);
            return result;
        }, "執行列印失敗");
    }

    /// <summary>
    /// 取得列印設定
    /// </summary>
    [HttpGet("settings/{shopId?}")]
    public async Task<ActionResult<ApiResponse<PopPrintSettingDto?>>> GetSettings(
        string? shopId = null)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSettingsAsync(shopId);
            return result;
        }, $"取得列印設定失敗: {shopId}");
    }

    /// <summary>
    /// 更新列印設定
    /// </summary>
    [HttpPut("settings/{shopId?}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateSettings(
        string? shopId,
        [FromBody] UpdatePopPrintSettingDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateSettingsAsync(shopId, dto);
        }, $"更新列印設定失敗: {shopId}");
    }

    /// <summary>
    /// 查詢列印記錄列表
    /// </summary>
    [HttpGet("logs")]
    public async Task<ActionResult<ApiResponse<PagedResult<PopPrintLogDto>>>> GetLogs(
        [FromQuery] PopPrintLogQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetLogsAsync(query);
            return result;
        }, "查詢列印記錄列表失敗");
    }

    /// <summary>
    /// 匯出Excel
    /// </summary>
    [HttpPost("export-excel")]
    public async Task<IActionResult> ExportExcel(
        [FromBody] GeneratePrintDataDto dto)
    {
        try
        {
            var data = await _service.ExportExcelAsync(dto);
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                $"POP列印資料_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出Excel失敗", ex);
            return BadRequest(ApiResponse<object>.Fail("匯出Excel失敗", "EXPORT_ERROR"));
        }
    }
}

