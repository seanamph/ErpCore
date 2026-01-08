namespace ErpCore.Application.DTOs.DropdownList;

/// <summary>
/// 區域 DTO
/// </summary>
public class ZoneDto
{
    public string ZoneId { get; set; } = string.Empty;
    public string ZoneName { get; set; } = string.Empty;
    public string? CityId { get; set; }
    public int? SeqNo { get; set; }
    public string Status { get; set; } = "1";
}

/// <summary>
/// 區域查詢 DTO
/// </summary>
public class ZoneQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? SortField { get; set; } = "ZoneName";
    public string? SortOrder { get; set; } = "ASC";
    public string? ZoneName { get; set; }
    public string? CityId { get; set; }
    public string? Status { get; set; } = "1";
}

/// <summary>
/// 區域選項 DTO (用於下拉選單)
/// </summary>
public class ZoneOptionDto
{
    public string Value { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
}

