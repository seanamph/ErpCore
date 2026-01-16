namespace ErpCore.Shared.Common;

/// <summary>
/// 項目權限查詢結果模型 (用於 Repository 層，避免循環依賴)
/// </summary>
public class ItemPermissionQueryResult
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
/// 項目系統列表查詢結果模型
/// </summary>
public class ItemSystemQueryResult
{
    public string SystemId { get; set; } = string.Empty;
    public string SystemName { get; set; } = string.Empty;
    public int PermissionCount { get; set; }
    public int TotalCount { get; set; }
    public string Status { get; set; } = string.Empty; // 全選/部份/未選
}

/// <summary>
/// 項目選單列表查詢結果模型
/// </summary>
public class ItemMenuQueryResult
{
    public string MenuId { get; set; } = string.Empty;
    public string MenuName { get; set; } = string.Empty;
    public int PermissionCount { get; set; }
    public int TotalCount { get; set; }
    public string Status { get; set; } = string.Empty; // 全選/部份/未選
}

/// <summary>
/// 項目作業列表查詢結果模型
/// </summary>
public class ItemProgramQueryResult
{
    public string ProgramId { get; set; } = string.Empty;
    public string ProgramName { get; set; } = string.Empty;
    public int PermissionCount { get; set; }
    public int TotalCount { get; set; }
    public string Status { get; set; } = string.Empty; // 全選/部份/未選
}

/// <summary>
/// 項目按鈕列表查詢結果模型
/// </summary>
public class ItemButtonQueryResult
{
    public long ButtonKey { get; set; }
    public string ButtonId { get; set; } = string.Empty;
    public string ButtonName { get; set; } = string.Empty;
    public bool IsAuthorized { get; set; }
}

/// <summary>
/// 項目權限查詢條件模型
/// </summary>
public class ItemPermissionQueryModel
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
