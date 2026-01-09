using ErpCore.Domain.Entities.CustomerInvoice;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.CustomerInvoice;

/// <summary>
/// 總帳資料 Repository 介面 (SYS2000 - 總帳資料維護)
/// </summary>
public interface ILedgerDataRepository
{
    /// <summary>
    /// 根據總帳編號查詢總帳
    /// </summary>
    Task<GeneralLedger?> GetByIdAsync(string ledgerId);

    /// <summary>
    /// 查詢總帳列表（分頁）
    /// </summary>
    Task<PagedResult<GeneralLedger>> QueryAsync(GeneralLedgerQuery query);

    /// <summary>
    /// 查詢總帳總數
    /// </summary>
    Task<int> GetCountAsync(GeneralLedgerQuery query);

    /// <summary>
    /// 新增總帳
    /// </summary>
    Task<GeneralLedger> CreateAsync(GeneralLedger ledger);

    /// <summary>
    /// 修改總帳
    /// </summary>
    Task<GeneralLedger> UpdateAsync(GeneralLedger ledger);

    /// <summary>
    /// 刪除總帳
    /// </summary>
    Task DeleteAsync(string ledgerId);

    /// <summary>
    /// 檢查總帳編號是否存在
    /// </summary>
    Task<bool> ExistsAsync(string ledgerId);

    /// <summary>
    /// 更新總帳狀態
    /// </summary>
    Task UpdateStatusAsync(string ledgerId, string status);

    /// <summary>
    /// 查詢會計科目餘額
    /// </summary>
    Task<AccountBalance?> GetAccountBalanceAsync(string accountId, string period);

    /// <summary>
    /// 查詢會計科目餘額列表
    /// </summary>
    Task<IEnumerable<AccountBalance>> GetAccountBalancesAsync(AccountBalanceQuery query);

    /// <summary>
    /// 更新會計科目餘額
    /// </summary>
    Task UpdateAccountBalanceAsync(AccountBalance balance);
}

/// <summary>
/// 總帳查詢條件
/// </summary>
public class GeneralLedgerQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? LedgerId { get; set; }
    public string? AccountId { get; set; }
    public string? Period { get; set; }
    public DateTime? LedgerDateFrom { get; set; }
    public DateTime? LedgerDateTo { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 科目餘額查詢條件
/// </summary>
public class AccountBalanceQuery
{
    public string? AccountId { get; set; }
    public string? Period { get; set; }
    public DateTime? PeriodFrom { get; set; }
    public DateTime? PeriodTo { get; set; }
}

