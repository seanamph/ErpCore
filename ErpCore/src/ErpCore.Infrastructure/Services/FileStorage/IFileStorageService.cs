namespace ErpCore.Infrastructure.Services.FileStorage;

/// <summary>
/// 檔案儲存服務介面
/// 提供檔案儲存、讀取、刪除等功能
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// 儲存檔案
    /// </summary>
    /// <param name="fileBytes">檔案位元組陣列</param>
    /// <param name="fileName">檔案名稱</param>
    /// <param name="subDirectory">子目錄（可選）</param>
    /// <returns>檔案相對路徑</returns>
    Task<string> SaveFileAsync(byte[] fileBytes, string fileName, string? subDirectory = null);

    /// <summary>
    /// 讀取檔案
    /// </summary>
    /// <param name="filePath">檔案相對路徑</param>
    /// <returns>檔案位元組陣列</returns>
    Task<byte[]> ReadFileAsync(string filePath);

    /// <summary>
    /// 刪除檔案
    /// </summary>
    /// <param name="filePath">檔案相對路徑</param>
    /// <returns>是否刪除成功</returns>
    Task<bool> DeleteFileAsync(string filePath);

    /// <summary>
    /// 檢查檔案是否存在
    /// </summary>
    /// <param name="filePath">檔案相對路徑</param>
    /// <returns>檔案是否存在</returns>
    Task<bool> FileExistsAsync(string filePath);

    /// <summary>
    /// 取得檔案完整路徑
    /// </summary>
    /// <param name="filePath">檔案相對路徑</param>
    /// <returns>檔案完整路徑</returns>
    string GetFullPath(string filePath);
}

