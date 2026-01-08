using ErpCore.Application.DTOs.Certificate;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Certificate;

/// <summary>
/// 憑證服務介面 (SYSK110-SYSK150)
/// </summary>
public interface IVoucherService
{
    Task<PagedResult<VoucherDto>> GetVouchersAsync(VoucherQueryDto query);
    Task<VoucherDto> GetVoucherByIdAsync(string voucherId);
    Task<VoucherDto> CreateVoucherAsync(CreateVoucherDto dto);
    Task<VoucherDto> UpdateVoucherAsync(string voucherId, UpdateVoucherDto dto);
    Task DeleteVoucherAsync(string voucherId);
    Task ApproveVoucherAsync(string voucherId, ApproveVoucherDto dto);
    Task CancelVoucherAsync(string voucherId, CancelVoucherDto dto);
    Task<List<VoucherCheckResultDto>> CheckVouchersAsync(List<string> voucherIds);
}

