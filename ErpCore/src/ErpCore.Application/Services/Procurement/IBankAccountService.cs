using ErpCore.Application.DTOs.Procurement;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Procurement;

/// <summary>
/// 銀行帳戶服務介面 (銀行帳戶維護)
/// </summary>
public interface IBankAccountService
{
    Task<PagedResult<BankAccountDto>> GetBankAccountsAsync(BankAccountQueryDto query);
    Task<BankAccountDto> GetBankAccountByIdAsync(string bankAccountId);
    Task<string> CreateBankAccountAsync(CreateBankAccountDto dto);
    Task UpdateBankAccountAsync(string bankAccountId, UpdateBankAccountDto dto);
    Task DeleteBankAccountAsync(string bankAccountId);
    Task UpdateStatusAsync(string bankAccountId, string status);
    Task<BankAccountBalanceDto> GetBalanceAsync(string bankAccountId);
    Task<bool> ExistsAsync(string bankAccountId);
}
