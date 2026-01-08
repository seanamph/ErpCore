namespace ErpCore.Application.DTOs.DropdownList;

/// <summary>
/// 城市 DTO
/// </summary>
public class CityDto
{
    public string CityId { get; set; } = string.Empty;
    public string CityName { get; set; } = string.Empty;
    public string? CountryCode { get; set; }
    public int? SeqNo { get; set; }
    public string Status { get; set; } = "1";
}

/// <summary>
/// 城市查詢 DTO
/// </summary>
public class CityQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? SortField { get; set; } = "CityName";
    public string? SortOrder { get; set; } = "ASC";
    public string? CityName { get; set; }
    public string? CountryCode { get; set; }
    public string? Status { get; set; } = "1";
}

/// <summary>
/// 城市選項 DTO (用於下拉選單)
/// </summary>
public class CityOptionDto
{
    public string Value { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
}

