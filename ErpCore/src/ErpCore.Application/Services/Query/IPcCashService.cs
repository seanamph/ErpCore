using ErpCore.Application.DTOs.Query;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Query;

/// <summary>
/// 零用金主檔服務接口 (SYSQ210)
/// </summary>
public interface IPcCashService
{
    Task<PagedResult<PcCashDto>> QueryAsync(PcCashQueryDto query);
    Task<PcCashDto> GetByIdAsync(long tKey);
    Task<PcCashDto> GetByCashIdAsync(string cashId);
    Task<PcCashDto> CreateAsync(CreatePcCashDto dto);
    Task<PcCashDto> UpdateAsync(long tKey, UpdatePcCashDto dto);
    Task DeleteAsync(long tKey);
    Task<List<PcCashDto>> BatchCreateAsync(BatchCreatePcCashDto dto);
}

