namespace ErpCore.Application.DTOs.StoreFloor;

/// <summary>
/// 樓層查詢結果 DTO (SYS6381-SYS63A0 - 樓層查詢作業)
/// </summary>
public class FloorQueryDto
{
    /// <summary>
    /// 樓層代碼
    /// </summary>
    public string FloorId { get; set; } = string.Empty;

    /// <summary>
    /// 樓層名稱
    /// </summary>
    public string FloorName { get; set; } = string.Empty;

    /// <summary>
    /// 樓層英文名稱
    /// </summary>
    public string? FloorNameEn { get; set; }

    /// <summary>
    /// 樓層號碼
    /// </summary>
    public int? FloorNumber { get; set; }

    /// <summary>
    /// 說明
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";

    /// <summary>
    /// 商店數量
    /// </summary>
    public int ShopCount { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 樓層查詢請求 DTO
/// </summary>
public class FloorQueryRequestDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public FloorQueryFilters? Filters { get; set; }
}

/// <summary>
/// 樓層查詢篩選條件
/// </summary>
public class FloorQueryFilters
{
    public string? FloorId { get; set; }
    public string? FloorName { get; set; }
    public int? FloorNumber { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 樓層統計請求 DTO
/// </summary>
public class FloorStatisticsRequestDto
{
    public string? FloorId { get; set; }
    public bool IncludeShopDetails { get; set; } = false;
}

/// <summary>
/// 樓層統計結果 DTO
/// </summary>
public class FloorStatisticsDto
{
    public int TotalFloors { get; set; }
    public int ActiveFloors { get; set; }
    public int TotalShops { get; set; }
    public List<FloorStatisticsItemDto> FloorStatistics { get; set; } = new();
}

/// <summary>
/// 樓層統計項目 DTO
/// </summary>
public class FloorStatisticsItemDto
{
    public string FloorId { get; set; } = string.Empty;
    public string FloorName { get; set; } = string.Empty;
    public int ShopCount { get; set; }
    public int ActiveShopCount { get; set; }
}

/// <summary>
/// 樓層匯出 DTO
/// </summary>
public class FloorExportDto
{
    public FloorQueryFilters? Filters { get; set; }
    public string Format { get; set; } = "EXCEL"; // EXCEL, PDF
    public List<string> Columns { get; set; } = new();
}

