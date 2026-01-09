using ErpCore.Domain.Entities.InvoiceSales;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.InvoiceSales;

/// <summary>
/// 發票 Repository 接口 (SYSG110-SYSG190 - 發票資料維護)
/// </summary>
public interface IInvoiceRepository
{
    /// <summary>
    /// 根據主鍵查詢發票
    /// </summary>
    Task<Invoice?> GetByIdAsync(long tKey);

    /// <summary>
    /// 根據發票編號查詢發票
    /// </summary>
    Task<Invoice?> GetByInvoiceIdAsync(string invoiceId);

    /// <summary>
    /// 查詢發票列表（分頁）
    /// </summary>
    Task<PagedResult<Invoice>> QueryAsync(InvoiceQuery query);

    /// <summary>
    /// 新增發票
    /// </summary>
    Task<long> CreateAsync(Invoice invoice);

    /// <summary>
    /// 修改發票
    /// </summary>
    Task<int> UpdateAsync(Invoice invoice);

    /// <summary>
    /// 刪除發票
    /// </summary>
    Task<int> DeleteAsync(long tKey);

    /// <summary>
    /// 檢查發票編號是否存在
    /// </summary>
    Task<bool> ExistsByInvoiceIdAsync(string invoiceId, long? excludeTKey = null);
}

/// <summary>
/// 發票查詢條件
/// </summary>
public class InvoiceQuery
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
}

