using ErpCore.Domain.Entities.Contract;

namespace ErpCore.Infrastructure.Repositories.Contract;

/// <summary>
/// CMS合同 Repository 介面 (CMS2310-CMS2320)
/// </summary>
public interface ICmsContractRepository
{
    Task<CmsContract?> GetByIdAsync(long tKey);
    Task<CmsContract?> GetByContractIdAsync(string cmsContractId, int version);
    Task<IEnumerable<CmsContract>> QueryAsync(CmsContractQuery query);
    Task<int> GetCountAsync(CmsContractQuery query);
    Task<bool> ExistsAsync(string cmsContractId, int version);
    Task<CmsContract> CreateAsync(CmsContract contract);
    Task<CmsContract> UpdateAsync(CmsContract contract);
    Task DeleteAsync(long tKey);
    Task<int> BatchDeleteAsync(List<long> tKeys);
    Task<int> GetNextVersionAsync(string cmsContractId);
}

/// <summary>
/// CMS合同查詢條件
/// </summary>
public class CmsContractQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? CmsContractId { get; set; }
    public string? VendorId { get; set; }
    public string? ContractType { get; set; }
    public string? Status { get; set; }
}

