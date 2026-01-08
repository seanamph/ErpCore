using ErpCore.Application.DTOs.Lease;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Lease;

/// <summary>
/// 停車位資料服務介面 (SYSM111-SYSM138)
/// </summary>
public interface IParkingSpaceService
{
    Task<PagedResult<ParkingSpaceDto>> GetParkingSpacesAsync(ParkingSpaceQueryDto query);
    Task<ParkingSpaceDto> GetParkingSpaceByIdAsync(string parkingSpaceId);
    Task<IEnumerable<ParkingSpaceDto>> GetAvailableParkingSpacesAsync(string? shopId);
    Task<ParkingSpaceDto> CreateParkingSpaceAsync(CreateParkingSpaceDto dto);
    Task UpdateParkingSpaceAsync(string parkingSpaceId, UpdateParkingSpaceDto dto);
    Task DeleteParkingSpaceAsync(string parkingSpaceId);
    Task UpdateParkingSpaceStatusAsync(string parkingSpaceId, string status);
    Task<bool> ExistsAsync(string parkingSpaceId);
}

