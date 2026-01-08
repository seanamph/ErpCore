using ErpCore.Application.DTOs.TaxAccounting;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.TaxAccounting;

/// <summary>
/// 發票資料維護服務介面 (SYST211-SYST212)
/// </summary>
public interface IInvoiceDataService
{
    /// <summary>
    /// 查詢傳票列表
    /// </summary>
    Task<PagedResult<InvoiceVoucherDto>> GetVouchersAsync(InvoiceVoucherQueryDto query);

    /// <summary>
    /// 根據傳票編號查詢傳票
    /// </summary>
    Task<InvoiceVoucherDto?> GetVoucherByIdAsync(string voucherId);

    /// <summary>
    /// 新增傳票
    /// </summary>
    Task<string> CreateVoucherAsync(CreateInvoiceVoucherDto dto);

    /// <summary>
    /// 修改傳票
    /// </summary>
    Task UpdateVoucherAsync(string voucherId, UpdateInvoiceVoucherDto dto);

    /// <summary>
    /// 刪除傳票
    /// </summary>
    Task DeleteVoucherAsync(string voucherId);

    /// <summary>
    /// 作廢傳票
    /// </summary>
    Task VoidVoucherAsync(string voucherId);

    /// <summary>
    /// 檢查傳票借貸平衡
    /// </summary>
    Task<BalanceCheckDto> CheckBalanceAsync(string voucherId);

    /// <summary>
    /// 查詢費用/收入分攤比率列表
    /// </summary>
    Task<PagedResult<AllocationRatioDto>> GetAllocationRatiosAsync(AllocationRatioQueryDto query);

    /// <summary>
    /// 新增費用/收入分攤比率
    /// </summary>
    Task<long> CreateAllocationRatioAsync(CreateAllocationRatioDto dto);

    /// <summary>
    /// 修改費用/收入分攤比率
    /// </summary>
    Task UpdateAllocationRatioAsync(long tKey, UpdateAllocationRatioDto dto);

    /// <summary>
    /// 刪除費用/收入分攤比率
    /// </summary>
    Task DeleteAllocationRatioAsync(long tKey);
}

