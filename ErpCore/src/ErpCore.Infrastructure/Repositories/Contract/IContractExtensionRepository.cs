using ErpCore.Domain.Entities.Contract;

namespace ErpCore.Infrastructure.Repositories.Contract;

/// <summary>
/// 合同擴展 Repository 介面 (SYSF350-SYSF540)
/// </summary>
public interface IContractExtensionRepository
{
    Task<ContractExtension?> GetByIdAsync(long tKey);
    Task<IEnumerable<ContractExtension>> QueryAsync(ContractExtensionQuery query);
    Task<int> GetCountAsync(ContractExtensionQuery query);
    Task<bool> ExistsAsync(long tKey);
    Task<ContractExtension> CreateAsync(ContractExtension extension);
    Task<ContractExtension> UpdateAsync(ContractExtension extension);
    Task DeleteAsync(long tKey);
    Task<int> BatchDeleteAsync(List<long> tKeys);
}

/// <summary>
/// 合同擴展查詢條件
/// </summary>
public class ContractExtensionQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ContractId { get; set; }
    public string? VendorId { get; set; }
    public string? ExtensionType { get; set; }
    public string? Status { get; set; }
}

