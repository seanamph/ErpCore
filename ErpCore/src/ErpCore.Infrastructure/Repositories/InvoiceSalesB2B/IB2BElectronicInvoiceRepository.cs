using ErpCore.Domain.Entities.InvoiceSalesB2B;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.InvoiceSalesB2B;

/// <summary>
/// B2B電子發票 Repository 接口 (SYSG000_B2B - B2B電子發票列印)
/// </summary>
public interface IB2BElectronicInvoiceRepository
{
    /// <summary>
    /// 根據主鍵查詢B2B電子發票
    /// </summary>
    Task<B2BElectronicInvoice?> GetByIdAsync(long tKey);

    /// <summary>
    /// 根據發票編號查詢B2B電子發票
    /// </summary>
    Task<B2BElectronicInvoice?> GetByInvoiceIdAsync(string invoiceId);

    /// <summary>
    /// 查詢B2B電子發票列表（分頁）
    /// </summary>
    Task<PagedResult<B2BElectronicInvoice>> QueryAsync(B2BElectronicInvoiceQuery query);

    /// <summary>
    /// 新增B2B電子發票
    /// </summary>
    Task<long> CreateAsync(B2BElectronicInvoice electronicInvoice);

    /// <summary>
    /// 修改B2B電子發票
    /// </summary>
    Task<int> UpdateAsync(B2BElectronicInvoice electronicInvoice);

    /// <summary>
    /// 刪除B2B電子發票
    /// </summary>
    Task<int> DeleteAsync(long tKey);

    /// <summary>
    /// 檢查發票編號是否存在
    /// </summary>
    Task<bool> ExistsByInvoiceIdAsync(string invoiceId, long? excludeTKey = null);

    /// <summary>
    /// 查詢中獎清冊
    /// </summary>
    Task<PagedResult<B2BElectronicInvoice>> QueryAwardListAsync(B2BAwardListQuery query);
}

/// <summary>
/// B2B電子發票查詢條件
/// </summary>
public class B2BElectronicInvoiceQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? InvoiceId { get; set; }
    public string? PosId { get; set; }
    public string? InvYm { get; set; }
    public string? Track { get; set; }
    public string? PrizeType { get; set; }
    public string? TransferType { get; set; }
    public string? Status { get; set; }
    public string? B2BFlag { get; set; } = "Y";
}

/// <summary>
/// B2B中獎清冊查詢條件
/// </summary>
public class B2BAwardListQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? InvYm { get; set; }
    public string? PrizeType { get; set; }
    public string? B2BFlag { get; set; } = "Y";
}

