using ErpCore.Shared.Common;

namespace ErpCore.Application.DTOs.AnalysisReport;

/// <summary>
/// 耗材出售單 DTO (SYSA297)
/// </summary>
public class ConsumableSalesDto
{
    public string TxnNo { get; set; } = string.Empty;
    public Guid Rrn { get; set; }
    public string SiteId { get; set; } = string.Empty;
    public string? SiteName { get; set; }
    public DateTime PurchaseDate { get; set; }
    public string Status { get; set; } = "1";
    public string? StatusName { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal NetAmount { get; set; }
    public int DetailCount { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
}

/// <summary>
/// 耗材出售單詳細 DTO
/// </summary>
public class ConsumableSalesDetailDto : ConsumableSalesDto
{
    public int ApplyCount { get; set; }
    public string? Notes { get; set; }
    public List<ConsumableSalesDetailItemDto> Details { get; set; } = new();
}

/// <summary>
/// 耗材出售單明細項目 DTO
/// </summary>
public class ConsumableSalesDetailItemDto
{
    public Guid DetailId { get; set; }
    public int SeqNo { get; set; }
    public string ConsumableId { get; set; } = string.Empty;
    public string? ConsumableName { get; set; }
    public decimal Quantity { get; set; }
    public string? Unit { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Amount { get; set; }
    public string Tax { get; set; } = "1";
    public decimal TaxAmount { get; set; }
    public decimal NetAmount { get; set; }
    public string PurchaseStatus { get; set; } = "1";
    public string? Notes { get; set; }
}

/// <summary>
/// 新增耗材出售單 DTO
/// </summary>
public class CreateConsumableSalesDto
{
    public string SiteId { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; }
    public string? Notes { get; set; }
    public List<CreateConsumableSalesDetailDto> Details { get; set; } = new();
}

/// <summary>
/// 新增耗材出售單明細 DTO
/// </summary>
public class CreateConsumableSalesDetailDto
{
    public string ConsumableId { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string Tax { get; set; } = "1";
    public string? Notes { get; set; }
}

/// <summary>
/// 更新耗材出售單 DTO
/// </summary>
public class UpdateConsumableSalesDto
{
    public string? Notes { get; set; }
    public List<CreateConsumableSalesDetailDto> Details { get; set; } = new();
}

/// <summary>
/// 審核耗材出售單 DTO
/// </summary>
public class ApproveSalesDto
{
    public bool Approved { get; set; } = true;
    public string? Notes { get; set; }
}

/// <summary>
/// 耗材出售單查詢 DTO
/// </summary>
public class ConsumableSalesQueryDto : PagedQuery
{
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public ConsumableSalesQueryFilters? Filters { get; set; }
}

/// <summary>
/// 耗材出售單查詢篩選條件
/// </summary>
public class ConsumableSalesQueryFilters
{
    public string? TxnNo { get; set; }
    public string? SiteId { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
