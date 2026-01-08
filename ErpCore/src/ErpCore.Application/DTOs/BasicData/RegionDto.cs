namespace ErpCore.Application.DTOs.BasicData;

/// <summary>
/// 地區 DTO
/// </summary>
public class RegionDto
{
    public string RegionId { get; set; } = string.Empty;
    public string RegionName { get; set; } = string.Empty;
    public string? Memo { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 地區查詢 DTO
/// </summary>
public class RegionQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? RegionId { get; set; }
    public string? RegionName { get; set; }
}

/// <summary>
/// 新增地區 DTO
/// </summary>
public class CreateRegionDto
{
    public string RegionId { get; set; } = string.Empty;
    public string RegionName { get; set; } = string.Empty;
    public string? Memo { get; set; }
}

/// <summary>
/// 修改地區 DTO
/// </summary>
public class UpdateRegionDto
{
    public string RegionName { get; set; } = string.Empty;
    public string? Memo { get; set; }
}

/// <summary>
/// 批次刪除地區 DTO
/// </summary>
public class BatchDeleteRegionDto
{
    public List<string> RegionIds { get; set; } = new();
}

