using ErpCore.Domain.Entities.OtherManagement;

namespace ErpCore.Infrastructure.Repositories.OtherManagement;

/// <summary>
/// J系統功能 Repository 介面 (SYSJ000)
/// </summary>
public interface ISYSJFunctionRepository
{
    Task<SYSJFunction?> GetByIdAsync(long tKey);
    Task<SYSJFunction?> GetByFunctionIdAsync(string functionId);
    Task<IEnumerable<SYSJFunction>> QueryAsync(SYSJFunctionQuery query);
    Task<int> GetCountAsync(SYSJFunctionQuery query);
    Task<long> CreateAsync(SYSJFunction entity);
    Task UpdateAsync(SYSJFunction entity);
    Task DeleteAsync(long tKey);
    Task<bool> ExistsAsync(string functionId);
}

