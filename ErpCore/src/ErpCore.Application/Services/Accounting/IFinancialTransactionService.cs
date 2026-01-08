using ErpCore.Application.DTOs.Accounting;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Accounting;

/// <summary>
/// 財務交易服務介面 (SYSN210)
/// </summary>
public interface IFinancialTransactionService
{
    /// <summary>
    /// 查詢財務交易列表
    /// </summary>
    Task<PagedResult<FinancialTransactionDto>> GetFinancialTransactionsAsync(FinancialTransactionQueryDto query);

    /// <summary>
    /// 根據主鍵查詢財務交易
    /// </summary>
    Task<FinancialTransactionDto> GetFinancialTransactionByIdAsync(long tKey);

    /// <summary>
    /// 根據交易單號查詢財務交易
    /// </summary>
    Task<FinancialTransactionDto> GetFinancialTransactionByTxnNoAsync(string txnNo);

    /// <summary>
    /// 新增財務交易
    /// </summary>
    Task<long> CreateFinancialTransactionAsync(CreateFinancialTransactionDto dto);

    /// <summary>
    /// 修改財務交易
    /// </summary>
    Task UpdateFinancialTransactionAsync(long tKey, UpdateFinancialTransactionDto dto);

    /// <summary>
    /// 刪除財務交易
    /// </summary>
    Task DeleteFinancialTransactionAsync(long tKey);

    /// <summary>
    /// 確認財務交易
    /// </summary>
    Task ConfirmFinancialTransactionAsync(long tKey);

    /// <summary>
    /// 過帳財務交易
    /// </summary>
    Task PostFinancialTransactionAsync(long tKey);

    /// <summary>
    /// 檢查借貸平衡
    /// </summary>
    Task<BalanceCheckDto> CheckBalanceAsync(List<CreateFinancialTransactionDto> transactions);
}

