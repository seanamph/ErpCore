using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.TaxAccounting;
using ErpCore.Application.Services.TaxAccounting;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.TaxAccounting;

/// <summary>
/// 傳票轉入控制器 (SYST002-SYST003)
/// 提供外部系統傳票資料的轉入功能
/// </summary>
[Route("api/v1/tax-accounting/voucher-import")]
public class VoucherImportController : BaseController
{
    private readonly IVoucherImportService _service;

    public VoucherImportController(
        IVoucherImportService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 上傳住金傳票檔案
    /// </summary>
    [HttpPost("ahm/upload")]
    [RequestSizeLimit(100_000_000)] // 100MB
    public async Task<ActionResult<ApiResponse<VoucherImportLogDto>>> UploadAhmFile(IFormFile file)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.UploadAhmFileAsync(file);
            return result;
        }, "上傳住金傳票檔案失敗");
    }

    /// <summary>
    /// 查詢轉入記錄列表
    /// </summary>
    [HttpGet("logs")]
    public async Task<ActionResult<ApiResponse<PagedResult<VoucherImportLogDto>>>> GetImportLogs(
        [FromQuery] VoucherImportLogQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetImportLogsAsync(query);
            return result;
        }, "查詢轉入記錄列表失敗");
    }

    /// <summary>
    /// 查詢轉入記錄明細
    /// </summary>
    [HttpGet("logs/{tKey}/details")]
    public async Task<ActionResult<ApiResponse<VoucherImportLogDetailDto>>> GetImportLogDetails(
        long tKey,
        [FromQuery] VoucherImportDetailQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetImportLogDetailsAsync(tKey, query);
            return result;
        }, $"查詢轉入記錄明細失敗: {tKey}");
    }

    /// <summary>
    /// 日立傳票轉入（日結傳票）
    /// </summary>
    [HttpPost("htv/daily")]
    public async Task<ActionResult<ApiResponse<ImportResultDto>>> ImportHtvDaily(
        [FromBody] ImportHtvDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.ImportHtvDailyAsync(dto);
            return result;
        }, "日立日結傳票轉入失敗");
    }

    /// <summary>
    /// 日立傳票轉入（月結傳票）
    /// </summary>
    [HttpPost("htv/monthly")]
    public async Task<ActionResult<ApiResponse<ImportResultDto>>> ImportHtvMonthly(
        [FromBody] ImportHtvDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.ImportHtvMonthlyAsync(dto);
            return result;
        }, "日立月結傳票轉入失敗");
    }

    /// <summary>
    /// 日立供應商資料轉入
    /// </summary>
    [HttpPost("htv/supplier")]
    public async Task<ActionResult<ApiResponse<ImportResultDto>>> ImportHtvSupplier(
        [FromBody] ImportHtvDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.ImportHtvSupplierAsync(dto);
            return result;
        }, "日立供應商資料轉入失敗");
    }

    /// <summary>
    /// 查詢轉入進度
    /// </summary>
    [HttpGet("logs/{tKey}/progress")]
    public async Task<ActionResult<ApiResponse<ImportProgressDto>>> GetImportProgress(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetImportProgressAsync(tKey);
            return result;
        }, $"查詢轉入進度失敗: {tKey}");
    }

    /// <summary>
    /// 重新處理轉入記錄
    /// </summary>
    [HttpPost("logs/{tKey}/retry")]
    public async Task<ActionResult<ApiResponse<ImportResultDto>>> RetryImport(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.RetryImportAsync(tKey);
            return result;
        }, $"重新處理轉入記錄失敗: {tKey}");
    }
}

