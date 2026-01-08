using ErpCore.Domain.Entities.Tools;

namespace ErpCore.Infrastructure.Repositories.Tools;

/// <summary>
/// 檔案上傳儲存庫介面
/// </summary>
public interface IFileUploadRepository
{
    /// <summary>
    /// 建立檔案上傳記錄
    /// </summary>
    Task<FileUpload> CreateAsync(FileUpload entity);

    /// <summary>
    /// 根據ID查詢檔案上傳記錄
    /// </summary>
    Task<FileUpload?> GetByIdAsync(long id);

    /// <summary>
    /// 查詢檔案上傳記錄列表
    /// </summary>
    Task<IEnumerable<FileUpload>> GetListAsync(string? uploadedBy = null, string? relatedTable = null, string? relatedId = null, string? status = "1");

    /// <summary>
    /// 更新檔案上傳記錄
    /// </summary>
    Task<bool> UpdateAsync(FileUpload entity);

    /// <summary>
    /// 刪除檔案上傳記錄（軟刪除）
    /// </summary>
    Task<bool> DeleteAsync(long id);
}

