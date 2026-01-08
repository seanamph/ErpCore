using ErpCore.Domain.Entities.Accounting;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.Accounting;

/// <summary>
/// 會計科目 Repository 介面 (SYSN110)
/// </summary>
public interface IAccountSubjectRepository
{
    /// <summary>
    /// 根據科目代號查詢會計科目
    /// </summary>
    Task<AccountSubject?> GetByIdAsync(string stypeId);

    /// <summary>
    /// 查詢會計科目列表（分頁）
    /// </summary>
    Task<PagedResult<AccountSubject>> QueryAsync(AccountSubjectQuery query);

    /// <summary>
    /// 新增會計科目
    /// </summary>
    Task<AccountSubject> CreateAsync(AccountSubject accountSubject);

    /// <summary>
    /// 修改會計科目
    /// </summary>
    Task<AccountSubject> UpdateAsync(AccountSubject accountSubject);

    /// <summary>
    /// 刪除會計科目
    /// </summary>
    Task DeleteAsync(string stypeId);

    /// <summary>
    /// 檢查科目代號是否存在
    /// </summary>
    Task<bool> ExistsAsync(string stypeId);

    /// <summary>
    /// 檢查未沖帳餘額
    /// </summary>
    Task<decimal> GetUnsettledBalanceAsync(string stypeId);
}

/// <summary>
/// 會計科目查詢條件
/// </summary>
public class AccountSubjectQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? StypeId { get; set; }
    public string? StypeName { get; set; }
    public string? Dc { get; set; }
    public string? LedgerMd { get; set; }
    public string? VoucherType { get; set; }
    public string? BudgetYn { get; set; }
    public string? StypeClass { get; set; }
}

