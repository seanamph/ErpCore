using ErpCore.Application.DTOs.Energy;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Energy;

/// <summary>
/// 能源基礎服務介面 (SYSO100-SYSO130 - 能源基礎功能)
/// </summary>
public interface IEnergyBaseService
{
    Task<PagedResult<EnergyBaseDto>> GetEnergyBasesAsync(EnergyBaseQueryDto query);
    Task<EnergyBaseDto?> GetEnergyBaseByIdAsync(long tKey);
    Task<EnergyBaseDto?> GetEnergyBaseByEnergyIdAsync(string energyId);
    Task<long> CreateEnergyBaseAsync(CreateEnergyBaseDto dto);
    Task UpdateEnergyBaseAsync(long tKey, UpdateEnergyBaseDto dto);
    Task DeleteEnergyBaseAsync(long tKey);
}

