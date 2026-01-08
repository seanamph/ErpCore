using ErpCore.Domain.Entities.OtherManagement;

namespace ErpCore.Infrastructure.Repositories.OtherManagement;

/// <summary>
/// U系統功能 Repository 介面 (SYSU000)
/// </summary>
public interface ISYSUFunctionRepository
{
    Task<SYSUFunction?> GetByIdAsync(long tKey);
    Task<SYSUFunction?> GetByFunctionIdAsync(string functionId);
    Task<IEnumerable<SYSUFunction>> QueryAsync(SYSUFunctionQuery query);
    Task<int> GetCountAsync(SYSUFunctionQuery query);
    Task<long> CreateAsync(SYSUFunction entity);
    Task UpdateAsync(SYSUFunction entity);
    Task DeleteAsync(long tKey);
    Task<bool> ExistsAsync(string functionId);
}

