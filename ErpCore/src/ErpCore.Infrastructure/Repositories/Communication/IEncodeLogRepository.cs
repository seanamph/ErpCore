using ErpCore.Domain.Entities.Communication;

namespace ErpCore.Infrastructure.Repositories.Communication;

/// <summary>
/// 編碼記錄儲存庫介面
/// </summary>
public interface IEncodeLogRepository
{
    /// <summary>
    /// 建立編碼記錄
    /// </summary>
    Task<EncodeLog> CreateAsync(EncodeLog entity);

    /// <summary>
    /// 根據ID查詢編碼記錄
    /// </summary>
    Task<EncodeLog?> GetByIdAsync(long id);
}

