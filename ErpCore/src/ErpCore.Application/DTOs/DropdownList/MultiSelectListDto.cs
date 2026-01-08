namespace ErpCore.Application.DTOs.DropdownList;

/// <summary>
/// 多選區域 DTO
/// </summary>
public class MultiAreaDto
{
    public string AreaId { get; set; } = string.Empty;
    public string AreaName { get; set; } = string.Empty;
    public int? SeqNo { get; set; }
    public string Status { get; set; } = "A";
}

/// <summary>
/// 多選區域查詢 DTO
/// </summary>
public class MultiAreaQueryDto
{
    public string? AreaName { get; set; }
    public string? Status { get; set; } = "A";
    public string? SortField { get; set; } = "AreaId";
    public string? SortOrder { get; set; } = "ASC";
}

/// <summary>
/// 多選店別 DTO
/// </summary>
public class MultiShopDto
{
    public string ShopId { get; set; } = string.Empty;
    public string ShopName { get; set; } = string.Empty;
    public string? RegionId { get; set; }
    public string? RegionName { get; set; }
    public string? TypeId { get; set; }
    public string? TypeName { get; set; }
    public string? ShopLevel { get; set; }
    public string? ShopLevelName { get; set; }
    public string Status { get; set; } = "1";
}

/// <summary>
/// 多選店別查詢 DTO
/// </summary>
public class MultiShopQueryDto
{
    public string? ShopName { get; set; }
    public string? RegionIds { get; set; }
    public string? TypeIds { get; set; }
    public string? ShopLevels { get; set; }
    public string? Status { get; set; } = "1";
    public string? Kind { get; set; }
    public string? GoodsId { get; set; }
    public string? AllShop { get; set; }
    public string? SortField { get; set; } = "ShopId";
    public string? SortOrder { get; set; } = "ASC";
}

/// <summary>
/// 多選使用者 DTO
/// </summary>
public class MultiUserDto
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? OrgId { get; set; }
    public string? OrgName { get; set; }
    public string? Title { get; set; }
    public string Status { get; set; } = "A";
}

/// <summary>
/// 多選使用者查詢 DTO
/// </summary>
public class MultiUserQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? OrgId { get; set; }
    public string? Status { get; set; } = "A";
    public string? SortField { get; set; } = "UserId";
    public string? SortOrder { get; set; } = "ASC";
}

/// <summary>
/// 選項 DTO (用於下拉選單)
/// </summary>
public class OptionDto
{
    public string Value { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
}

