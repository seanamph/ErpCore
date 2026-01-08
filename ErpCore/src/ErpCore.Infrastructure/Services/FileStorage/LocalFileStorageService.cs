using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Services.FileStorage;

/// <summary>
/// 本地檔案儲存服務實作
/// 將檔案儲存到本地檔案系統
/// </summary>
public class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;
    private readonly ILoggerService _logger;

    public LocalFileStorageService(ILoggerService logger)
    {
        _logger = logger;
        // 設定基礎路徑（可在 appsettings.json 中設定）
        _basePath = Path.Combine(Directory.GetCurrentDirectory(), "Files", "BusinessReports");
        
        // 確保目錄存在
        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
            _logger.LogInfo($"建立檔案儲存目錄: {_basePath}");
        }
    }

    public async Task<string> SaveFileAsync(byte[] fileBytes, string fileName, string? subDirectory = null)
    {
        try
        {
            _logger.LogInfo($"儲存檔案: {fileName}, 大小: {fileBytes.Length} bytes");

            // 建立檔案路徑
            var targetDirectory = string.IsNullOrEmpty(subDirectory)
                ? _basePath
                : Path.Combine(_basePath, subDirectory);

            // 確保子目錄存在
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            // 產生唯一檔案名稱（避免檔案名稱衝突）
            var uniqueFileName = $"{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid():N}_{fileName}";
            var fullPath = Path.Combine(targetDirectory, uniqueFileName);
            var relativePath = string.IsNullOrEmpty(subDirectory)
                ? uniqueFileName
                : Path.Combine(subDirectory, uniqueFileName).Replace('\\', '/');

            // 儲存檔案
            await File.WriteAllBytesAsync(fullPath, fileBytes);

            _logger.LogInfo($"檔案儲存成功: {relativePath}");
            return relativePath;
        }
        catch (Exception ex)
        {
            _logger.LogError($"儲存檔案失敗: {fileName}", ex);
            throw;
        }
    }

    public async Task<byte[]> ReadFileAsync(string filePath)
    {
        try
        {
            var fullPath = GetFullPath(filePath);
            
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"檔案不存在: {filePath}");
            }

            _logger.LogInfo($"讀取檔案: {filePath}");
            return await File.ReadAllBytesAsync(fullPath);
        }
        catch (Exception ex)
        {
            _logger.LogError($"讀取檔案失敗: {filePath}", ex);
            throw;
        }
    }

    public async Task<bool> DeleteFileAsync(string filePath)
    {
        try
        {
            var fullPath = GetFullPath(filePath);
            
            if (!File.Exists(fullPath))
            {
                _logger.LogWarning($"檔案不存在，無法刪除: {filePath}");
                return false;
            }

            File.Delete(fullPath);
            _logger.LogInfo($"檔案刪除成功: {filePath}");
            return await Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除檔案失敗: {filePath}", ex);
            throw;
        }
    }

    public Task<bool> FileExistsAsync(string filePath)
    {
        try
        {
            var fullPath = GetFullPath(filePath);
            return Task.FromResult(File.Exists(fullPath));
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查檔案是否存在失敗: {filePath}", ex);
            throw;
        }
    }

    public string GetFullPath(string filePath)
    {
        // 如果已經是完整路徑，直接返回
        if (Path.IsPathRooted(filePath))
        {
            return filePath;
        }

        // 將相對路徑轉換為完整路徑
        return Path.Combine(_basePath, filePath.Replace('/', Path.DirectorySeparatorChar));
    }
}

