using ErpCore.Application.DTOs.UniversalModule;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.UniversalModule;

/// <summary>
/// 通用模組服務介面 (UNIV000系列)
/// </summary>
public interface IUniv000Service
{
    Task<PagedResult<Univ000Dto>> GetUniv000ListAsync(Univ000QueryDto query);
    Task<Univ000Dto?> GetUniv000ByIdAsync(long tKey);
    Task<Univ000Dto?> GetUniv000ByDataIdAsync(string dataId);
    Task<long> CreateUniv000Async(CreateUniv000Dto dto);
    Task UpdateUniv000Async(long tKey, UpdateUniv000Dto dto);
    Task DeleteUniv000Async(long tKey);
}

