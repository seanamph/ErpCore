using ErpCore.Domain.Entities.CustomerInvoice;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.CustomerInvoice;

/// <summary>
/// 發票列印 Repository 介面 (SYS2000 - 發票列印作業)
/// </summary>
public interface IInvoicePrintRepository
{
    /// <summary>
    /// 根據發票號碼查詢發票
    /// </summary>
    Task<Invoice?> GetByIdAsync(string invoiceNo);

    /// <summary>
    /// 查詢發票列表（分頁）
    /// </summary>
    Task<PagedResult<Invoice>> QueryAsync(InvoiceQuery query);

    /// <summary>
    /// 查詢發票總數
    /// </summary>
    Task<int> GetCountAsync(InvoiceQuery query);

    /// <summary>
    /// 新增發票
    /// </summary>
    Task<Invoice> CreateAsync(Invoice invoice, List<InvoiceDetail>? details = null);

    /// <summary>
    /// 修改發票
    /// </summary>
    Task<Invoice> UpdateAsync(Invoice invoice, List<InvoiceDetail>? details = null);

    /// <summary>
    /// 刪除發票
    /// </summary>
    Task DeleteAsync(string invoiceNo);

    /// <summary>
    /// 檢查發票號碼是否存在
    /// </summary>
    Task<bool> ExistsAsync(string invoiceNo);

    /// <summary>
    /// 查詢發票明細
    /// </summary>
    Task<IEnumerable<InvoiceDetail>> GetDetailsByInvoiceNoAsync(string invoiceNo);

    /// <summary>
    /// 記錄列印記錄
    /// </summary>
    Task<InvoicePrintLog> CreatePrintLogAsync(InvoicePrintLog printLog);

    /// <summary>
    /// 查詢列印記錄
    /// </summary>
    Task<IEnumerable<InvoicePrintLog>> GetPrintLogsByInvoiceNoAsync(string invoiceNo);

    /// <summary>
    /// 更新發票列印資訊
    /// </summary>
    Task UpdatePrintInfoAsync(string invoiceNo, string printFormat, string printUser);
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
    public string? InvoiceNo { get; set; }
    public string? CustomerId { get; set; }
    public DateTime? InvoiceDateFrom { get; set; }
    public DateTime? InvoiceDateTo { get; set; }
    public string? Status { get; set; }
}

