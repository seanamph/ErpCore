using ErpCore.Application.DTOs.Procurement;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Procurement;

/// <summary>
/// 銀行帳戶服務介面
/// </summary>
public interface IBankAccountService
{
    /// <summary>
    /// 查詢銀行帳戶列表
    /// </summary>
    Task<PagedResult<BankAccountDto>> GetBankAccountsAsync(BankAccountQueryDto query);

    /// <summary>
    /// 查詢單筆銀行帳戶
    /// </summary>
    Task<BankAccountDto> GetBankAccountByIdAsync(string bankAccountId);

    /// <summary>
    /// 新增銀行帳戶
    /// </summary>
    Task<string> CreateBankAccountAsync(CreateBankAccountDto dto);

    /// <summary>
    /// 修改銀行帳戶
    /// </summary>
    Task UpdateBankAccountAsync(string bankAccountId, UpdateBankAccountDto dto);

    /// <summary>
    /// 刪除銀行帳戶
    /// </summary>
    Task DeleteBankAccountAsync(string bankAccountId);

    /// <summary>
    /// 更新銀行帳戶狀態
    /// </summary>
    Task UpdateStatusAsync(string bankAccountId, string status);

    /// <summary>
    /// 查詢銀行帳戶餘額
    /// </summary>
    Task<BankAccountBalanceDto> GetBalanceAsync(string bankAccountId);

    /// <summary>
    /// 檢查銀行帳戶是否存在
    /// </summary>
    Task<bool> ExistsAsync(string bankAccountId);
}

