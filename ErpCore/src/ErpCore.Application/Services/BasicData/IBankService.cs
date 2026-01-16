using ErpCore.Application.DTOs.BasicData;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.BasicData;

/// <summary>
/// 銀行服務介面
/// </summary>
public interface IBankService
{
    /// <summary>
    /// 查詢銀行列表
    /// </summary>
    Task<PagedResult<BankDto>> GetBanksAsync(BankQueryDto query);

    /// <summary>
    /// 查詢單筆銀行
    /// </summary>
    Task<BankDto> GetBankAsync(string bankId);

    /// <summary>
    /// 新增銀行
    /// </summary>
    Task<string> CreateBankAsync(CreateBankDto dto);

    /// <summary>
    /// 修改銀行
    /// </summary>
    Task UpdateBankAsync(string bankId, UpdateBankDto dto);

    /// <summary>
    /// 刪除銀行
    /// </summary>
    Task DeleteBankAsync(string bankId);

    /// <summary>
    /// 批次刪除銀行
    /// </summary>
    Task DeleteBanksBatchAsync(BatchDeleteBankDto dto);
}
