using ErpCore.Application.DTOs.Lease;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Lease;

/// <summary>
/// 租賃處理服務介面 (SYS8B50-SYS8B90)
/// </summary>
public interface ILeaseProcessService
{
    Task<PagedResult<LeaseProcessDto>> GetLeaseProcessesAsync(LeaseProcessQueryDto query);
    Task<LeaseProcessDto> GetLeaseProcessByIdAsync(string processId);
    Task<List<LeaseProcessDto>> GetLeaseProcessesByLeaseIdAsync(string leaseId);
    Task<string> CreateLeaseProcessAsync(CreateLeaseProcessDto dto);
    Task UpdateLeaseProcessAsync(string processId, UpdateLeaseProcessDto dto);
    Task DeleteLeaseProcessAsync(string processId);
    Task UpdateLeaseProcessStatusAsync(string processId, UpdateLeaseProcessStatusDto dto);
    Task ExecuteLeaseProcessAsync(string processId, ExecuteLeaseProcessDto dto);
    Task ApproveLeaseProcessAsync(string processId, ApproveLeaseProcessDto dto);
    Task<bool> ExistsAsync(string processId);
}

