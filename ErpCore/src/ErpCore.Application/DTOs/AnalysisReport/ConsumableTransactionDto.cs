using ErpCore.Shared.Common;

namespace ErpCore.Application.DTOs.AnalysisReport;

/// <summary>
/// 耗材異動記錄 DTO (SYSA255)
/// </summary>
public class ConsumableTransactionDto
{
    public long TransactionId { get; set; }
    public string ConsumableId { get; set; } = string.Empty;
    public string TransactionType { get; set; } = string.Empty;
    public string? TransactionTypeName { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? Amount { get; set; }
    public string? SiteId { get; set; }
    public string? SiteName { get; set; }
    public string? WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public string? SourceId { get; set; }
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 耗材異動記錄查詢 DTO
/// </summary>
public class ConsumableTransactionQueryDto : PagedQuery
{
    public string ConsumableId { get; set; } = string.Empty;
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? TransactionType { get; set; }
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}
