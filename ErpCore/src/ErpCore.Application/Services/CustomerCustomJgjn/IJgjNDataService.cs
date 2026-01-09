using ErpCore.Application.DTOs.CustomerCustomJgjn;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.CustomerCustomJgjn;

/// <summary>
/// JGJN資料服務介面
/// </summary>
public interface IJgjNDataService
{
    Task<PagedResult<JgjNDataDto>> GetJgjNDataListAsync(JgjNDataQueryDto query);
    Task<JgjNDataDto?> GetJgjNDataByIdAsync(long tKey);
    Task<JgjNDataDto?> GetJgjNDataByDataIdAndModuleCodeAsync(string dataId, string moduleCode);
    Task<long> CreateJgjNDataAsync(CreateJgjNDataDto dto);
    Task UpdateJgjNDataAsync(long tKey, UpdateJgjNDataDto dto);
    Task DeleteJgjNDataAsync(long tKey);
}

