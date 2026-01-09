using ErpCore.Application.DTOs.Energy;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Energy;

/// <summary>
/// 能源處理服務介面 (SYSO310 - 能源處理功能)
/// </summary>
public interface IEnergyProcessService
{
    Task<PagedResult<EnergyProcessDto>> GetEnergyProcessesAsync(EnergyProcessQueryDto query);
    Task<EnergyProcessDto?> GetEnergyProcessByIdAsync(long tKey);
    Task<EnergyProcessDto?> GetEnergyProcessByProcessIdAsync(string processId);
    Task<long> CreateEnergyProcessAsync(CreateEnergyProcessDto dto);
    Task UpdateEnergyProcessAsync(long tKey, UpdateEnergyProcessDto dto);
    Task DeleteEnergyProcessAsync(long tKey);
}

