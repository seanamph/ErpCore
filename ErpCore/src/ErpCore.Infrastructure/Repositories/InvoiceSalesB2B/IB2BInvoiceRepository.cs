using ErpCore.Domain.Entities.InvoiceSalesB2B;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.InvoiceSalesB2B;

/// <summary>
/// B2B發票 Repository 接口 (SYSG000_B2B - B2B發票資料維護)
/// </summary>
public interface IB2BInvoiceRepository
{
    /// <summary>
    /// 根據主鍵查詢B2B發票
    /// </summary>
    Task<B2BInvoice?> GetByIdAsync(long tKey);

    /// <summary>
    /// 根據發票編號查詢B2B發票
    /// </summary>
    Task<B2BInvoice?> GetByInvoiceIdAsync(string invoiceId);

    /// <summary>
    /// 查詢B2B發票列表（分頁）
    /// </summary>
    Task<PagedResult<B2BInvoice>> QueryAsync(B2BInvoiceQuery query);

    /// <summary>
    /// 新增B2B發票
    /// </summary>
    Task<long> CreateAsync(B2BInvoice invoice);

    /// <summary>
    /// 修改B2B發票
    /// </summary>
    Task<int> UpdateAsync(B2BInvoice invoice);

    /// <summary>
    /// 刪除B2B發票
    /// </summary>
    Task<int> DeleteAsync(long tKey);

    /// <summary>
    /// 檢查發票編號是否存在
    /// </summary>
    Task<bool> ExistsByInvoiceIdAsync(string invoiceId, long? excludeTKey = null);
}

/// <summary>
/// B2B發票查詢條件
/// </summary>
public class B2BInvoiceQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? InvoiceId { get; set; }
    public string? InvoiceType { get; set; }
    public string? InvoiceYm { get; set; }
    public string? TaxId { get; set; }
    public string? SiteId { get; set; }
    public string? Status { get; set; }
    public string? B2BFlag { get; set; } = "Y";
}

