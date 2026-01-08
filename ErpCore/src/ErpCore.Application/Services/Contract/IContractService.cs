using ErpCore.Application.DTOs.Contract;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Contract;

/// <summary>
/// 合同服務介面 (SYSF110-SYSF140)
/// </summary>
public interface IContractService
{
    Task<PagedResult<ContractDto>> GetContractsAsync(ContractQueryDto query);
    Task<ContractDto> GetContractByIdAsync(string contractId, int version);
    Task<ContractResultDto> CreateContractAsync(CreateContractDto dto);
    Task UpdateContractAsync(string contractId, int version, UpdateContractDto dto);
    Task DeleteContractAsync(string contractId, int version);
    Task ApproveContractAsync(string contractId, int version, ApproveContractDto dto);
    Task<ContractResultDto> CreateNewVersionAsync(string contractId, int version, NewVersionDto dto);
    Task<bool> ExistsAsync(string contractId, int version);
}

