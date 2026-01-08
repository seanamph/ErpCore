using ErpCore.Domain.Entities.OtherManagement;

namespace ErpCore.Infrastructure.Repositories.OtherManagement;

/// <summary>
/// S系統功能 Repository 介面 (SYSS000)
/// </summary>
public interface ISYSSFunctionRepository
{
    Task<SYSSFunction?> GetByIdAsync(long tKey);
    Task<SYSSFunction?> GetByFunctionIdAsync(string functionId);
    Task<IEnumerable<SYSSFunction>> QueryAsync(SYSSFunctionQuery query);
    Task<int> GetCountAsync(SYSSFunctionQuery query);
    Task<long> CreateAsync(SYSSFunction entity);
    Task UpdateAsync(SYSSFunction entity);
    Task DeleteAsync(long tKey);
    Task<bool> ExistsAsync(string functionId);
}

