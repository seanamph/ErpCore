using ErpCore.Application.DTOs.TaxAccounting;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.TaxAccounting;

/// <summary>
/// 交易資料處理服務介面 (SYST221, SYST311-SYST352)
/// </summary>
public interface ITransactionDataService
{
    /// <summary>
    /// 查詢傳票確認列表
    /// </summary>
    Task<PagedResult<InvoiceVoucherDto>> GetConfirmVouchersAsync(VoucherConfirmQueryDto query);

    /// <summary>
    /// 批次確認傳票
    /// </summary>
    Task<BatchConfirmResultDto> BatchConfirmVouchersAsync(BatchConfirmVoucherDto dto);

    /// <summary>
    /// 查詢傳票過帳列表
    /// </summary>
    Task<PagedResult<InvoiceVoucherDto>> GetPostingVouchersAsync(VoucherPostingQueryDto query);

    /// <summary>
    /// 批次過帳傳票
    /// </summary>
    Task<BatchPostingResultDto> BatchPostingVouchersAsync(BatchPostingVoucherDto dto);

    /// <summary>
    /// 查詢傳票狀態統計
    /// </summary>
    Task<VoucherStatusCountDto> GetVoucherStatusCountAsync(string postingYearMonth);

    /// <summary>
    /// 查詢傳票列表 (SYST331-SYST332)
    /// </summary>
    Task<PagedResult<InvoiceVoucherDto>> QueryVouchersAsync(InvoiceVoucherQueryDto query);

    /// <summary>
    /// 查詢反過帳資料年結處理
    /// </summary>
    Task<PagedResult<InvoiceVoucherDto>> GetReverseYearEndVouchersAsync(ReverseYearEndQueryDto query);

    /// <summary>
    /// 反過帳傳票
    /// </summary>
    Task ReversePostingVoucherAsync(string voucherId, ReversePostingDto dto);
}

