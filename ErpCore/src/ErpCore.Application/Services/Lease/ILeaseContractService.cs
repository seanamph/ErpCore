using ErpCore.Application.DTOs.Lease;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Lease;

/// <summary>
/// 租賃合同資料服務介面 (SYSM111-SYSM138)
/// </summary>
public interface ILeaseContractService
{
    Task<PagedResult<LeaseContractDto>> GetLeaseContractsAsync(LeaseContractQueryDto query);
    Task<LeaseContractDto> GetLeaseContractByIdAsync(string contractNo);
    Task<IEnumerable<LeaseContractDto>> GetLeaseContractsByLeaseIdAsync(string leaseId);
    Task<LeaseContractDto> CreateLeaseContractAsync(CreateLeaseContractDto dto);
    Task UpdateLeaseContractAsync(string contractNo, UpdateLeaseContractDto dto);
    Task DeleteLeaseContractAsync(string contractNo);
    Task UpdateLeaseContractStatusAsync(string contractNo, string status);
    Task<bool> ExistsAsync(string contractNo);
}

