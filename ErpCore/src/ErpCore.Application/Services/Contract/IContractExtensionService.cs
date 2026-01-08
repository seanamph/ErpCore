using ErpCore.Application.DTOs.Contract;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Contract;

/// <summary>
/// 合同擴展服務介面 (SYSF350-SYSF540)
/// </summary>
public interface IContractExtensionService
{
    Task<PagedResult<ContractExtensionDto>> GetContractExtensionsAsync(ContractExtensionQueryDto query);
    Task<ContractExtensionDto> GetContractExtensionByIdAsync(long tKey);
    Task<ContractExtensionResultDto> CreateContractExtensionAsync(CreateContractExtensionDto dto);
    Task UpdateContractExtensionAsync(long tKey, UpdateContractExtensionDto dto);
    Task DeleteContractExtensionAsync(long tKey);
    Task<int> BatchDeleteContractExtensionsAsync(List<long> tKeys);
    Task<bool> ExistsAsync(long tKey);
}

