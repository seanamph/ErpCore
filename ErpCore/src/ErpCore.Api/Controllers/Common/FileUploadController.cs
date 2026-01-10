using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Tools;
using ErpCore.Application.Services.Tools;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Common;

/// <summary>
/// 通用檔案上傳控制器
/// 提供統一的檔案上傳功能
/// </summary>
[Route("api/v1/common/files")]
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
}

