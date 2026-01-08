namespace ErpCore.Application.DTOs.System;

/// <summary>
/// 項目對應查詢 DTO (SYS0360)
/// </summary>
public class ItemCorrespondQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? ItemId { get; set; }
    public string? ItemName { get; set; }
    public string? ItemType { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 項目對應 DTO (SYS0360)
/// </summary>
public class ItemCorrespondDto
{
    public string ItemId { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string? ItemType { get; set; }
    public string Status { get; set; } = "1";
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// 新增項目對應 DTO (SYS0360)
/// </summary>
public class CreateItemCorrespondDto
{
    public string ItemId { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string? ItemType { get; set; }
    public string Status { get; set; } = "1";
    public string? Notes { get; set; }
}

/// <summary>
/// 修改項目對應 DTO (SYS0360)
/// </summary>
public class UpdateItemCorrespondDto
{
    public string ItemName { get; set; } = string.Empty;
    public string? ItemType { get; set; }
    public string Status { get; set; } = "1";
    public string? Notes { get; set; }
}

/// <summary>
/// 項目系統列表 DTO (SYS0360)
/// </summary>
public class ItemSystemDto
{
    public string SystemId { get; set; } = string.Empty;
    public string SystemName { get; set; } = string.Empty;
    public int PermissionCount { get; set; }
    public int TotalCount { get; set; }
    public string Status { get; set; } = string.Empty; // 全選/部份/未選
}

/// <summary>
/// 項目選單列表 DTO (SYS0360)
/// </summary>
public class ItemMenuDto
{
    public string MenuId { get; set; } = string.Empty;
    public string MenuName { get; set; } = string.Empty;
    public int PermissionCount { get; set; }
    public int TotalCount { get; set; }
    public string Status { get; set; } = string.Empty; // 全選/部份/未選
}

/// <summary>
/// 項目作業列表 DTO (SYS0360)
/// </summary>
public class ItemProgramDto
{
    public string ProgramId { get; set; } = string.Empty;
    public string ProgramName { get; set; } = string.Empty;
    public int PermissionCount { get; set; }
    public int TotalCount { get; set; }
    public string Status { get; set; } = string.Empty; // 全選/部份/未選
}

/// <summary>
/// 項目按鈕列表 DTO (SYS0360)
/// </summary>
public class ItemButtonDto
{
    public long ButtonKey { get; set; }
    public string ButtonId { get; set; } = string.Empty;
    public string ButtonName { get; set; } = string.Empty;
    public bool IsAuthorized { get; set; }
}

/// <summary>
/// 項目權限 DTO (SYS0360)
/// </summary>
public class ItemPermissionDto
{
    public long TKey { get; set; }
    public string ItemId { get; set; } = string.Empty;
    public string ProgramId { get; set; } = string.Empty;
    public string PageId { get; set; } = string.Empty;
    public string ButtonId { get; set; } = string.Empty;
    public long? ButtonKey { get; set; }
    public string? SystemId { get; set; }
    public string? SystemName { get; set; }
    public string? SubSystemId { get; set; }
    public string? SubSystemName { get; set; }
    public string? ProgramName { get; set; }
    public string? ButtonName { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// 項目權限查詢 DTO (SYS0360)
/// </summary>
public class ItemPermissionQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? SystemId { get; set; }
    public string? MenuId { get; set; }
    public string? ProgramId { get; set; }
    public long? ButtonKey { get; set; }
}

/// <summary>
/// 設定項目系統權限 DTO (SYS0360)
/// </summary>
public class SetItemSystemPermissionDto
{
    public List<string> SystemIds { get; set; } = new();
    public string Action { get; set; } = string.Empty; // grant: 授予權限, revoke: 撤銷權限
}

/// <summary>
/// 設定項目選單權限 DTO (SYS0360)
/// </summary>
public class SetItemMenuPermissionDto
{
    public List<string> MenuIds { get; set; } = new();
    public string Action { get; set; } = string.Empty; // grant: 授予權限, revoke: 撤銷權限
}

/// <summary>
/// 設定項目作業權限 DTO (SYS0360)
/// </summary>
public class SetItemProgramPermissionDto
{
    public List<string> ProgramIds { get; set; } = new();
    public string Action { get; set; } = string.Empty; // grant: 授予權限, revoke: 撤銷權限
}

/// <summary>
/// 設定項目按鈕權限 DTO (SYS0360)
/// </summary>
public class SetItemButtonPermissionDto
{
    public List<long> ButtonKeys { get; set; } = new();
    public string Action { get; set; } = string.Empty; // grant: 授予權限, revoke: 撤銷權限
}

