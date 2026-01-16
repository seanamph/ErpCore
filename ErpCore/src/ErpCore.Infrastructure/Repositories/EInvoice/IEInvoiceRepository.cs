using ErpCore.Domain.Entities.EInvoice;
using ErpCore.Shared.Common;
using EInvoiceEntity = ErpCore.Domain.Entities.EInvoice.EInvoice;

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
    Task<List<EInvoiceEntity>> GetEInvoicesByUploadIdAsync(long uploadId);
    Task<PagedResult<EInvoiceEntity>> GetEInvoicesPagedAsync(EInvoiceQuery query);
    Task<PagedResult<EInvoiceWithNamesDto>> GetEInvoicesWithNamesPagedAsync(EInvoiceQuery query);
    Task<EInvoiceEntity?> GetEInvoiceByIdAsync(long invoiceId);
    Task<long> CreateEInvoiceAsync(EInvoiceEntity invoice);
    Task UpdateEInvoiceAsync(EInvoiceEntity invoice);
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
    public string? StoreFloor { get; set; } // ECA4040: 樓層
    public string? StoreType { get; set; } // ECA4040: 類型
    public string? RetailerId { get; set; } // ECA4010: 零售商ID
    public string? ScId { get; set; } // ECA4010: 專櫃ID
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
/// 電子發票查詢結果 DTO（包含 StoreName 和 ProviderName）
/// </summary>
public class EInvoiceWithNamesDto
{
    public long InvoiceId { get; set; }
    public long? UploadId { get; set; }
    public string? OrderNo { get; set; }
    public string? RetailerOrderNo { get; set; }
    public string? RetailerOrderDetailNo { get; set; }
    public DateTime? OrderDate { get; set; }
    public string? StoreId { get; set; }
    public string? StoreName { get; set; }
    public string? ProviderId { get; set; }
    public string? ProviderName { get; set; }
    public string? NdType { get; set; }
    public string? GoodsId { get; set; }
    public string? GoodsName { get; set; }
    public string? SpecId { get; set; }
    public string? ProviderGoodsId { get; set; }
    public string? SpecColor { get; set; }
    public string? SpecSize { get; set; }
    public decimal? SuggestPrice { get; set; }
    public decimal? InternetPrice { get; set; }
    public string? ShippingType { get; set; }
    public decimal? ShippingFee { get; set; }
    public int? OrderQty { get; set; }
    public decimal? OrderShippingFee { get; set; }
    public decimal? OrderSubtotal { get; set; }
    public decimal? OrderTotal { get; set; }
    public string? OrderStatus { get; set; }
    public string ProcessStatus { get; set; } = "PENDING";
    public string? ErrorMessage { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
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
    // ECA4020 專用欄位（使用現有欄位存儲）
    public string? ScId { get; set; }
    public string? ScName { get; set; }
    public decimal? AvgPrice { get; set; }
    public decimal? SalesPercent { get; set; }
    public int? SalesRanking { get; set; }
    // ECA4040 專用欄位
    public string? StoreFloor { get; set; }
    public string? StoreType { get; set; }
    // ECA4060 專用欄位（已開立/未開立/全部統計）
    public int? SumYQty { get; set; }
    public decimal? SumYSubtotal { get; set; }
    public decimal? SumYFee { get; set; }
    public decimal? SalesYPercent { get; set; }
    public int? SumNQty { get; set; }
    public decimal? SumNSubtotal { get; set; }
    public decimal? SumNFee { get; set; }
    public decimal? SalesNPercent { get; set; }
    public int? SumAQty { get; set; }
    public decimal? SumASubtotal { get; set; }
    public decimal? SumAFee { get; set; }
    public decimal? SalesAPercent { get; set; }
}

/// <summary>
/// 電子發票商品銷售統計報表 DTO (ECA4020) - Repository 層
/// </summary>
public class ECA4020ReportDto
{
    public string? ScId { get; set; }
    public string? ScName { get; set; }
    public string? GoodsId { get; set; }
    public string? GoodsName { get; set; }
    public string? ProviderGoodsId { get; set; }
    public int SumQty { get; set; }
    public decimal SumSubtotal { get; set; }
    public decimal SumFee { get; set; }
    public decimal AvgPrice { get; set; }
    public decimal SalesPercent { get; set; }
    public int SalesRanking { get; set; }
}

/// <summary>
/// 電子發票零售商銷售統計報表 DTO (ECA4030) - Repository 層
/// </summary>
public class ECA4030ReportDto
{
    public string? RetailerId { get; set; }
    public string? RetailerName { get; set; }
    public int SumQty { get; set; }
    public decimal SumSubtotal { get; set; }
    public decimal SumFee { get; set; }
    public decimal SalesPercent { get; set; }
    public int SalesRanking { get; set; }
}

