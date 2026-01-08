using ErpCore.Domain.Entities.OtherManagement;

namespace ErpCore.Infrastructure.Repositories.OtherManagement;

/// <summary>
/// V系統功能 Repository 介面 (SYSV000)
/// </summary>
public interface ISYSVFunctionRepository
{
    Task<SYSVFunction?> GetByIdAsync(long tKey);
    Task<SYSVFunction?> GetByFunctionIdAsync(string functionId);
    Task<IEnumerable<SYSVFunction>> QueryAsync(SYSVFunctionQuery query);
    Task<int> GetCountAsync(SYSVFunctionQuery query);
    Task<long> CreateAsync(SYSVFunction entity);
    Task UpdateAsync(SYSVFunction entity);
    Task DeleteAsync(long tKey);
    Task<bool> ExistsAsync(string functionId);
}

