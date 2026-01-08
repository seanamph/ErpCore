using ErpCore.Application.DTOs.Tools;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Tools;
using ErpCore.Infrastructure.Repositories.Tools;
using ErpCore.Infrastructure.Services.FileStorage;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using Microsoft.AspNetCore.Http;

namespace ErpCore.Application.Services.Tools;

/// <summary>
/// 檔案上傳服務實作
/// </summary>
public class FileUploadService : BaseService, IFileUploadService
{
    private readonly IFileUploadRepository _repository;
    private readonly IFileStorageService _fileStorageService;
    private const long MaxFileSize = 100 * 1024 * 1024; // 100MB
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".csv" };

    public FileUploadService(
        IFileUploadRepository repository,
        IFileStorageService fileStorageService,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _fileStorageService = fileStorageService;
    }

    public async Task<FileUploadResponseDto> UploadFileAsync(IFormFile file, FileUploadRequestDto? request = null)
    {
        try
        {
            _logger.LogInfo($"開始上傳檔案: {file.FileName}");

            // 驗證檔案
            ValidateFile(file);

            // 讀取檔案內容
            byte[] fileBytes;
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                fileBytes = memoryStream.ToArray();
            }

            // 取得副檔名
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var fileType = file.ContentType;

            // 產生唯一檔案名稱
            var uniqueFileName = $"{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid():N}{fileExtension}";

            // 儲存檔案
            var subDirectory = request?.UploadPath ?? "uploads";
            var filePath = await _fileStorageService.SaveFileAsync(fileBytes, uniqueFileName, subDirectory);

            // 建立檔案上傳記錄
            var entity = new FileUpload
            {
                FileName = uniqueFileName,
                OriginalFileName = file.FileName,
                FilePath = filePath,
                FileSize = fileBytes.Length,
                FileType = fileType,
                FileExtension = fileExtension,
                UploadPath = subDirectory,
                UploadedBy = GetCurrentUserId(),
                UploadedAt = DateTime.Now,
                Status = "1",
                RelatedTable = request?.RelatedTable,
                RelatedId = request?.RelatedId,
                Description = request?.Description
            };

            var savedEntity = await _repository.CreateAsync(entity);

            _logger.LogInfo($"檔案上傳成功: {savedEntity.Id}");

            return new FileUploadResponseDto
            {
                Id = savedEntity.Id,
                FileName = savedEntity.FileName,
                OriginalFileName = savedEntity.OriginalFileName,
                FilePath = savedEntity.FilePath,
                FileSize = savedEntity.FileSize,
                FileType = savedEntity.FileType,
                FileExtension = savedEntity.FileExtension,
                UploadPath = savedEntity.UploadPath,
                UploadedAt = savedEntity.UploadedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"上傳檔案失敗: {file.FileName}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<FileUploadResponseDto>> UploadFilesAsync(IEnumerable<IFormFile> files, FileUploadRequestDto? request = null)
    {
        try
        {
            var results = new List<FileUploadResponseDto>();
            foreach (var file in files)
            {
                var result = await UploadFileAsync(file, request);
                results.Add(result);
            }
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError("批次上傳檔案失敗", ex);
            throw;
        }
    }

    public async Task<FileUploadDto?> GetFileUploadAsync(long id)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                return null;
            }

            return new FileUploadDto
            {
                Id = entity.Id,
                FileName = entity.FileName,
                OriginalFileName = entity.OriginalFileName,
                FilePath = entity.FilePath,
                FileSize = entity.FileSize,
                FileType = entity.FileType,
                FileExtension = entity.FileExtension,
                UploadPath = entity.UploadPath,
                UploadedBy = entity.UploadedBy,
                UploadedAt = entity.UploadedAt,
                Status = entity.Status,
                RelatedTable = entity.RelatedTable,
                RelatedId = entity.RelatedId,
                Description = entity.Description
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢檔案上傳記錄失敗: {id}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<FileUploadDto>> GetFileUploadsAsync(FileUploadQueryDto query)
    {
        try
        {
            var entities = await _repository.GetListAsync(
                query.UploadedBy,
                query.RelatedTable,
                query.RelatedId,
                query.Status);

            return entities.Select(e => new FileUploadDto
            {
                Id = e.Id,
                FileName = e.FileName,
                OriginalFileName = e.OriginalFileName,
                FilePath = e.FilePath,
                FileSize = e.FileSize,
                FileType = e.FileType,
                FileExtension = e.FileExtension,
                UploadPath = e.UploadPath,
                UploadedBy = e.UploadedBy,
                UploadedAt = e.UploadedAt,
                Status = e.Status,
                RelatedTable = e.RelatedTable,
                RelatedId = e.RelatedId,
                Description = e.Description
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢檔案上傳記錄列表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]?> DownloadFileAsync(long id)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null || entity.Status != "1")
            {
                return null;
            }

            var fileBytes = await _fileStorageService.ReadFileAsync(entity.FilePath);
            return fileBytes;
        }
        catch (Exception ex)
        {
            _logger.LogError($"下載檔案失敗: {id}", ex);
            throw;
        }
    }

    public async Task<bool> DeleteFileUploadAsync(long id)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                return false;
            }

            // 刪除檔案
            await _fileStorageService.DeleteFileAsync(entity.FilePath);

            // 軟刪除記錄
            return await _repository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除檔案上傳記錄失敗: {id}", ex);
            throw;
        }
    }

    private void ValidateFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("檔案不能為空");
        }

        if (file.Length > MaxFileSize)
        {
            throw new ArgumentException($"檔案大小不能超過 {MaxFileSize / 1024 / 1024}MB");
        }

        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(fileExtension))
        {
            throw new ArgumentException($"不支援的檔案類型: {fileExtension}");
        }
    }
}

