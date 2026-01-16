using ErpCore.Domain.Entities.Procurement;

namespace ErpCore.Infrastructure.Repositories.Procurement;

/// <summary>
/// 銀行帳戶 Repository 介面 (銀行帳戶維護)
/// </summary>
public interface IBankAccountRepository
{
    Task<BankAccount?> GetByIdAsync(string bankAccountId);
    Task<IEnumerable<BankAccount>> QueryAsync(BankAccountQuery query);
    Task<int> GetCountAsync(BankAccountQuery query);
    Task<bool> ExistsAsync(string bankAccountId);
    Task<bool> ExistsByAccountNumberAsync(string accountNumber);
    Task<BankAccount> CreateAsync(BankAccount bankAccount);
    Task<BankAccount> UpdateAsync(BankAccount bankAccount);
    Task DeleteAsync(string bankAccountId);
    Task<decimal?> GetBalanceAsync(string bankAccountId);
}

/// <summary>
/// 銀行帳戶查詢條件
/// </summary>
public class BankAccountQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? BankAccountId { get; set; }
    public string? BankId { get; set; }
    public string? AccountName { get; set; }
    public string? AccountNumber { get; set; }
    public string? AccountType { get; set; }
    public string? CurrencyId { get; set; }
    public string? Status { get; set; }
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}
