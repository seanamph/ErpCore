using ErpCore.Domain.Entities.Accounting;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.TaxAccounting;

/// <summary>
/// 交易資料處理 Repository 介面 (SYST221, SYST311-SYST352)
/// </summary>
public interface ITransactionDataRepository
{
    /// <summary>
    /// 查詢傳票確認列表
    /// </summary>
    Task<PagedResult<Voucher>> GetConfirmVouchersAsync(VoucherConfirmQuery query);

    /// <summary>
    /// 批次確認傳票
    /// </summary>
    Task<int> BatchConfirmVouchersAsync(List<string> voucherIds, DateTime confirmDate, string confirmBy);

    /// <summary>
    /// 查詢傳票過帳列表
    /// </summary>
    Task<PagedResult<Voucher>> GetPostingVouchersAsync(VoucherPostingQuery query);

    /// <summary>
    /// 批次過帳傳票
    /// </summary>
    Task<int> BatchPostingVouchersAsync(List<string> voucherIds, string postingYearMonth, DateTime postingDate, string postingBy);

    /// <summary>
    /// 查詢傳票狀態統計
    /// </summary>
    Task<VoucherStatusCount> GetVoucherStatusCountAsync(string postingYearMonth);

    /// <summary>
    /// 反過帳傳票
    /// </summary>
    Task ReversePostingVoucherAsync(string voucherId, DateTime reversePostingDate, string reversePostingBy);

    /// <summary>
    /// 查詢反過帳資料年結處理
    /// </summary>
    Task<PagedResult<Voucher>> GetReverseYearEndVouchersAsync(ReverseYearEndQuery query);
}

/// <summary>
/// 傳票確認查詢條件
/// </summary>
public class VoucherConfirmQuery : PagedQuery
{
    public string? VoucherNoFrom { get; set; }
    public string? VoucherNoTo { get; set; }
    public DateTime? VoucherDateFrom { get; set; }
    public DateTime? VoucherDateTo { get; set; }
    public List<string>? VoucherTypes { get; set; }
    public string? VoucherStatus { get; set; }
    public DateTime? ConfirmDateFrom { get; set; }
    public DateTime? ConfirmDateTo { get; set; }
}

/// <summary>
/// 傳票過帳查詢條件
/// </summary>
public class VoucherPostingQuery : PagedQuery
{
    public string? PostingYearMonth { get; set; }
    public DateTime? VoucherDateFrom { get; set; }
    public DateTime? VoucherDateTo { get; set; }
    public List<string>? VoucherTypes { get; set; }
    public string? VoucherStatus { get; set; }
    public bool PostingByDetail { get; set; }
}

/// <summary>
/// 反過帳資料年結處理查詢條件
/// </summary>
public class ReverseYearEndQuery : PagedQuery
{
    public string? Year { get; set; }
}

/// <summary>
/// 傳票狀態統計
/// </summary>
public class VoucherStatusCount
{
    public string PostingYearMonth { get; set; } = string.Empty;
    public int CreateCount { get; set; }
    public int ConfirmCount { get; set; }
    public int PostingCount { get; set; }
    public decimal CreateAmount { get; set; }
    public decimal ConfirmAmount { get; set; }
    public decimal PostingAmount { get; set; }
}

