namespace ErpCore.Application.DTOs.System;

/// <summary>
/// 子系統項目資料傳輸物件 (SYS0420)
/// </summary>
public class MenuDto
{
    public long TKey { get; set; }
    public string MenuId { get; set; } = string.Empty;
    public string MenuName { get; set; } = string.Empty;
    public int? SeqNo { get; set; }
    public string SystemId { get; set; } = string.Empty;
    public string? SystemName { get; set; }
    public string? ParentMenuId { get; set; }
    public string? ParentMenuName { get; set; }
    public string Status { get; set; } = "1";
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 子系統項目查詢 DTO
/// </summary>
public class MenuQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string SortOrder { get; set; } = "ASC";
    public MenuQueryFilters? Filters { get; set; }
}

/// <summary>
/// 子系統查詢篩選條件
/// </summary>
public class MenuQueryFilters
{
    public string? MenuId { get; set; }
    public string? MenuName { get; set; }
    public string? SystemId { get; set; }
    public string? ParentMenuId { get; set; }
}

/// <summary>
/// 新增子系統項目 DTO
/// </summary>
public class CreateMenuDto
{
    public string MenuId { get; set; } = string.Empty;
    public string MenuName { get; set; } = string.Empty;
    public int SeqNo { get; set; }
    public string SystemId { get; set; } = string.Empty;
    public string ParentMenuId { get; set; } = "0";
}

/// <summary>
/// 修改子系統項目 DTO
/// </summary>
public class UpdateMenuDto
{
    public string MenuName { get; set; } = string.Empty;
    public int SeqNo { get; set; }
    public string SystemId { get; set; } = string.Empty;
    public string ParentMenuId { get; set; } = "0";
}

/// <summary>
/// 批次刪除 DTO
/// </summary>
public class BatchDeleteMenusDto
{
    public List<string> MenuIds { get; set; } = new();
}
