using ErpCore.Application.DTOs.DropdownList;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.DropdownList;

/// <summary>
/// 區域服務介面
/// </summary>
public interface IZoneService
{
    Task<ZoneDto?> GetZoneAsync(string zoneId);
    Task<PagedResult<ZoneDto>> GetZonesAsync(ZoneQueryDto query);
    Task<IEnumerable<ZoneOptionDto>> GetZoneOptionsAsync(string? cityId = null, string? status = "1");
}

