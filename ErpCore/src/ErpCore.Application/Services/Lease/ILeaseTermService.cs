using ErpCore.Application.DTOs.Lease;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Lease;

/// <summary>
/// 租賃條件服務介面 (SYSE110-SYSE140)
/// </summary>
public interface ILeaseTermService
{
    Task<PagedResult<LeaseTermDto>> GetLeaseTermsAsync(LeaseTermQueryDto query);
    Task<LeaseTermDto> GetLeaseTermByIdAsync(long tKey);
    Task<IEnumerable<LeaseTermDto>> GetLeaseTermsByLeaseIdAndVersionAsync(string leaseId, string version);
    Task<LeaseTermDto> CreateLeaseTermAsync(CreateLeaseTermDto dto);
    Task UpdateLeaseTermAsync(long tKey, UpdateLeaseTermDto dto);
    Task DeleteLeaseTermAsync(long tKey);
    Task DeleteLeaseTermsByLeaseIdAndVersionAsync(string leaseId, string version);
    Task<bool> ExistsAsync(long tKey);
}

