namespace ErpCore.Application.DTOs.DropdownList;

/// <summary>
/// 選單 DTO
/// </summary>
public class MenuDto
{
    public string MenuId { get; set; } = string.Empty;
    public string MenuName { get; set; } = string.Empty;
    public string? SystemId { get; set; }
    public string? ParentMenuId { get; set; }
    public int? SeqNo { get; set; }
    public string? Icon { get; set; }
    public string? Url { get; set; }
    public string Status { get; set; } = "1";
}

/// <summary>
/// 選單查詢 DTO
/// </summary>
public class MenuQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? SortField { get; set; } = "MenuName";
    public string? SortOrder { get; set; } = "ASC";
    public string? MenuName { get; set; }
    public string? SystemId { get; set; }
    public string? Status { get; set; } = "1";
}

/// <summary>
/// 選單選項 DTO (用於下拉選單)
/// </summary>
public class MenuOptionDto
{
    public string Value { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
}

