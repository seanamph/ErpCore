using ErpCore.Shared.Common;

namespace ErpCore.Application.DTOs.AnalysisReport;

/// <summary>
/// 耗材列印項目 DTO (SYSA254)
/// </summary>
public class ConsumablePrintItemDto
{
    public string ConsumableId { get; set; } = string.Empty;
    public string ConsumableName { get; set; } = string.Empty;
    public string? BarCode { get; set; }
    public string? CategoryName { get; set; }
    public string? Unit { get; set; }
    public string? Specification { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string Status { get; set; } = "1";
    public string? StatusName { get; set; }
    public string? AssetStatus { get; set; }
    public string? SiteId { get; set; }
    public string? SiteName { get; set; }
    public string? Location { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? Price { get; set; }
}

/// <summary>
/// 耗材列印列表 DTO
/// </summary>
public class ConsumablePrintListDto
{
    public List<ConsumablePrintItemDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
}

/// <summary>
/// 耗材列印查詢 DTO
/// </summary>
public class ConsumablePrintQueryDto
{
    public string Type { get; set; } = "2"; // 1:耗材管理報表, 2:耗材標籤列印
    public string? Status { get; set; }
    public string? SiteId { get; set; }
    public string? AssetStatus { get; set; }
    public List<string>? ConsumableIds { get; set; }
}

/// <summary>
/// 批次列印 DTO
/// </summary>
public class BatchPrintDto
{
    public string Type { get; set; } = "2";
    public List<string> ConsumableIds { get; set; } = new();
    public int PrintCount { get; set; } = 1;
    public string? SiteId { get; set; }
}

/// <summary>
/// 批次列印回應 DTO
/// </summary>
public class BatchPrintResponseDto
{
    public Guid PrintLogId { get; set; }
    public int PrintCount { get; set; }
}

/// <summary>
/// 列印記錄 DTO
/// </summary>
public class ConsumablePrintLogDto
{
    public Guid LogId { get; set; }
    public string ConsumableId { get; set; } = string.Empty;
    public string? ConsumableName { get; set; }
    public string? PrintType { get; set; }
    public string? PrintTypeName { get; set; }
    public int PrintCount { get; set; }
    public DateTime PrintDate { get; set; }
    public string? PrintedBy { get; set; }
    public string? PrintedByName { get; set; }
    public string? SiteId { get; set; }
    public string? SiteName { get; set; }
}

/// <summary>
/// 列印記錄查詢 DTO
/// </summary>
public class ConsumablePrintLogQueryDto : PagedQuery
{
    public string? ConsumableId { get; set; }
    public string? PrintType { get; set; }
    public string? SiteId { get; set; }
    public string? PrintedBy { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
