using ErpCore.Domain.Entities.Query;

namespace ErpCore.Infrastructure.Repositories.Query;

/// <summary>
/// 零用金參數 Repository 介面 (SYSQ110)
/// </summary>
public interface ICashParamsRepository
{
    Task<CashParams?> GetByIdAsync(long tKey);
    Task<IEnumerable<CashParams>> GetAllAsync();
    Task<CashParams> CreateAsync(CashParams entity);
    Task<CashParams> UpdateAsync(CashParams entity);
    Task DeleteAsync(long tKey);
}

