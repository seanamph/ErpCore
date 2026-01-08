using ErpCore.Application.DTOs.TaxAccounting;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.TaxAccounting;

/// <summary>
/// 傳票型態服務介面 (SYST121-SYST122)
/// </summary>
public interface IVoucherTypeService
{
    Task<PagedResult<VoucherTypeDto>> GetVoucherTypesAsync(VoucherTypeQueryDto query);
    Task<VoucherTypeDto> GetVoucherTypeByIdAsync(string voucherId);
    Task<string> CreateVoucherTypeAsync(CreateVoucherTypeDto dto);
    Task UpdateVoucherTypeAsync(string voucherId, UpdateVoucherTypeDto dto);
    Task DeleteVoucherTypeAsync(string voucherId);
    Task<bool> ExistsAsync(string voucherId);
}

