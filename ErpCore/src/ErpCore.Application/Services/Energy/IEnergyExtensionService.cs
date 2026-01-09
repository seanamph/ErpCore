using ErpCore.Application.DTOs.Energy;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Energy;

/// <summary>
/// 能源擴展服務介面 (SYSOU10-SYSOU33 - 能源擴展功能)
/// </summary>
public interface IEnergyExtensionService
{
    Task<PagedResult<EnergyExtensionDto>> GetEnergyExtensionsAsync(EnergyExtensionQueryDto query);
    Task<EnergyExtensionDto?> GetEnergyExtensionByIdAsync(long tKey);
    Task<EnergyExtensionDto?> GetEnergyExtensionByExtensionIdAsync(string extensionId);
    Task<long> CreateEnergyExtensionAsync(CreateEnergyExtensionDto dto);
    Task UpdateEnergyExtensionAsync(long tKey, UpdateEnergyExtensionDto dto);
    Task DeleteEnergyExtensionAsync(long tKey);
}

