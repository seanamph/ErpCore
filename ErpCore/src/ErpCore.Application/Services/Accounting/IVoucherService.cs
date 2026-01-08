using ErpCore.Application.DTOs.Accounting;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Accounting;

/// <summary>
/// 傳票服務介面 (SYSN120)
/// </summary>
public interface IVoucherService
{
    Task<PagedResult<VoucherDto>> GetVouchersAsync(VoucherQueryDto query);
    Task<VoucherDto> GetVoucherByIdAsync(string voucherId);
    Task<string> CreateVoucherAsync(CreateVoucherDto dto);
    Task UpdateVoucherAsync(string voucherId, UpdateVoucherDto dto);
    Task DeleteVoucherAsync(string voucherId);
    Task PostVoucherAsync(string voucherId);
}

