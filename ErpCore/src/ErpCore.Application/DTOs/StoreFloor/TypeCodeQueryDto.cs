namespace ErpCore.Application.DTOs.StoreFloor;

/// <summary>
/// 類型代碼查詢結果 DTO (SYS6501-SYS6560 - 類型代碼查詢)
/// </summary>
public class TypeCodeQueryResultDto
{
    public long TKey { get; set; }
    public string TypeCode { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public string? TypeNameEn { get; set; }
    public string? Category { get; set; }
    public string Status { get; set; } = "A";
    public int UsageCount { get; set; } // 使用次數（統計用）
}

/// <summary>
/// 類型代碼查詢請求 DTO
/// </summary>
public class TypeCodeQueryRequestDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public TypeCodeQueryFilters? Filters { get; set; }
}

/// <summary>
/// 類型代碼查詢篩選條件
/// </summary>
public class TypeCodeQueryFilters
{
    public string? TypeCode { get; set; }
    public string? TypeName { get; set; }
    public string? Category { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 類型代碼統計請求 DTO
/// </summary>
public class TypeCodeStatisticsRequestDto
{
    public string? Category { get; set; }
    public bool IncludeUsageDetails { get; set; } = false;
}

/// <summary>
/// 類型代碼統計結果 DTO
/// </summary>
public class TypeCodeStatisticsDto
{
    public int TotalTypeCodes { get; set; }
    public int ActiveTypeCodes { get; set; }
    public List<TypeCodeCategoryStatisticsDto> CategoryStatistics { get; set; } = new();
}

/// <summary>
/// 類型代碼分類統計 DTO
/// </summary>
public class TypeCodeCategoryStatisticsDto
{
    public string? TypeCategory { get; set; }
    public int Count { get; set; }
    public int UsageCount { get; set; }
}

