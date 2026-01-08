using ErpCore.Application.DTOs.DropdownList;
using ErpCore.Domain.Entities.DropdownList;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.DropdownList;

/// <summary>
/// 城市 Repository 介面
/// </summary>
public interface ICityRepository
{
    Task<City?> GetByIdAsync(string cityId);
    Task<PagedResult<City>> QueryAsync(CityQueryDto query);
    Task<IEnumerable<CityOptionDto>> GetOptionsAsync(string? countryCode = null, string? status = "1");
}

