using ErpCore.Domain.Entities.EInvoice;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.EInvoice;

/// <summary>
/// 電子發票 Repository 介面
/// </summary>
public interface IEInvoiceRepository
{
    Task<EInvoiceUpload?> GetUploadByIdAsync(long uploadId);
    Task<PagedResult<EInvoiceUpload>> GetUploadsPagedAsync(EInvoiceUploadQuery query);
    Task<long> CreateUploadAsync(EInvoiceUpload upload);
    Task UpdateUploadAsync(EInvoiceUpload upload);
    Task DeleteUploadAsync(long uploadId);
    Task<List<EInvoice>> GetEInvoicesByUploadIdAsync(long uploadId);
    Task<PagedResult<EInvoice>> GetEInvoicesPagedAsync(EInvoiceQuery query);
    Task<EInvoice?> GetEInvoiceByIdAsync(long invoiceId);
    Task<long> CreateEInvoiceAsync(EInvoice invoice);
    Task UpdateEInvoiceAsync(EInvoice invoice);
    Task<PagedResult<EInvoiceReportDto>> GetEInvoiceReportsPagedAsync(EInvoiceReportQuery query);
}

/// <summary>
/// 電子發票上傳記錄查詢條件
/// </summary>
public class EInvoiceUploadQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? Status { get; set; }
    public string? UploadBy { get; set; }
    public string? StoreId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? UploadType { get; set; }
}

/// <summary>
/// 電子發票查詢條件 (ECA3020)
/// </summary>
public class EInvoiceQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public long? UploadId { get; set; }
    public string? OrderNo { get; set; }
    public string? RetailerOrderNo { get; set; }
    public string? RetailerOrderDetailNo { get; set; }
    public string? StoreId { get; set; }
    public string? ProviderId { get; set; }
    public DateTime? OrderDateFrom { get; set; }
    public DateTime? OrderDateTo { get; set; }
    public string? ProcessStatus { get; set; }
    public string? OrderStatus { get; set; }
    public string? GoodsId { get; set; }
    public string? GoodsName { get; set; }
    public string? SpecId { get; set; }
    public string? ProviderGoodsId { get; set; }
    public string? NdType { get; set; }
}

/// <summary>
/// 電子發票報表查詢條件 (ECA3040, ECA4010-ECA4060)
/// </summary>
public class EInvoiceReportQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? ReportType { get; set; } // ECA3040, ECA4010, ECA4020, ECA4030, ECA4040, ECA4050, ECA4060
    public string? OrderNo { get; set; }
    public string? RetailerOrderNo { get; set; }
    public string? StoreId { get; set; }
    public string? ProviderId { get; set; }
    public string? GoodsId { get; set; }
    public string? GoodsName { get; set; }
    public DateTime? OrderDateFrom { get; set; }
    public DateTime? OrderDateTo { get; set; }
    public DateTime? ShipDateFrom { get; set; }
    public DateTime? ShipDateTo { get; set; }
    public string? OrderStatus { get; set; }
    public string? ProcessStatus { get; set; }
}

/// <summary>
/// 電子發票報表 DTO
/// </summary>
public class EInvoiceReportDto
{
    public long InvoiceId { get; set; }
    public string? OrderNo { get; set; }
    public DateTime? OrderDate { get; set; }
    public DateTime? ShipDate { get; set; }
    public string? OrderStatus { get; set; }
    public int? OrderQty { get; set; }
    public decimal? OrderSubtotal { get; set; }
    public decimal? OrderTotal { get; set; }
    public string? StoreId { get; set; }
    public string? StoreName { get; set; }
    public string? RetailerId { get; set; }
    public string? RetailerName { get; set; }
    public string? ProviderId { get; set; }
    public string? ProviderName { get; set; }
    public string? GoodsId { get; set; }
    public string? GoodsName { get; set; }
    public string? SpecId { get; set; }
    public string? ProviderGoodsId { get; set; }
    public decimal? SuggestPrice { get; set; }
    public decimal? InternetPrice { get; set; }
    public decimal? ShippingFee { get; set; }
    public string? ShippingType { get; set; }
    // 統計欄位
    public int? TotalQty { get; set; }
    public decimal? TotalAmount { get; set; }
    public int? TotalOrders { get; set; }
}

