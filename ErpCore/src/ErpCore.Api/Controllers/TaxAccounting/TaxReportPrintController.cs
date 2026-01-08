using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.TaxAccounting;
using ErpCore.Application.Services.TaxAccounting;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.TaxAccounting;

/// <summary>
/// 稅務報表列印控制器 (SYST510-SYST530)
/// 提供SAP拋轉-銀行往來、稅務報表列印等功能
/// </summary>
[Route("api/v1/tax-accounting/tax-report-prints")]
public class TaxReportPrintController : BaseController
{
    private readonly ITaxReportPrintService _service;

    public TaxReportPrintController(
        ITaxReportPrintService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    #region SYST510 - SAP拋轉-銀行往來

    /// <summary>
    /// 查詢SAP銀行往來資料
    /// </summary>
    [HttpGet("syst510/bank-total")]
    public async Task<ActionResult<ApiResponse<PagedResult<SapBankTotalDto>>>> GetSapBankTotal(
        [FromQuery] SapBankTotalQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSapBankTotalAsync(query);
            return result;
        }, "查詢SAP銀行往來資料失敗");
    }

    /// <summary>
    /// 產生SAP銀行往來CSV檔案
    /// </summary>
    [HttpPost("syst510/generate-csv")]
    public async Task<ActionResult<ApiResponse<CsvFileDto>>> GenerateSapBankTotalCsv(
        [FromBody] GenerateCsvDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GenerateSapBankTotalCsvAsync(dto);
            return result;
        }, "產生SAP銀行往來CSV檔案失敗");
    }

    /// <summary>
    /// 下載SAP銀行往來CSV檔案
    /// </summary>
    [HttpGet("syst510/download/{fileName}")]
    public async Task<IActionResult> DownloadCsv(string fileName)
    {
        try
        {
            var stream = await _service.DownloadCsvAsync(fileName);
            return File(stream, "text/csv", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError($"下載CSV檔案失敗: {fileName}", ex);
            return BadRequest(ApiResponse<object>.Fail($"下載CSV檔案失敗: {ex.Message}"));
        }
    }

    #endregion

    #region 稅務報表列印記錄管理

    /// <summary>
    /// 查詢列印記錄列表
    /// </summary>
    [HttpGet("print-logs")]
    public async Task<ActionResult<ApiResponse<PagedResult<TaxReportPrintDto>>>> GetPrintLogs(
        [FromQuery] TaxReportPrintQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPrintLogsAsync(query);
            return result;
        }, "查詢列印記錄列表失敗");
    }

    /// <summary>
    /// 新增列印記錄
    /// </summary>
    [HttpPost("print-logs")]
    public async Task<ActionResult<ApiResponse<long>>> CreatePrintLog(
        [FromBody] CreateTaxReportPrintDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreatePrintLogAsync(dto);
            return result;
        }, "新增列印記錄失敗");
    }

    /// <summary>
    /// 修改列印記錄
    /// </summary>
    [HttpPut("print-logs/{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdatePrintLog(
        long tKey,
        [FromBody] UpdateTaxReportPrintDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdatePrintLogAsync(tKey, dto);
            return (object)null!;
        }, $"修改列印記錄失敗: {tKey}");
    }

    /// <summary>
    /// 刪除列印記錄
    /// </summary>
    [HttpDelete("print-logs/{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeletePrintLog(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeletePrintLogAsync(tKey);
            return (object)null!;
        }, $"刪除列印記錄失敗: {tKey}");
    }

    #endregion
}

