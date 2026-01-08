using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Inventory;
using ErpCore.Application.Services.Inventory;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Inventory;

/// <summary>
/// POP卡商品卡列印作業控制器 - AP版本 (SYSW171)
/// </summary>
[Route("api/v1/pop-print-ap")]
public class PopPrintApController : BaseController
{
    private readonly IPopPrintService _service;

    public PopPrintApController(
        IPopPrintService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢商品列表（用於列印）- AP版本
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
    /// 產生列印資料 - AP版本
    /// </summary>
    [HttpPost("generate")]
    public async Task<ActionResult<ApiResponse<PopPrintDataDto>>> GeneratePrintData(
        [FromBody] GeneratePrintDataDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            // 強制設定版本為AP
            dto.Version = "AP";
            var result = await _service.GeneratePrintDataAsync(dto);
            return result;
        }, "產生列印資料失敗");
    }

    /// <summary>
    /// 執行列印 - AP版本
    /// </summary>
    [HttpPost("print")]
    public async Task<ActionResult<ApiResponse<PrintJobDto>>> Print(
        [FromBody] PrintRequestDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            // 強制設定版本為AP
            dto.Version = "AP";
            var result = await _service.PrintAsync(dto);
            return result;
        }, "執行列印失敗");
    }

    /// <summary>
    /// 取得列印設定 - AP版本
    /// </summary>
    [HttpGet("settings/{shopId?}")]
    public async Task<ActionResult<ApiResponse<PopPrintSettingDto?>>> GetSettings(
        string? shopId = null)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSettingsAsync(shopId, "AP");
            return result;
        }, $"取得列印設定失敗: {shopId}");
    }

    /// <summary>
    /// 更新列印設定 - AP版本
    /// </summary>
    [HttpPut("settings/{shopId?}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateSettings(
        string? shopId,
        [FromBody] UpdatePopPrintSettingDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            // 強制設定版本為AP
            dto.Version = "AP";
            await _service.UpdateSettingsAsync(shopId, dto);
        }, $"更新列印設定失敗: {shopId}");
    }

    /// <summary>
    /// 查詢列印記錄列表 - AP版本
    /// </summary>
    [HttpGet("logs")]
    public async Task<ActionResult<ApiResponse<PagedResult<PopPrintLogDto>>>> GetLogs(
        [FromQuery] PopPrintLogQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            // 自動過濾AP版本記錄
            query.Version = "AP";
            var result = await _service.GetLogsAsync(query);
            return result;
        }, "查詢列印記錄列表失敗");
    }

    /// <summary>
    /// 匯出Excel - AP版本
    /// </summary>
    [HttpPost("export-excel")]
    public async Task<IActionResult> ExportExcel(
        [FromBody] GeneratePrintDataDto dto)
    {
        try
        {
            // 強制設定版本為AP
            dto.Version = "AP";
            var data = await _service.ExportExcelAsync(dto);
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                $"POP列印資料_AP_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出Excel失敗", ex);
            return BadRequest(ApiResponse<object>.Fail("匯出Excel失敗", "EXPORT_ERROR"));
        }
    }
}

