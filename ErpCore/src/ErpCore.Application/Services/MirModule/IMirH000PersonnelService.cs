using ErpCore.Application.DTOs.MirModule;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.MirModule;

/// <summary>
/// MIRH000 人事服務介面
/// </summary>
public interface IMirH000PersonnelService
{
    Task<PagedResult<MirH000PersonnelDto>> GetPersonnelListAsync(MirH000PersonnelQueryDto query);
    Task<MirH000PersonnelDto> GetPersonnelByIdAsync(string personnelId);
    Task<string> CreatePersonnelAsync(CreateMirH000PersonnelDto dto);
    Task UpdatePersonnelAsync(string personnelId, UpdateMirH000PersonnelDto dto);
    Task DeletePersonnelAsync(string personnelId);
}

