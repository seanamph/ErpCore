namespace ErpCore.Application.DTOs.StoreFloor;

/// <summary>
/// POS終端查詢請求 DTO
/// </summary>
public class PosTerminalQueryRequestDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public PosTerminalQueryFilters? Filters { get; set; }
}

/// <summary>
/// POS終端查詢篩選條件
/// </summary>
public class PosTerminalQueryFilters
{
    public string? PosTerminalId { get; set; }
    public string? PosSystemId { get; set; }
    public string? ShopId { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// POS終端統計請求 DTO
/// </summary>
public class PosTerminalStatisticsRequestDto
{
    public string? PosSystemId { get; set; }
    public bool IncludeTransactionDetails { get; set; } = false;
}

/// <summary>
/// POS終端統計結果 DTO
/// </summary>
public class PosTerminalStatisticsDto
{
    public int TotalTerminals { get; set; }
    public int ActiveTerminals { get; set; }
    public List<PosTerminalStatisticsItemDto> TerminalStatistics { get; set; } = new();
}

/// <summary>
/// POS終端統計項目 DTO
/// </summary>
public class PosTerminalStatisticsItemDto
{
    public string PosTerminalId { get; set; } = string.Empty;
    public string TerminalName { get; set; } = string.Empty;
    public string? ShopId { get; set; }
    public int TransactionCount { get; set; }
    public DateTime? LastSyncDate { get; set; }
}

