namespace ErpCore.Application.DTOs.StoreFloor;

/// <summary>
/// 商店查詢 DTO (SYS6210-SYS6270 - 商店查詢作業)
/// </summary>
public class StoreQueryDto
{
    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每頁筆數
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// 排序欄位
    /// </summary>
    public string? SortField { get; set; }

    /// <summary>
    /// 排序方向 (ASC/DESC)
    /// </summary>
    public string? SortOrder { get; set; }

    /// <summary>
    /// 查詢條件
    /// </summary>
    public StoreQueryFilters? Filters { get; set; }
}

/// <summary>
/// 商店查詢條件
/// </summary>
public class StoreQueryFilters
{
    /// <summary>
    /// 商店編號
    /// </summary>
    public string? ShopId { get; set; }

    /// <summary>
    /// 商店名稱
    /// </summary>
    public string? ShopName { get; set; }

    /// <summary>
    /// 商店類型
    /// </summary>
    public string? ShopType { get; set; }

    /// <summary>
    /// 狀態
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// 區域
    /// </summary>
    public string? Zone { get; set; }

    /// <summary>
    /// 樓層代碼
    /// </summary>
    public string? FloorId { get; set; }

    /// <summary>
    /// POS啟用
    /// </summary>
    public bool? PosEnabled { get; set; }

    /// <summary>
    /// 開店日期起
    /// </summary>
    public DateTime? OpenDateFrom { get; set; }

    /// <summary>
    /// 開店日期迄
    /// </summary>
    public DateTime? OpenDateTo { get; set; }

    /// <summary>
    /// 關鍵字
    /// </summary>
    public string? Keyword { get; set; }
}

/// <summary>
/// 商店匯出 DTO
/// </summary>
public class StoreExportDto
{
    /// <summary>
    /// 匯出格式 (Excel/PDF)
    /// </summary>
    public string Format { get; set; } = "Excel";

    /// <summary>
    /// 查詢條件
    /// </summary>
    public StoreQueryFilters? Filters { get; set; }
}

