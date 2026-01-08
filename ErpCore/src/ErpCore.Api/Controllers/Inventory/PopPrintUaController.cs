using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Inventory;
using ErpCore.Application.Services.Inventory;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Inventory;

/// <summary>
/// POP卡商品卡列印作業控制器 - UA版本 (SYSW172)
/// </summary>
[Route("api/v1/pop-print-ua")]
public class PopPrintUaController : BaseController
{
    private readonly IPopPrintService _service;

    public PopPrintUaController(
        IPopPrintService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢商品列表（用於列印）- UA版本
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
    /// 產生列印資料 - UA版本
    /// </summary>
    [HttpPost("generate")]
    public async Task<ActionResult<ApiResponse<PopPrintDataDto>>> GeneratePrintData(
        [FromBody] GeneratePrintDataDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            // 強制設定版本為UA
            dto.Version = "UA";
            var result = await _service.GeneratePrintDataAsync(dto);
            return result;
        }, "產生列印資料失敗");
    }

    /// <summary>
    /// 執行列印 - UA版本
    /// </summary>
    [HttpPost("print")]
    public async Task<ActionResult<ApiResponse<PrintJobDto>>> Print(
        [FromBody] PrintRequestDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            // 強制設定版本為UA
            dto.Version = "UA";
            var result = await _service.PrintAsync(dto);
            return result;
        }, "執行列印失敗");
    }

    /// <summary>
    /// 取得列印設定 - UA版本
    /// </summary>
    [HttpGet("settings/{shopId?}")]
    public async Task<ActionResult<ApiResponse<PopPrintSettingDto?>>> GetSettings(
        string? shopId = null)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSettingsAsync(shopId, "UA");
            return result;
        }, $"取得列印設定失敗: {shopId}");
    }

    /// <summary>
    /// 更新列印設定 - UA版本
    /// </summary>
    [HttpPut("settings/{shopId?}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateSettings(
        string? shopId,
        [FromBody] UpdatePopPrintSettingDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            // 強制設定版本為UA
            dto.Version = "UA";
            await _service.UpdateSettingsAsync(shopId, dto);
        }, $"更新列印設定失敗: {shopId}");
    }

    /// <summary>
    /// 查詢列印記錄列表 - UA版本
    /// </summary>
    [HttpGet("logs")]
    public async Task<ActionResult<ApiResponse<PagedResult<PopPrintLogDto>>>> GetLogs(
        [FromQuery] PopPrintLogQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            // 自動過濾UA版本記錄
            query.Version = "UA";
            var result = await _service.GetLogsAsync(query);
            return result;
        }, "查詢列印記錄列表失敗");
    }

    /// <summary>
    /// 匯出Excel - UA版本
    /// </summary>
    [HttpPost("export-excel")]
    public async Task<IActionResult> ExportExcel(
        [FromBody] GeneratePrintDataDto dto)
    {
        try
        {
            // 強制設定版本為UA
            dto.Version = "UA";
            var data = await _service.ExportExcelAsync(dto);
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                $"POP列印資料_UA_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出Excel失敗", ex);
            return BadRequest(ApiResponse<object>.Fail("匯出Excel失敗", "EXPORT_ERROR"));
        }
    }
}

