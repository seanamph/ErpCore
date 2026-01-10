using ErpCore.Application.DTOs.Query;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Query;

/// <summary>
/// 零用金請款檔服務接口 (SYSQ220)
/// </summary>
public interface IPcCashRequestService
{
    Task<PagedResult<PcCashRequestDto>> QueryAsync(PcCashRequestQueryDto query);
    Task<PcCashRequestDto> GetByIdAsync(long tKey);
    Task<PcCashRequestDto> GetByRequestIdAsync(string requestId);
    Task<PcCashRequestDto> CreateAsync(CreatePcCashRequestDto dto);
    Task<PcCashRequestDto> UpdateAsync(long tKey, UpdatePcCashRequestDto dto);
    Task DeleteAsync(long tKey);
}

