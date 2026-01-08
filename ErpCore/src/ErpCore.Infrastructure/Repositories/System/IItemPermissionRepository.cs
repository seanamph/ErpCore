using ErpCore.Domain.Entities.System;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 項目權限 Repository 介面 (SYS0360)
/// </summary>
public interface IItemPermissionRepository
{
    /// <summary>
    /// 查詢項目權限列表（含關聯資訊）
    /// </summary>
    Task<PagedResult<ItemPermissionDto>> QueryPermissionsAsync(string itemId, ItemPermissionQuery query);

    /// <summary>
    /// 查詢項目系統列表及權限狀態
    /// </summary>
    Task<List<ItemSystemDto>> GetSystemListAsync(string itemId);

    /// <summary>
    /// 查詢項目選單列表及權限狀態
    /// </summary>
    Task<List<ItemMenuDto>> GetMenuListAsync(string itemId, string systemId);

    /// <summary>
    /// 查詢項目作業列表及權限狀態
    /// </summary>
    Task<List<ItemProgramDto>> GetProgramListAsync(string itemId, string menuId);

    /// <summary>
    /// 查詢項目按鈕列表及權限狀態
    /// </summary>
    Task<List<ItemButtonDto>> GetButtonListAsync(string itemId, string programId);

    /// <summary>
    /// 查詢項目權限（單筆）
    /// </summary>
    Task<ItemPermission?> GetByIdAsync(long tKey);

    /// <summary>
    /// 新增項目權限
    /// </summary>
    Task<ItemPermission> CreateAsync(ItemPermission itemPermission);

    /// <summary>
    /// 批量新增項目權限
    /// </summary>
    Task<int> BatchCreateAsync(string itemId, List<ItemPermissionItem> items, string? createdBy);

    /// <summary>
    /// 修改項目權限
    /// </summary>
    Task<ItemPermission> UpdateAsync(ItemPermission itemPermission);

    /// <summary>
    /// 刪除項目權限
    /// </summary>
    Task DeleteAsync(long tKey);

    /// <summary>
    /// 批量刪除項目權限
    /// </summary>
    Task<int> BatchDeleteAsync(List<long> tKeys);

    /// <summary>
    /// 根據系統ID批量設定權限
    /// </summary>
    Task<int> SetPermissionsBySystemAsync(string itemId, string systemId, bool isAuthorized, string? createdBy);

    /// <summary>
    /// 根據選單ID批量設定權限
    /// </summary>
    Task<int> SetPermissionsByMenuAsync(string itemId, string menuId, bool isAuthorized, string? createdBy);

    /// <summary>
    /// 根據作業ID批量設定權限
    /// </summary>
    Task<int> SetPermissionsByProgramAsync(string itemId, string programId, bool isAuthorized, string? createdBy);

    /// <summary>
    /// 根據按鈕Key批量設定權限
    /// </summary>
    Task<int> SetPermissionsByButtonAsync(string itemId, List<long> buttonKeys, bool isAuthorized, string? createdBy);
}

/// <summary>
/// 項目權限查詢條件
/// </summary>
public class ItemPermissionQuery
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
/// 項目權限 DTO（含關聯資訊）
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
/// 項目系統列表 DTO
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
/// 項目選單列表 DTO
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
/// 項目作業列表 DTO
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
/// 項目按鈕列表 DTO
/// </summary>
public class ItemButtonDto
{
    public long ButtonKey { get; set; }
    public string ButtonId { get; set; } = string.Empty;
    public string ButtonName { get; set; } = string.Empty;
    public bool IsAuthorized { get; set; }
}

/// <summary>
/// 項目權限項目
/// </summary>
public class ItemPermissionItem
{
    public string ProgramId { get; set; } = string.Empty;
    public string PageId { get; set; } = string.Empty;
    public string ButtonId { get; set; } = string.Empty;
    public long? ButtonKey { get; set; }
}

