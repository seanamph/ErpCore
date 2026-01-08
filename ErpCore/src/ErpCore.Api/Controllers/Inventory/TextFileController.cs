using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Inventory;
using ErpCore.Application.Services.Inventory;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Inventory;

/// <summary>
/// HT680 - BAT格式文本文件處理系列控制器
/// </summary>
[Route("api/v1/textfile")]
public class TextFileController : BaseController
{
    private readonly ITextFileService _service;

    public TextFileController(
        ITextFileService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 上傳文本文件並開始處理
    /// </summary>
    [HttpPost("upload")]
    public async Task<ActionResult<ApiResponse<TextFileProcessLogDto>>> UploadFile(
        IFormFile file,
        [FromForm] string fileType,
        [FromForm] string? shopId = null)
    {
        return await ExecuteAsync(async () =>
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("文件不能為空");
            }

            var result = await _service.UploadAndProcessFileAsync(file, fileType, shopId);
            return result;
        }, "上傳文件失敗");
    }

    /// <summary>
    /// 查詢處理記錄列表
    /// </summary>
    [HttpGet("process-logs")]
    public async Task<ActionResult<ApiResponse<PagedResult<TextFileProcessLogDto>>>> GetProcessLogs(
        [FromQuery] TextFileProcessLogQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetProcessLogsAsync(query);
            return result;
        }, "查詢處理記錄列表失敗");
    }

    /// <summary>
    /// 根據處理記錄ID查詢單筆記錄
    /// </summary>
    [HttpGet("process-logs/{logId}")]
    public async Task<ActionResult<ApiResponse<TextFileProcessLogDto>>> GetProcessLog(Guid logId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetProcessLogByIdAsync(logId);
            if (result == null)
            {
                throw new InvalidOperationException($"處理記錄不存在: {logId}");
            }
            return result;
        }, $"查詢處理記錄失敗: {logId}");
    }

    /// <summary>
    /// 查詢處理記錄的明細列表
    /// </summary>
    [HttpGet("process-logs/{logId}/details")]
    public async Task<ActionResult<ApiResponse<PagedResult<TextFileProcessDetailDto>>>> GetProcessDetails(
        Guid logId,
        [FromQuery] TextFileProcessDetailQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetProcessDetailsAsync(logId, query);
            return result;
        }, $"查詢處理明細列表失敗: {logId}");
    }

    /// <summary>
    /// 重新處理文件
    /// </summary>
    [HttpPost("process-logs/{logId}/reprocess")]
    public async Task<ActionResult<ApiResponse<object>>> ReprocessFile(Guid logId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.ReprocessFileAsync(logId);
        }, $"重新處理文件失敗: {logId}");
    }

    /// <summary>
    /// 刪除處理記錄
    /// </summary>
    [HttpDelete("process-logs/{logId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteProcessLog(Guid logId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteProcessLogAsync(logId);
        }, $"刪除處理記錄失敗: {logId}");
    }

    /// <summary>
    /// 下載處理結果文件（Excel或CSV格式）
    /// </summary>
    [HttpGet("process-logs/{logId}/download")]
    public async Task<IActionResult> DownloadProcessResult(
        Guid logId,
        [FromQuery] string format = "excel")
    {
        try
        {
            var fileBytes = await _service.DownloadProcessResultAsync(logId, format);
            var contentType = format.ToLower() == "excel" 
                ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" 
                : "text/csv";
            var fileName = $"處理結果_{logId}.{(format.ToLower() == "excel" ? "xlsx" : "csv")}";

            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError($"下載處理結果失敗: LogId={logId}, Format={format}", ex);
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Code = 400,
                Message = $"下載處理結果失敗: {ex.Message}",
                Timestamp = DateTime.UtcNow
            });
        }
    }
}

