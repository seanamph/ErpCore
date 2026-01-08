using ErpCore.Domain.Entities.Extension;

namespace ErpCore.Infrastructure.Repositories.Extension;

/// <summary>
/// 擴展功能 Repository 介面
/// </summary>
public interface IExtensionFunctionRepository
{
    Task<ExtensionFunction?> GetByIdAsync(long tKey);
    Task<ExtensionFunction?> GetByExtensionIdAsync(string extensionId);
    Task<IEnumerable<ExtensionFunction>> QueryAsync(ExtensionFunctionQuery query);
    Task<int> GetCountAsync(ExtensionFunctionQuery query);
    Task<long> CreateAsync(ExtensionFunction entity);
    Task UpdateAsync(ExtensionFunction entity);
    Task DeleteAsync(long tKey);
    Task<bool> ExistsAsync(string extensionId);
    Task BatchCreateAsync(IEnumerable<ExtensionFunction> entities);
}

