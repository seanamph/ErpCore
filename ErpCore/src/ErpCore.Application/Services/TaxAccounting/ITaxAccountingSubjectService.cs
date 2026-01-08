using ErpCore.Application.DTOs.Accounting;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.TaxAccounting;

/// <summary>
/// 會計科目服務介面 (SYST111-SYST11A)
/// </summary>
public interface ITaxAccountingSubjectService
{
    /// <summary>
    /// 查詢會計科目列表
    /// </summary>
    Task<PagedResult<AccountSubjectDto>> GetAccountSubjectsAsync(AccountSubjectQueryDto query);

    /// <summary>
    /// 查詢單筆會計科目
    /// </summary>
    Task<AccountSubjectDto> GetAccountSubjectByIdAsync(string stypeId);

    /// <summary>
    /// 新增會計科目
    /// </summary>
    Task<string> CreateAccountSubjectAsync(CreateAccountSubjectDto dto);

    /// <summary>
    /// 修改會計科目
    /// </summary>
    Task UpdateAccountSubjectAsync(string stypeId, UpdateAccountSubjectDto dto);

    /// <summary>
    /// 刪除會計科目
    /// </summary>
    Task DeleteAccountSubjectAsync(string stypeId);

    /// <summary>
    /// 檢查科目代號是否存在
    /// </summary>
    Task<bool> ExistsAsync(string stypeId);

    /// <summary>
    /// 檢查是否有未沖帳餘額
    /// </summary>
    Task<UnsettledBalanceDto> CheckUnsettledBalanceAsync(string stypeId);
}

