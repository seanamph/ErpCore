using ErpCore.Domain.Entities.InvoiceExtension;

namespace ErpCore.Infrastructure.Repositories.InvoiceExtension;

/// <summary>
/// 電子發票擴展 Repository 介面
/// </summary>
public interface IEInvoiceExtensionRepository
{
    Task<EInvoiceExtension?> GetByIdAsync(long extensionId);
    Task<IEnumerable<EInvoiceExtension>> QueryAsync(EInvoiceExtensionQuery query);
    Task<int> GetCountAsync(EInvoiceExtensionQuery query);
    Task<long> CreateAsync(EInvoiceExtension entity);
    Task UpdateAsync(EInvoiceExtension entity);
    Task DeleteAsync(long extensionId);
}

/// <summary>
/// 查詢條件
/// </summary>
public class EInvoiceExtensionQuery
{
    public long? InvoiceId { get; set; }
    public string? ExtensionType { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

