using ErpCore.Application.DTOs.DropdownList;
using ErpCore.Domain.Entities.DropdownList;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.DropdownList;

/// <summary>
/// 區域 Repository 介面
/// </summary>
public interface IZoneRepository
{
    Task<Zone?> GetByIdAsync(string zoneId);
    Task<PagedResult<Zone>> QueryAsync(ZoneQueryDto query);
    Task<IEnumerable<ZoneOptionDto>> GetOptionsAsync(string? cityId = null, string? status = "1");
}

