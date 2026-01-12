using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.AnalysisReport;

/// <summary>
/// 耗材管理報表 Repository 介面 (SYSA255)
/// </summary>
public interface IConsumableReportRepository
{
    /// <summary>
    /// 查詢耗材管理報表資料
    /// </summary>
    Task<PagedResult<ConsumableReportItem>> GetReportDataAsync(ConsumableReportQuery query);

    /// <summary>
    /// 查詢耗材管理報表統計資訊
    /// </summary>
    Task<ConsumableReportSummary> GetReportSummaryAsync(ConsumableReportQuery query);
}

/// <summary>
/// 耗材管理報表查詢條件
/// </summary>
public class ConsumableReportQuery : PagedQuery
{
    public string? ConsumableId { get; set; }
    public string? ConsumableName { get; set; }
    public string? CategoryId { get; set; }
    public List<string>? SiteIds { get; set; }
    public List<string>? WarehouseIds { get; set; }
    public string? Status { get; set; }
    public string? AssetStatus { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? ReportType { get; set; } // Summary, Detail, CostAnalysis
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

/// <summary>
/// 耗材管理報表項目
/// </summary>
public class ConsumableReportItem
{
    public string ConsumableId { get; set; } = string.Empty;
    public string ConsumableName { get; set; } = string.Empty;
    public string? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? SiteId { get; set; }
    public string? SiteName { get; set; }
    public string? WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public string? Unit { get; set; }
    public string? Specification { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? BarCode { get; set; }
    public string Status { get; set; } = "1";
    public string? StatusName { get; set; }
    public string? AssetStatus { get; set; }
    public string? AssetStatusName { get; set; }
    public string? Location { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? MinQuantity { get; set; }
    public decimal? MaxQuantity { get; set; }
    public decimal? Price { get; set; }
    public decimal? CurrentQty { get; set; }
    public decimal? CurrentAmt { get; set; }
    public decimal? InQty { get; set; }
    public decimal? OutQty { get; set; }
    public decimal? InAmt { get; set; }
    public decimal? OutAmt { get; set; }
    public bool IsLowStock { get; set; }
    public bool IsOverStock { get; set; }
}

/// <summary>
/// 耗材管理報表統計資訊
/// </summary>
public class ConsumableReportSummary
{
    public int TotalConsumables { get; set; }
    public decimal TotalCurrentQty { get; set; }
    public decimal TotalCurrentAmt { get; set; }
    public decimal TotalInQty { get; set; }
    public decimal TotalOutQty { get; set; }
    public decimal TotalInAmt { get; set; }
    public decimal TotalOutAmt { get; set; }
}
