using ErpCore.Domain.Entities.CustomerCustomJgjn;

namespace ErpCore.Infrastructure.Repositories.CustomerCustomJgjn;

/// <summary>
/// JGJN發票 Repository 介面
/// </summary>
public interface IJgjNInvoiceRepository
{
    Task<JgjNInvoice?> GetByIdAsync(long tKey);
    Task<JgjNInvoice?> GetByInvoiceIdAsync(string invoiceId);
    Task<IEnumerable<JgjNInvoice>> QueryAsync(JgjNInvoiceQuery query);
    Task<int> GetCountAsync(JgjNInvoiceQuery query);
    Task<long> CreateAsync(JgjNInvoice entity);
    Task UpdateAsync(JgjNInvoice entity);
    Task DeleteAsync(long tKey);
    Task UpdatePrintStatusAsync(long tKey, string printStatus, DateTime? printDate, string? filePath);
}

/// <summary>
/// JGJN發票查詢條件
/// </summary>
public class JgjNInvoiceQuery
{
    public string? CustomerId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Status { get; set; }
    public string? PrintStatus { get; set; }
    public string? Keyword { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

