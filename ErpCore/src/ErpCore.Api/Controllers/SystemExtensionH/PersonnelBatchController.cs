using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.SystemExtensionH;
using ErpCore.Application.Services.SystemExtensionH;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.SystemExtensionH;

/// <summary>
/// 人事批量新增控制器 (SYSH3D0_FMI - 人事批量新增)
/// </summary>
[Route("api/v1/personnel/batch-import")]
public class PersonnelBatchController : BaseController
{
    private readonly IPersonnelBatchService _service;

    public PersonnelBatchController(
        IPersonnelBatchService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 上傳Excel檔案
    /// </summary>
    [HttpPost("upload")]
    public async Task<ActionResult<ApiResponse<PersonnelImportLogDto>>> UploadFile(
        [FromForm] IFormFile file)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.UploadFileAsync(file);
            return result;
        }, "上傳檔案失敗");
    }

    /// <summary>
    /// 執行批量匯入
    /// </summary>
    [HttpPost("{importId}/execute")]
    public async Task<ActionResult<ApiResponse<PersonnelImportResultDto>>> ExecuteImport(string importId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.ExecuteImportAsync(importId);
            return result;
        }, $"執行批量匯入失敗: {importId}");
    }

    /// <summary>
    /// 取得匯入進度
    /// </summary>
    [HttpGet("{importId}/progress")]
    public async Task<ActionResult<ApiResponse<PersonnelImportProgressDto>>> GetProgress(string importId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetProgressAsync(importId);
            return result;
        }, $"查詢匯入進度失敗: {importId}");
    }

    /// <summary>
    /// 取得匯入記錄
    /// </summary>
    [HttpGet("{importId}")]
    public async Task<ActionResult<ApiResponse<PersonnelImportLogDto>>> GetImportLog(string importId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetImportLogAsync(importId);
            return result;
        }, $"查詢匯入記錄失敗: {importId}");
    }

    /// <summary>
    /// 查詢匯入記錄列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<PersonnelImportLogDto>>>> GetImportLogs(
        [FromQuery] PersonnelImportLogQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetImportLogsAsync(query);
            return result;
        }, "查詢匯入記錄列表失敗");
    }

    /// <summary>
    /// 匯出失敗資料
    /// </summary>
    [HttpGet("{importId}/export-failed")]
    public async Task<ActionResult> ExportFailedData(string importId)
    {
        try
        {
            var data = await _service.ExportFailedDataAsync(importId);
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                $"失敗資料_{importId}_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }
        catch (Exception ex)
        {
            _logger.LogError($"匯出失敗資料失敗: {importId}", ex);
            return BadRequest(ApiResponse<object>.Fail($"匯出失敗資料失敗: {ex.Message}", "EXPORT_ERROR"));
        }
    }
}

