using ErpCore.Domain.Entities.Lease;

namespace ErpCore.Infrastructure.Repositories.Lease;

/// <summary>
/// 停車位資料 Repository 介面 (SYSM111-SYSM138)
/// </summary>
public interface IParkingSpaceRepository
{
    Task<ParkingSpace?> GetByIdAsync(string parkingSpaceId);
    Task<IEnumerable<ParkingSpace>> QueryAsync(ParkingSpaceQuery query);
    Task<int> GetCountAsync(ParkingSpaceQuery query);
    Task<bool> ExistsAsync(string parkingSpaceId);
    Task<ParkingSpace> CreateAsync(ParkingSpace parkingSpace);
    Task<ParkingSpace> UpdateAsync(ParkingSpace parkingSpace);
    Task DeleteAsync(string parkingSpaceId);
    Task UpdateStatusAsync(string parkingSpaceId, string status);
    Task<IEnumerable<ParkingSpace>> GetAvailableParkingSpacesAsync(string? shopId);
}

/// <summary>
/// 停車位查詢條件
/// </summary>
public class ParkingSpaceQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ParkingSpaceId { get; set; }
    public string? ShopId { get; set; }
    public string? FloorId { get; set; }
    public string? Status { get; set; }
    public string? LeaseId { get; set; }
}

