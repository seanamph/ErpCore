using ErpCore.Application.DTOs.DropdownList;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.DropdownList;

/// <summary>
/// 城市服務介面
/// </summary>
public interface ICityService
{
    Task<CityDto?> GetCityAsync(string cityId);
    Task<PagedResult<CityDto>> GetCitiesAsync(CityQueryDto query);
    Task<IEnumerable<CityOptionDto>> GetCityOptionsAsync(string? countryCode = null, string? status = "1");
}

