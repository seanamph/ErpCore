using ErpCore.Application.DTOs.MirModule;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.MirModule;

/// <summary>
/// MIRW000 資料服務介面
/// </summary>
public interface IMirW000DataService
{
    Task<PagedResult<MirW000DataDto>> GetDataListAsync(MirW000DataQueryDto query);
    Task<MirW000DataDto> GetDataByIdAsync(string dataId);
    Task<string> CreateDataAsync(CreateMirW000DataDto dto);
    Task UpdateDataAsync(string dataId, UpdateMirW000DataDto dto);
    Task DeleteDataAsync(string dataId);
}

