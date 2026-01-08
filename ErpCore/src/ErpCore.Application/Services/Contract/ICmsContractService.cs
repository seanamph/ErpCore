using ErpCore.Application.DTOs.Contract;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Contract;

/// <summary>
/// CMS合同服務介面 (CMS2310-CMS2320)
/// </summary>
public interface ICmsContractService
{
    Task<PagedResult<CmsContractDto>> GetCmsContractsAsync(CmsContractQueryDto query);
    Task<CmsContractDto> GetCmsContractByIdAsync(long tKey);
    Task<CmsContractDto> GetCmsContractByContractIdAsync(string cmsContractId, int version);
    Task<CmsContractResultDto> CreateCmsContractAsync(CreateCmsContractDto dto);
    Task UpdateCmsContractAsync(long tKey, UpdateCmsContractDto dto);
    Task DeleteCmsContractAsync(long tKey);
    Task<int> BatchDeleteCmsContractsAsync(List<long> tKeys);
    Task<bool> ExistsAsync(string cmsContractId, int version);
}

