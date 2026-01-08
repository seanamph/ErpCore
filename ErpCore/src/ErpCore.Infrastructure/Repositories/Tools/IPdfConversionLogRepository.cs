using ErpCore.Domain.Entities.Tools;

namespace ErpCore.Infrastructure.Repositories.Tools;

/// <summary>
/// PDF轉換記錄儲存庫介面
/// </summary>
public interface IPdfConversionLogRepository
{
    /// <summary>
    /// 建立PDF轉換記錄
    /// </summary>
    Task<PdfConversionLog> CreateAsync(PdfConversionLog entity);

    /// <summary>
    /// 根據ID查詢PDF轉換記錄
    /// </summary>
    Task<PdfConversionLog?> GetByIdAsync(Guid id);

    /// <summary>
    /// 更新PDF轉換記錄
    /// </summary>
    Task<bool> UpdateAsync(PdfConversionLog entity);
}

