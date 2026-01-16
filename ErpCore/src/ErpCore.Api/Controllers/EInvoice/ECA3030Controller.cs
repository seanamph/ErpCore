using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.EInvoice;
using ErpCore.Application.Services.EInvoice;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.EInvoice;

/// <summary>
/// 電子發票上傳作業控制器 (ECA3030)
/// </summary>
[Route("api/v1/einvoices/eca3030")]
public class ECA3030Controller : BaseController
{
    private readonly IEInvoiceService _service;
    private const string UploadType = "ECA3030";

    public ECA3030Controller(
        IEInvoiceService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 上傳電子發票檔案 (ECA3030)
    /// </summary>
    [HttpPost("upload")]
    public async Task<ActionResult<ApiResponse<EInvoiceUploadDto>>> UploadFile(
        IFormFile file,
        [FromForm] string? storeId = null,
        [FromForm] string? retailerId = null,
        [FromForm] string? description = null)
    {
        return await ExecuteAsync(async () =>
        {
            // 自動設定 uploadType = "ECA3030"
            var result = await _service.UploadFileAsync(file, storeId, retailerId, UploadType);
            return result;
        }, "上傳電子發票檔案失敗");
    }

    /// <summary>
    /// 查詢上傳記錄列表 (ECA3030)
    /// </summary>
    [HttpGet("uploads")]
    public async Task<ActionResult<ApiResponse<PagedResult<EInvoiceUploadDto>>>> GetUploads(
        [FromQuery] EInvoiceUploadQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            // 自動設定和過濾 uploadType = "ECA3030"
            query.UploadType = UploadType;
            var result = await _service.GetUploadsAsync(query);
            return result;
        }, "查詢上傳記錄列表失敗");
    }

    /// <summary>
    /// 查詢單筆上傳記錄 (ECA3030)
    /// </summary>
    [HttpGet("uploads/{uploadId}")]
    public async Task<ActionResult<ApiResponse<EInvoiceUploadDto>>> GetUpload(long uploadId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetUploadAsync(uploadId);
            // 驗證是否為 ECA3030 類型
            if (result.UploadType != UploadType)
            {
                throw new InvalidOperationException($"上傳記錄類型不正確: {result.UploadType}，預期: {UploadType}");
            }
            return result;
        }, $"查詢上傳記錄失敗: {uploadId}");
    }

    /// <summary>
    /// 查詢處理狀態 (ECA3030)
    /// </summary>
    [HttpGet("uploads/{uploadId}/status")]
    public async Task<ActionResult<ApiResponse<EInvoiceProcessStatusDto>>> GetProcessStatus(long uploadId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetProcessStatusAsync(uploadId);
            return result;
        }, $"查詢處理狀態失敗: {uploadId}");
    }

    /// <summary>
    /// 開始處理上傳檔案 (ECA3030)
    /// </summary>
    [HttpPost("uploads/{uploadId}/process")]
    public async Task<ActionResult<ApiResponse<object>>> StartProcess(long uploadId)
    {
        return await ExecuteAsync(async () =>
        {
            // 驗證上傳記錄類型
            var upload = await _service.GetUploadAsync(uploadId);
            if (upload.UploadType != UploadType)
            {
                throw new InvalidOperationException($"上傳記錄類型不正確: {upload.UploadType}，預期: {UploadType}");
            }
            
            await _service.StartProcessAsync(uploadId);
        }, $"開始處理上傳檔案失敗: {uploadId}");
    }

    /// <summary>
    /// 刪除上傳記錄 (ECA3030)
    /// </summary>
    [HttpDelete("uploads/{uploadId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteUpload(long uploadId)
    {
        return await ExecuteAsync(async () =>
        {
            // 驗證上傳記錄類型
            var upload = await _service.GetUploadAsync(uploadId);
            if (upload.UploadType != UploadType)
            {
                throw new InvalidOperationException($"上傳記錄類型不正確: {upload.UploadType}，預期: {UploadType}");
            }
            
            await _service.DeleteUploadAsync(uploadId);
        }, $"刪除上傳記錄失敗: {uploadId}");
    }

    /// <summary>
    /// 下載上傳檔案 (ECA3030)
    /// </summary>
    [HttpGet("uploads/{uploadId}/download")]
    public async Task<IActionResult> DownloadUpload(long uploadId)
    {
        try
        {
            // 驗證上傳記錄類型
            var upload = await _service.GetUploadAsync(uploadId);
            if (upload.UploadType != UploadType)
            {
                return BadRequest(ApiResponse<object>.Fail($"上傳記錄類型不正確: {upload.UploadType}，預期: {UploadType}"));
            }
            
            var (fileBytes, fileName, contentType) = await _service.DownloadUploadFileAsync(uploadId);
            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError($"下載上傳檔案失敗: {uploadId}", ex);
            return BadRequest(ApiResponse<object>.Fail($"下載上傳檔案失敗: {ex.Message}"));
        }
    }
}
