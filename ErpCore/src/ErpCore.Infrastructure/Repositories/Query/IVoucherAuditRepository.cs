using ErpCore.Domain.Entities.Query;
using ErpCore.Application.DTOs.Query;

namespace ErpCore.Infrastructure.Repositories.Query;

/// <summary>
/// 傳票審核傳送檔 Repository 接口 (SYSQ250)
/// </summary>
public interface IVoucherAuditRepository
{
    Task<VoucherAudit?> GetByIdAsync(long tKey);
    Task<VoucherAudit?> GetByVoucherIdAsync(string voucherId);
    Task<PagedResult<VoucherAuditDto>> QueryAsync(VoucherAuditQueryDto query);
    Task<VoucherAudit> CreateAsync(VoucherAudit entity);
    Task<VoucherAudit> UpdateAsync(VoucherAudit entity);
    Task DeleteAsync(long tKey);
}

