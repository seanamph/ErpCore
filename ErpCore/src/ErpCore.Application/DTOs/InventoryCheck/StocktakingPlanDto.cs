namespace ErpCore.Application.DTOs.InventoryCheck;

/// <summary>
/// 盤點計劃 DTO
/// </summary>
public class StocktakingPlanDto
{
    public string PlanId { get; set; } = string.Empty;
    public DateTime PlanDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? SakeType { get; set; }
    public string? SakeDept { get; set; }
    public string PlanStatus { get; set; } = string.Empty;
    public string? PlanStatusName { get; set; }
    public string? SiteId { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<StocktakingPlanShopDto> Shops { get; set; } = new();
    public List<StocktakingDetailDto> Details { get; set; } = new();
    public int ShopCount { get; set; }
    public decimal TotalDiffQty { get; set; }
    public decimal TotalDiffAmount { get; set; }
}

/// <summary>
/// 盤點計劃店舖 DTO
/// </summary>
public class StocktakingPlanShopDto
{
    public long TKey { get; set; }
    public string PlanId { get; set; } = string.Empty;
    public string ShopId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? StatusName { get; set; }
    public string? InvStatus { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 盤點明細 DTO
/// </summary>
public class StocktakingDetailDto
{
    public Guid DetailId { get; set; }
    public string PlanId { get; set; } = string.Empty;
    public string ShopId { get; set; } = string.Empty;
    public string GoodsId { get; set; } = string.Empty;
    public decimal BookQty { get; set; }
    public decimal PhysicalQty { get; set; }
    public decimal DiffQty { get; set; }
    public decimal? UnitCost { get; set; }
    public decimal? DiffAmount { get; set; }
    public string? Kind { get; set; }
    public string? ShelfNo { get; set; }
    public int? SerialNo { get; set; }
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 建立盤點計劃 DTO
/// </summary>
public class CreateStocktakingPlanDto
{
    public DateTime PlanDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? SakeType { get; set; }
    public string? SakeDept { get; set; }
    public string? SiteId { get; set; }
    public List<string> ShopIds { get; set; } = new();
}

/// <summary>
/// 修改盤點計劃 DTO
/// </summary>
public class UpdateStocktakingPlanDto
{
    public DateTime PlanDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? SakeType { get; set; }
    public string? SakeDept { get; set; }
    public string? SiteId { get; set; }
    public List<string> ShopIds { get; set; } = new();
}

/// <summary>
/// 查詢盤點計劃 DTO
/// </summary>
public class StocktakingPlanQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? PlanId { get; set; }
    public DateTime? PlanDateFrom { get; set; }
    public DateTime? PlanDateTo { get; set; }
    public string? PlanStatus { get; set; }
    public string? ShopId { get; set; }
    public string? SakeType { get; set; }
}

/// <summary>
/// 盤點報表查詢 DTO
/// </summary>
public class StocktakingReportQueryDto
{
    public string ReportType { get; set; } = "SUMMARY"; // SUMMARY:彙總, DETAIL:明細
    public string? ShopId { get; set; }
    public string? GoodsId { get; set; }
    public bool IncludeZero { get; set; } = false;
}

/// <summary>
/// 盤點報表 DTO
/// </summary>
public class StocktakingReportDto
{
    public string PlanId { get; set; } = string.Empty;
    public DateTime PlanDate { get; set; }
    public List<StocktakingReportSummaryDto> Summary { get; set; } = new();
    public List<StocktakingDetailDto> Details { get; set; } = new();
}

/// <summary>
/// 盤點報表彙總 DTO
/// </summary>
public class StocktakingReportSummaryDto
{
    public string ShopId { get; set; } = string.Empty;
    public string? ShopName { get; set; }
    public int GoodsCount { get; set; }
    public decimal TotalBookQty { get; set; }
    public decimal TotalPhysicalQty { get; set; }
    public decimal TotalDiffQty { get; set; }
    public decimal TotalDiffAmount { get; set; }
}

