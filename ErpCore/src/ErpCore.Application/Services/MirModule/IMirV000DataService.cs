using ErpCore.Application.DTOs.MirModule;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.MirModule;

/// <summary>
/// MIRV000 資料服務介面
/// </summary>
public interface IMirV000DataService
{
    Task<PagedResult<MirV000DataDto>> GetDataListAsync(MirV000DataQueryDto query);
    Task<MirV000DataDto> GetDataByIdAsync(string dataId);
    Task<string> CreateDataAsync(CreateMirV000DataDto dto);
    Task UpdateDataAsync(string dataId, UpdateMirV000DataDto dto);
    Task DeleteDataAsync(string dataId);
}

