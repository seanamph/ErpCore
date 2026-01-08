using ErpCore.Domain.Entities.ReportManagement;

namespace ErpCore.Infrastructure.Repositories.ReportManagement;

/// <summary>
/// 應收帳款 Repository 介面 (SYSR210-SYSR240)
/// </summary>
public interface IAccountsReceivableRepository
{
    Task<AccountsReceivable?> GetByIdAsync(long tKey);
    Task<IEnumerable<AccountsReceivable>> GetAllAsync();
    Task<IEnumerable<AccountsReceivable>> GetByReceiptDateRangeAsync(DateTime? startDate, DateTime? endDate);
    Task<IEnumerable<AccountsReceivable>> GetByVoucherNoAsync(string voucherNo);
    Task<IEnumerable<AccountsReceivable>> GetByReceiptNoAsync(string receiptNo);
    Task<IEnumerable<AccountsReceivable>> GetByObjectIdAsync(string objectId);
    Task<AccountsReceivable> CreateAsync(AccountsReceivable entity);
    Task<AccountsReceivable> UpdateAsync(AccountsReceivable entity);
    Task DeleteAsync(long tKey);
}

