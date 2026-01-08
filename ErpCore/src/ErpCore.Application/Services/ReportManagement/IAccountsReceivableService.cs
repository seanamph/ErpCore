using ErpCore.Application.DTOs.ReportManagement;

namespace ErpCore.Application.Services.ReportManagement;

/// <summary>
/// 應收帳款服務介面 (SYSR210-SYSR240)
/// </summary>
public interface IAccountsReceivableService
{
    Task<IEnumerable<AccountsReceivableDto>> GetAllAsync();
    Task<AccountsReceivableDto> GetByIdAsync(long tKey);
    Task<IEnumerable<AccountsReceivableDto>> GetByReceiptDateRangeAsync(DateTime? startDate, DateTime? endDate);
    Task<IEnumerable<AccountsReceivableDto>> GetByVoucherNoAsync(string voucherNo);
    Task<IEnumerable<AccountsReceivableDto>> GetByReceiptNoAsync(string receiptNo);
    Task<IEnumerable<AccountsReceivableDto>> GetByObjectIdAsync(string objectId);
    Task<AccountsReceivableDto> CreateAsync(CreateAccountsReceivableDto dto);
    Task<AccountsReceivableDto> UpdateAsync(long tKey, UpdateAccountsReceivableDto dto);
    Task DeleteAsync(long tKey);
}

