using ErpCore.Application.DTOs.Query;

namespace ErpCore.Application.Services.Query;

/// <summary>
/// 零用金參數服務介面 (SYSQ110)
/// </summary>
public interface ICashParamsService
{
    Task<IEnumerable<CashParamsDto>> GetAllAsync();
    Task<CashParamsDto> GetByIdAsync(long tKey);
    Task<CashParamsDto> CreateAsync(CreateCashParamsDto dto);
    Task<CashParamsDto> UpdateAsync(long tKey, UpdateCashParamsDto dto);
    Task DeleteAsync(long tKey);
}

