using ErpCore.Domain.Entities.DropdownList;
using ErpCore.Infrastructure.Repositories.Queries;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.DropdownList;

/// <summary>
/// 區域 Repository 介面
/// </summary>
public interface IZoneRepository
{
    Task<Zone?> GetByIdAsync(string zoneId);
    Task<PagedResult<Zone>> QueryAsync(ZoneQuery query);
    Task<IEnumerable<ZoneOption>> GetOptionsAsync(string? cityId = null, string? status = "1");
    Task<IEnumerable<Zone>> GetByCityIdAsync(string cityId);
    Task<bool> CreateAsync(Zone zone);
    Task<bool> UpdateAsync(Zone zone);
    Task<bool> DeleteAsync(string zoneId);
}

