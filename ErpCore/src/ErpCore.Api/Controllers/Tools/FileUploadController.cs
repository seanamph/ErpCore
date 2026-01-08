using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Tools;
using ErpCore.Application.Services.Tools;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Tools;

/// <summary>
/// 檔案上傳工具控制器 (FILE_UPLOAD)
/// </summary>
[Route("api/v1/files")]
public class FileUploadController : BaseController
{
    private readonly IFileUploadService _service;

    public FileUploadController(
        IFileUploadService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 上傳單個檔案
    /// </summary>
    [HttpPost("upload")]
    public async Task<ActionResult<ApiResponse<FileUploadResponseDto>>> UploadFile(
        [FromForm] IFormFile file,
        [FromForm] string? uploadPath = null,
        [FromForm] string? relatedTable = null,
        [FromForm] string? relatedId = null,
        [FromForm] string? description = null)
    {
        return await ExecuteAsync(async () =>
        {
            var request = new FileUploadRequestDto
            {
                UploadPath = uploadPath,
                RelatedTable = relatedTable,
                RelatedId = relatedId,
                Description = description
            };

            var result = await _service.UploadFileAsync(file, request);
            return result;
        }, "上傳檔案失敗");
    }

    /// <summary>
    /// 上傳多個檔案
    /// </summary>
    [HttpPost("upload-multiple")]
    public async Task<ActionResult<ApiResponse<IEnumerable<FileUploadResponseDto>>>> UploadFiles(
        [FromForm] IEnumerable<IFormFile> files,
        [FromForm] string? uploadPath = null,
        [FromForm] string? relatedTable = null,
        [FromForm] string? relatedId = null,
        [FromForm] string? description = null)
    {
        return await ExecuteAsync(async () =>
        {
            var request = new FileUploadRequestDto
            {
                UploadPath = uploadPath,
                RelatedTable = relatedTable,
                RelatedId = relatedId,
                Description = description
            };

            var result = await _service.UploadFilesAsync(files, request);
            return result;
        }, "批次上傳檔案失敗");
    }

    /// <summary>
    /// 查詢檔案上傳記錄
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<FileUploadDto>>> GetFileUpload(long id)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetFileUploadAsync(id);
            if (result == null)
            {
                throw new Exception($"檔案上傳記錄不存在: {id}");
            }
            return result;
        }, $"查詢檔案上傳記錄失敗: {id}");
    }

    /// <summary>
    /// 查詢檔案上傳記錄列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<FileUploadDto>>>> GetFileUploads(
        [FromQuery] FileUploadQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetFileUploadsAsync(query);
            return result;
        }, "查詢檔案上傳記錄列表失敗");
    }

    /// <summary>
    /// 下載檔案
    /// </summary>
    [HttpGet("{id}/download")]
    public async Task<IActionResult> DownloadFile(long id)
    {
        try
        {
            var fileUpload = await _service.GetFileUploadAsync(id);
            if (fileUpload == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Code = 404,
                    Message = "檔案不存在"
                });
            }

            var fileBytes = await _service.DownloadFileAsync(id);
            if (fileBytes == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Code = 404,
                    Message = "檔案內容不存在"
                });
            }

            return File(fileBytes, fileUpload.FileType ?? "application/octet-stream", fileUpload.OriginalFileName);
        }
        catch (Exception ex)
        {
            _logger.LogError($"下載檔案失敗: {id}", ex);
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Code = 400,
                Message = $"下載檔案失敗: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// 刪除檔案上傳記錄
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteFileUpload(long id)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.DeleteFileUploadAsync(id);
            if (!result)
            {
                throw new Exception($"刪除檔案上傳記錄失敗: {id}");
            }
            return (object)null!;
        }, $"刪除檔案上傳記錄失敗: {id}");
    }
}

