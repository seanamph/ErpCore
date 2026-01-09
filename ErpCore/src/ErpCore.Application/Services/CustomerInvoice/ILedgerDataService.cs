using ErpCore.Application.DTOs.CustomerInvoice;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.CustomerInvoice;

/// <summary>
/// 總帳資料服務介面 (SYS2000 - 總帳資料維護)
/// </summary>
public interface ILedgerDataService
{
    /// <summary>
    /// 查詢總帳列表
    /// </summary>
    Task<PagedResult<GeneralLedgerDto>> GetGeneralLedgersAsync(GeneralLedgerQueryDto query);

    /// <summary>
    /// 查詢單筆總帳
    /// </summary>
    Task<GeneralLedgerDto> GetGeneralLedgerByIdAsync(string ledgerId);

    /// <summary>
    /// 新增總帳
    /// </summary>
    Task<string> CreateGeneralLedgerAsync(CreateGeneralLedgerDto dto);

    /// <summary>
    /// 修改總帳
    /// </summary>
    Task UpdateGeneralLedgerAsync(string ledgerId, UpdateGeneralLedgerDto dto);

    /// <summary>
    /// 刪除總帳
    /// </summary>
    Task DeleteGeneralLedgerAsync(string ledgerId);

    /// <summary>
    /// 過帳
    /// </summary>
    Task PostLedgerAsync(string ledgerId);

    /// <summary>
    /// 查詢科目餘額
    /// </summary>
    Task<IEnumerable<AccountBalanceDto>> GetAccountBalancesAsync(AccountBalanceQueryDto query);
}

