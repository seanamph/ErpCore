using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.BusinessReport;
using ErpCore.Application.Services.BusinessReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.BusinessReport;

/// <summary>
/// 業務報表列印記錄作業控制器 (SYSL161)
/// </summary>
[Route("api/v1/business-report-print-logs")]
public class BusinessReportPrintLogController : BaseController
{
    private readonly IBusinessReportPrintLogService _service;

    public BusinessReportPrintLogController(
        IBusinessReportPrintLogService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢業務報表列印記錄列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<BusinessReportPrintLogDto>>>> GetBusinessReportPrintLogs(
        [FromQuery] BusinessReportPrintLogQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetBusinessReportPrintLogsAsync(query);
            return result;
        }, "查詢業務報表列印記錄列表失敗");
    }

    /// <summary>
    /// 根據主鍵查詢單筆資料
    /// </summary>
    [HttpGet("{tKey}")]
    public async Task<ActionResult<ApiResponse<BusinessReportPrintLogDto>>> GetBusinessReportPrintLogById(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetBusinessReportPrintLogByIdAsync(tKey);
            if (result == null)
            {
                throw new Exception($"找不到業務報表列印記錄: {tKey}");
            }
            return result;
        }, "查詢業務報表列印記錄失敗");
    }

    /// <summary>
    /// 根據 ReportId 查詢列印記錄列表
    /// </summary>
    [HttpGet("report/{reportId}")]
    public async Task<ActionResult<ApiResponse<List<BusinessReportPrintLogDto>>>> GetBusinessReportPrintLogsByReportId(string reportId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetBusinessReportPrintLogsByReportIdAsync(reportId);
            return result;
        }, "查詢業務報表列印記錄失敗");
    }

    /// <summary>
    /// 列印業務報表
    /// </summary>
    [HttpPost("{reportId}/print")]
    public async Task<ActionResult<ApiResponse<BusinessReportPrintResultDto>>> PrintReport(
        string reportId,
        [FromBody] BusinessReportPrintRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.PrintReportAsync(reportId, request);
            return result;
        }, "列印業務報表失敗");
    }

    /// <summary>
    /// 預覽業務報表
    /// </summary>
    [HttpPost("{reportId}/preview")]
    public async Task<ActionResult<ApiResponse<object>>> PreviewReport(
        string reportId,
        [FromBody] BusinessReportPrintRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.PreviewReportAsync(reportId, request);
            return result;
        }, "預覽業務報表失敗");
    }

    /// <summary>
    /// 匯出業務報表
    /// </summary>
    [HttpPost("{reportId}/export")]
    public async Task<ActionResult<ApiResponse<BusinessReportPrintResultDto>>> ExportReport(
        string reportId,
        [FromBody] BusinessReportExportRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.ExportReportAsync(reportId, request);
            return result;
        }, "匯出業務報表失敗");
    }

    /// <summary>
    /// 刪除業務報表列印記錄
    /// </summary>
    [HttpDelete("{tKey}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteBusinessReportPrintLog(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.DeleteBusinessReportPrintLogAsync(tKey);
            return result;
        }, "刪除業務報表列印記錄失敗");
    }

    /// <summary>
    /// 下載業務報表列印檔案
    /// </summary>
    [HttpGet("{tKey}/download")]
    public async Task<IActionResult> DownloadFile(long tKey)
    {
        try
        {
            var fileBytes = await _service.GetFileBytesAsync(tKey);
            var fileName = await _service.GetFileNameAsync(tKey);

            // 設定 Content-Type
            var contentType = "application/octet-stream";
            var extension = Path.GetExtension(fileName).ToLower();
            switch (extension)
            {
                case ".pdf":
                    contentType = "application/pdf";
                    break;
                case ".xlsx":
                case ".xls":
                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    break;
            }

            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError($"下載業務報表列印檔案失敗: {tKey}", ex);
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Code = 400,
                Message = $"下載檔案失敗: {ex.Message}",
                Timestamp = DateTime.UtcNow
            });
        }
    }
}

