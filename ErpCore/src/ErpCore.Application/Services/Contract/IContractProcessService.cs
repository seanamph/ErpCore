using ErpCore.Application.DTOs.Contract;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Contract;

/// <summary>
/// 合同處理服務介面 (SYSF210-SYSF220)
/// </summary>
public interface IContractProcessService
{
    Task<PagedResult<ContractProcessDto>> GetContractProcessesAsync(ContractProcessQueryDto query);
    Task<ContractProcessDto> GetContractProcessByIdAsync(string processId);
    Task<ContractProcessResultDto> CreateContractProcessAsync(CreateContractProcessDto dto);
    Task UpdateContractProcessAsync(string processId, UpdateContractProcessDto dto);
    Task DeleteContractProcessAsync(string processId);
    Task CompleteContractProcessAsync(string processId);
    Task CancelContractProcessAsync(string processId);
    Task<bool> ExistsAsync(string processId);
}

