using ErpCore.Application.DTOs.Lease;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Lease;

/// <summary>
/// 租賃服務介面 (SYS8110-SYS8220)
/// </summary>
public interface ILeaseService
{
    Task<PagedResult<LeaseDto>> GetLeasesAsync(LeaseQueryDto query);
    Task<LeaseDto> GetLeaseByIdAsync(string leaseId);
    Task<LeaseResultDto> CreateLeaseAsync(CreateLeaseDto dto);
    Task UpdateLeaseAsync(string leaseId, UpdateLeaseDto dto);
    Task DeleteLeaseAsync(string leaseId);
    Task BatchDeleteLeasesAsync(BatchDeleteLeaseDto dto);
    Task UpdateLeaseStatusAsync(string leaseId, UpdateLeaseStatusDto dto);
    Task<bool> ExistsAsync(string leaseId);
}

