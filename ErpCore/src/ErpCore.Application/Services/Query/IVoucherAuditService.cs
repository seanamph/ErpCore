using ErpCore.Application.DTOs.Query;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Query;

/// <summary>
/// 傳票審核傳送檔服務接口 (SYSQ250)
/// </summary>
public interface IVoucherAuditService
{
    Task<PagedResult<VoucherAuditDto>> QueryAsync(VoucherAuditQueryDto query);
    Task<VoucherAuditDto> GetByIdAsync(long tKey);
    Task<VoucherAuditDto> GetByVoucherIdAsync(string voucherId);
    Task<VoucherAuditDto> CreateAsync(VoucherAuditDto dto);
    Task<VoucherAuditDto> UpdateAsync(long tKey, UpdateVoucherAuditDto dto);
    Task DeleteAsync(long tKey);
}

