using ErpCore.Application.DTOs.TaxAccounting;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.TaxAccounting;

/// <summary>
/// 常用傳票服務介面 (SYST123)
/// </summary>
public interface ICommonVoucherService
{
    Task<PagedResult<CommonVoucherDto>> GetCommonVouchersAsync(CommonVoucherQueryDto query);
    Task<CommonVoucherDto> GetCommonVoucherByTKeyAsync(long tKey);
    Task<long> CreateCommonVoucherAsync(CreateCommonVoucherDto dto);
    Task UpdateCommonVoucherAsync(long tKey, UpdateCommonVoucherDto dto);
    Task DeleteCommonVoucherAsync(long tKey);
    Task<bool> ExistsAsync(string voucherId);
}

