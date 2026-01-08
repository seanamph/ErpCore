using ErpCore.Domain.Entities.Certificate;

namespace ErpCore.Infrastructure.Repositories.Certificate;

/// <summary>
/// 憑證 Repository 介面 (SYSK110-SYSK150)
/// </summary>
public interface IVoucherRepository
{
    Task<Voucher?> GetByIdAsync(long tKey);
    Task<Voucher?> GetByVoucherIdAsync(string voucherId);
    Task<IEnumerable<Voucher>> GetAllAsync();
    Task<IEnumerable<Voucher>> GetPagedAsync(int pageIndex, int pageSize, string? voucherId = null, string? voucherType = null, string? shopId = null, string? status = null, DateTime? voucherDateFrom = null, DateTime? voucherDateTo = null);
    Task<int> GetCountAsync(string? voucherId = null, string? voucherType = null, string? shopId = null, string? status = null, DateTime? voucherDateFrom = null, DateTime? voucherDateTo = null);
    Task<Voucher> CreateAsync(Voucher entity);
    Task<Voucher> UpdateAsync(Voucher entity);
    Task DeleteAsync(string voucherId);
    Task<bool> ExistsAsync(string voucherId);
}

