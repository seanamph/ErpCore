using ErpCore.Application.DTOs.StandardModule;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.StandardModule;

/// <summary>
/// STD3000 資料服務介面 (SYS3620 - 標準資料維護)
/// </summary>
public interface IStd3000DataService
{
    Task<PagedResult<Std3000DataDto>> GetStd3000DataListAsync(Std3000DataQueryDto query);
    Task<Std3000DataDto?> GetStd3000DataByIdAsync(long tKey);
    Task<Std3000DataDto?> GetStd3000DataByDataIdAsync(string dataId);
    Task<long> CreateStd3000DataAsync(CreateStd3000DataDto dto);
    Task UpdateStd3000DataAsync(long tKey, UpdateStd3000DataDto dto);
    Task DeleteStd3000DataAsync(long tKey);
}

