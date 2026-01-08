using ErpCore.Application.DTOs.Lease;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Lease;

/// <summary>
/// 租賃擴展服務介面 (SYS8A10-SYS8A45)
/// </summary>
public interface ILeaseExtensionService
{
    Task<PagedResult<LeaseExtensionDto>> GetLeaseExtensionsAsync(LeaseExtensionQueryDto query);
    Task<LeaseExtensionDto> GetLeaseExtensionByIdAsync(string extensionId);
    Task<List<LeaseExtensionDto>> GetLeaseExtensionsByLeaseIdAsync(string leaseId);
    Task<string> CreateLeaseExtensionAsync(CreateLeaseExtensionDto dto);
    Task UpdateLeaseExtensionAsync(string extensionId, UpdateLeaseExtensionDto dto);
    Task DeleteLeaseExtensionAsync(string extensionId);
    Task BatchDeleteLeaseExtensionsAsync(BatchDeleteLeaseExtensionDto dto);
    Task UpdateLeaseExtensionStatusAsync(string extensionId, UpdateLeaseExtensionStatusDto dto);
    Task<bool> ExistsAsync(string extensionId);
}

