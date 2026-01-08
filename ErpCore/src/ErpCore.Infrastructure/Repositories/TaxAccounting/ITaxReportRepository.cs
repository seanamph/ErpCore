using ErpCore.Domain.Entities.Accounting;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.TaxAccounting;

/// <summary>
/// 稅務報表查詢 Repository 介面 (SYST411-SYST452)
/// </summary>
public interface ITaxReportRepository
{
    /// <summary>
    /// 查詢傳票列表
    /// </summary>
    Task<PagedResult<Voucher>> GetVouchersAsync(TaxReportVoucherQuery query);

    /// <summary>
    /// 查詢傳票明細
    /// </summary>
    Task<List<VoucherDetail>> GetVoucherDetailsAsync(string voucherId);
}

/// <summary>
/// 傳票查詢條件
/// </summary>
public class TaxReportVoucherQuery : PagedQuery
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? VoucherIdFrom { get; set; }
    public string? VoucherIdTo { get; set; }
    public List<string>? VoucherKinds { get; set; }
    public List<string>? VoucherStatuses { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedDateFrom { get; set; }
    public DateTime? CreatedDateTo { get; set; }
    public string? SiteId { get; set; }
}

