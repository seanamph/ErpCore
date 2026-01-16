using ErpCore.Domain.Entities.DropdownList;
using ErpCore.Infrastructure.Repositories.Queries;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.DropdownList;

/// <summary>
/// 城市 Repository 介面
/// </summary>
public interface ICityRepository
{
    Task<City?> GetByIdAsync(string cityId);
    Task<PagedResult<City>> QueryAsync(CityQuery query);
    Task<IEnumerable<CityOption>> GetOptionsAsync(string? countryCode = null, string? status = "1");
}

