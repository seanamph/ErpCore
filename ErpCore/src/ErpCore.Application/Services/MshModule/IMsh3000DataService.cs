using ErpCore.Application.DTOs.MshModule;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.MshModule;

/// <summary>
/// MSH3000 資料服務介面
/// </summary>
public interface IMsh3000DataService
{
    Task<PagedResult<Msh3000DataDto>> GetDataListAsync(Msh3000DataQueryDto query);
    Task<Msh3000DataDto> GetDataByIdAsync(string dataId);
    Task<string> CreateDataAsync(CreateMsh3000DataDto dto);
    Task UpdateDataAsync(string dataId, UpdateMsh3000DataDto dto);
    Task DeleteDataAsync(string dataId);
}

