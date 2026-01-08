using ErpCore.Domain.Entities.Accounting;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.Accounting;

/// <summary>
/// 財務交易 Repository 介面 (SYSN210)
/// </summary>
public interface IFinancialTransactionRepository
{
    /// <summary>
    /// 根據主鍵查詢財務交易
    /// </summary>
    Task<FinancialTransaction?> GetByIdAsync(long tKey);

    /// <summary>
    /// 根據交易單號查詢財務交易
    /// </summary>
    Task<FinancialTransaction?> GetByTxnNoAsync(string txnNo);

    /// <summary>
    /// 查詢財務交易列表（分頁）
    /// </summary>
    Task<PagedResult<FinancialTransaction>> QueryAsync(FinancialTransactionQuery query);

    /// <summary>
    /// 新增財務交易
    /// </summary>
    Task<FinancialTransaction> CreateAsync(FinancialTransaction transaction);

    /// <summary>
    /// 修改財務交易
    /// </summary>
    Task<FinancialTransaction> UpdateAsync(FinancialTransaction transaction);

    /// <summary>
    /// 刪除財務交易
    /// </summary>
    Task DeleteAsync(long tKey);

    /// <summary>
    /// 檢查交易單號是否存在
    /// </summary>
    Task<bool> ExistsAsync(string txnNo);

    /// <summary>
    /// 檢查借貸平衡
    /// </summary>
    Task<BalanceCheckResult> CheckBalanceAsync(List<FinancialTransaction> transactions);
}

/// <summary>
/// 財務交易查詢條件
/// </summary>
public class FinancialTransactionQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? TxnNo { get; set; }
    public DateTime? TxnDateFrom { get; set; }
    public DateTime? TxnDateTo { get; set; }
    public string? TxnType { get; set; }
    public string? StypeId { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 借貸平衡檢查結果
/// </summary>
public class BalanceCheckResult
{
    public bool IsBalanced { get; set; }
    public decimal DebitTotal { get; set; }
    public decimal CreditTotal { get; set; }
    public decimal Difference { get; set; }
}

