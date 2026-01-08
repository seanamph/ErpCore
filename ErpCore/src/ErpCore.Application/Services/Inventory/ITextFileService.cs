using ErpCore.Application.DTOs.Inventory;
using ErpCore.Shared.Common;
using Microsoft.AspNetCore.Http;

namespace ErpCore.Application.Services.Inventory;

/// <summary>
/// 文本文件處理服務介面
/// </summary>
public interface ITextFileService
{
    /// <summary>
    /// 上傳並處理文本文件
    /// </summary>
    Task<TextFileProcessLogDto> UploadAndProcessFileAsync(IFormFile file, string fileType, string? shopId);

    /// <summary>
    /// 查詢處理記錄列表
    /// </summary>
    Task<PagedResult<TextFileProcessLogDto>> GetProcessLogsAsync(TextFileProcessLogQueryDto query);

    /// <summary>
    /// 根據ID查詢處理記錄
    /// </summary>
    Task<TextFileProcessLogDto?> GetProcessLogByIdAsync(Guid logId);

    /// <summary>
    /// 查詢處理明細列表
    /// </summary>
    Task<PagedResult<TextFileProcessDetailDto>> GetProcessDetailsAsync(Guid logId, TextFileProcessDetailQueryDto query);

    /// <summary>
    /// 重新處理文件
    /// </summary>
    Task ReprocessFileAsync(Guid logId);

    /// <summary>
    /// 刪除處理記錄
    /// </summary>
    Task DeleteProcessLogAsync(Guid logId);

    /// <summary>
    /// 下載處理結果（Excel或CSV格式）
    /// </summary>
    Task<byte[]> DownloadProcessResultAsync(Guid logId, string format);
}

