using ErpCore.Domain.Entities.Certificate;

namespace ErpCore.Infrastructure.Repositories.Certificate;

/// <summary>
/// 憑證明細 Repository 介面 (SYSK110-SYSK150)
/// </summary>
public interface IVoucherDetailRepository
{
    Task<IEnumerable<VoucherDetail>> GetByVoucherIdAsync(string voucherId);
    Task<VoucherDetail?> GetByIdAsync(long tKey);
    Task<VoucherDetail> CreateAsync(VoucherDetail entity);
    Task<VoucherDetail> UpdateAsync(VoucherDetail entity);
    Task DeleteAsync(long tKey);
    Task DeleteByVoucherIdAsync(string voucherId);
}

