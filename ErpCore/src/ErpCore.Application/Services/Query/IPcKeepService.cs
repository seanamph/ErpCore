using ErpCore.Application.DTOs.Query;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Query;

/// <summary>
/// 保管人及額度設定服務介面 (SYSQ120)
/// </summary>
public interface IPcKeepService
{
    Task<PagedResult<PcKeepDto>> QueryAsync(PcKeepQueryDto query);
    Task<PcKeepDto> GetByIdAsync(long tKey);
    Task<PcKeepDto> CreateAsync(CreatePcKeepDto dto);
    Task<PcKeepDto> UpdateAsync(long tKey, UpdatePcKeepDto dto);
    Task DeleteAsync(long tKey);
}

