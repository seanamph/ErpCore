using ErpCore.Domain.Entities.InvoiceSales;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.InvoiceSales;

/// <summary>
/// 電子發票 Repository 接口 (SYSG210-SYSG2B0 - 電子發票列印)
/// </summary>
public interface IElectronicInvoiceRepository
{
    /// <summary>
    /// 根據主鍵查詢電子發票
    /// </summary>
    Task<ElectronicInvoice?> GetByIdAsync(long tKey);

    /// <summary>
    /// 查詢電子發票列表
    /// </summary>
    Task<PagedResult<ElectronicInvoice>> QueryAsync(ElectronicInvoiceQuery query);

    /// <summary>
    /// 新增電子發票
    /// </summary>
    Task<long> CreateAsync(ElectronicInvoice electronicInvoice);

    /// <summary>
    /// 修改電子發票
    /// </summary>
    Task<int> UpdateAsync(ElectronicInvoice electronicInvoice);

    /// <summary>
    /// 刪除電子發票
    /// </summary>
    Task<int> DeleteAsync(long tKey);

    /// <summary>
    /// 查詢中獎清冊
    /// </summary>
    Task<PagedResult<ElectronicInvoice>> QueryAwardListAsync(AwardListQuery query);
}

/// <summary>
/// 電子發票查詢參數
/// </summary>
public class ElectronicInvoiceQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? InvYm { get; set; }
    public string? Track { get; set; }
    public string? PosId { get; set; }
    public string? PrizeType { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 中獎清冊查詢參數
/// </summary>
public class AwardListQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? InvYm { get; set; }
    public string? PrizeType { get; set; }
}

