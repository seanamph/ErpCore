using ErpCore.Domain.Entities.Accounting;
using ErpCore.Domain.Entities.TaxAccounting;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.TaxAccounting;

/// <summary>
/// 發票資料維護 Repository 介面 (SYST211-SYST212)
/// </summary>
public interface IInvoiceDataRepository
{
    /// <summary>
    /// 根據傳票編號查詢傳票
    /// </summary>
    Task<Voucher?> GetVoucherByIdAsync(string voucherId);

    /// <summary>
    /// 查詢傳票列表（分頁）
    /// </summary>
    Task<PagedResult<Voucher>> QueryVouchersAsync(InvoiceVoucherQuery query);

    /// <summary>
    /// 新增傳票
    /// </summary>
    Task<Voucher> CreateVoucherAsync(Voucher voucher);

    /// <summary>
    /// 修改傳票
    /// </summary>
    Task<Voucher> UpdateVoucherAsync(Voucher voucher);

    /// <summary>
    /// 刪除傳票
    /// </summary>
    Task DeleteVoucherAsync(string voucherId);

    /// <summary>
    /// 檢查傳票是否存在
    /// </summary>
    Task<bool> VoucherExistsAsync(string voucherId);

    /// <summary>
    /// 檢查傳票借貸平衡
    /// </summary>
    Task<BalanceCheckResult> CheckBalanceAsync(string voucherId);

    /// <summary>
    /// 查詢發票傳票列表
    /// </summary>
    Task<List<InvoiceVoucher>> GetInvoiceVouchersByVoucherTKeyAsync(long voucherTKey);

    /// <summary>
    /// 新增發票傳票
    /// </summary>
    Task<InvoiceVoucher> CreateInvoiceVoucherAsync(InvoiceVoucher invoiceVoucher);

    /// <summary>
    /// 修改發票傳票
    /// </summary>
    Task<InvoiceVoucher> UpdateInvoiceVoucherAsync(InvoiceVoucher invoiceVoucher);

    /// <summary>
    /// 刪除發票傳票
    /// </summary>
    Task DeleteInvoiceVoucherAsync(long tKey);

    /// <summary>
    /// 查詢費用/收入分攤比率列表
    /// </summary>
    Task<PagedResult<AllocationRatio>> QueryAllocationRatiosAsync(AllocationRatioQuery query);

    /// <summary>
    /// 新增費用/收入分攤比率
    /// </summary>
    Task<AllocationRatio> CreateAllocationRatioAsync(AllocationRatio allocationRatio);

    /// <summary>
    /// 修改費用/收入分攤比率
    /// </summary>
    Task<AllocationRatio> UpdateAllocationRatioAsync(AllocationRatio allocationRatio);

    /// <summary>
    /// 刪除費用/收入分攤比率
    /// </summary>
    Task DeleteAllocationRatioAsync(long tKey);

    /// <summary>
    /// 新增傳票明細
    /// </summary>
    Task<VoucherDetail> CreateVoucherDetailAsync(VoucherDetail detail);

    /// <summary>
    /// 查詢傳票明細列表
    /// </summary>
    Task<List<VoucherDetail>> GetVoucherDetailsAsync(string voucherId);
}

/// <summary>
/// 傳票查詢條件
/// </summary>
public class InvoiceVoucherQuery : PagedQuery
{
    public string? VoucherId { get; set; }
    public DateTime? VoucherDateFrom { get; set; }
    public DateTime? VoucherDateTo { get; set; }
    public string? VoucherStatus { get; set; }
    public string? VoucherKind { get; set; }
    public string? TypeId { get; set; }
    public string? SiteId { get; set; }
    public string? VendorId { get; set; }
}

/// <summary>
/// 分攤比率查詢條件
/// </summary>
public class AllocationRatioQuery : PagedQuery
{
    public string? DisYm { get; set; }
    public string? StypeId { get; set; }
    public string? OrgId { get; set; }
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

/// <summary>
/// 借貸平衡檢查結果
/// </summary>
public class BalanceCheckResult
{
    public bool IsBalanced { get; set; }
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public decimal Difference { get; set; }
}

