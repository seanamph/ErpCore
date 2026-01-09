using ErpCore.Application.DTOs.StandardModule;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.StandardModule;

/// <summary>
/// STD5000 基礎資料服務介面 (SYS5110-SYS5150 - 基礎資料維護)
/// </summary>
public interface IStd5000BaseDataService
{
    Task<PagedResult<Std5000BaseDataDto>> GetStd5000BaseDataListAsync(Std5000BaseDataQueryDto query);
    Task<Std5000BaseDataDto?> GetStd5000BaseDataByIdAsync(long tKey);
    Task<Std5000BaseDataDto?> GetStd5000BaseDataByDataIdAndTypeAsync(string dataId, string dataType);
    Task<long> CreateStd5000BaseDataAsync(CreateStd5000BaseDataDto dto);
    Task UpdateStd5000BaseDataAsync(long tKey, UpdateStd5000BaseDataDto dto);
    Task DeleteStd5000BaseDataAsync(long tKey);
}

