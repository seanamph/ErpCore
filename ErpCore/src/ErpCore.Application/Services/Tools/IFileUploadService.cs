using ErpCore.Application.DTOs.Tools;
using Microsoft.AspNetCore.Http;

namespace ErpCore.Application.Services.Tools;

/// <summary>
/// 檔案上傳服務介面
/// </summary>
public interface IFileUploadService
{
    /// <summary>
    /// 上傳單個檔案
    /// </summary>
    Task<FileUploadResponseDto> UploadFileAsync(IFormFile file, FileUploadRequestDto? request = null);

    /// <summary>
    /// 上傳多個檔案
    /// </summary>
    Task<IEnumerable<FileUploadResponseDto>> UploadFilesAsync(IEnumerable<IFormFile> files, FileUploadRequestDto? request = null);

    /// <summary>
    /// 根據ID查詢檔案上傳記錄
    /// </summary>
    Task<FileUploadDto?> GetFileUploadAsync(long id);

    /// <summary>
    /// 查詢檔案上傳記錄列表
    /// </summary>
    Task<IEnumerable<FileUploadDto>> GetFileUploadsAsync(FileUploadQueryDto query);

    /// <summary>
    /// 下載檔案
    /// </summary>
    Task<byte[]?> DownloadFileAsync(long id);

    /// <summary>
    /// 刪除檔案上傳記錄
    /// </summary>
    Task<bool> DeleteFileUploadAsync(long id);
}

