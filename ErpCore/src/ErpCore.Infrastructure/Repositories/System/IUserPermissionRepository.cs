using ErpCore.Domain.Entities.System;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 使用者權限 Repository 介面 (SYS0320)
/// </summary>
public interface IUserPermissionRepository
{
    /// <summary>
    /// 查詢使用者權限列表（含關聯資訊）
    /// </summary>
    Task<PagedResult<UserPermissionDto>> QueryPermissionsAsync(string userId, UserPermissionQuery query);

    /// <summary>
    /// 查詢使用者權限統計（依系統）
    /// </summary>
    Task<List<UserPermissionStatsDto>> GetSystemStatsAsync(string userId);

    /// <summary>
    /// 查詢使用者權限（單筆）
    /// </summary>
    Task<UserButton?> GetByIdAsync(long tKey);

    /// <summary>
    /// 新增使用者權限
    /// </summary>
    Task<UserButton> CreateAsync(UserButton userButton);

    /// <summary>
    /// 批量新增使用者權限
    /// </summary>
    Task<int> BatchCreateAsync(string userId, List<string> buttonIds, string? createdBy);

    /// <summary>
    /// 修改使用者權限
    /// </summary>
    Task<UserButton> UpdateAsync(UserButton userButton);

    /// <summary>
    /// 刪除使用者權限
    /// </summary>
    Task DeleteAsync(long tKey);

    /// <summary>
    /// 批量刪除使用者權限
    /// </summary>
    Task<int> BatchDeleteAsync(List<long> tKeys);

    /// <summary>
    /// 檢查權限是否存在
    /// </summary>
    Task<bool> ExistsAsync(string userId, string buttonId);

    /// <summary>
    /// 根據系統ID批量設定權限
    /// </summary>
    Task<int> SetPermissionsBySystemAsync(string userId, string systemId, bool isAuthorized, string? createdBy);

    /// <summary>
    /// 根據子系統ID批量設定權限
    /// </summary>
    Task<int> SetPermissionsBySubSystemAsync(string userId, string subSystemId, bool isAuthorized, string? createdBy);

    /// <summary>
    /// 根據作業ID批量設定權限
    /// </summary>
    Task<int> SetPermissionsByProgramAsync(string userId, string programId, bool isAuthorized, string? createdBy);

    /// <summary>
    /// 根據使用者ID刪除所有直接權限（SYS0230）
    /// </summary>
    Task<int> DeleteByUserIdAsync(string userId);
}

/// <summary>
/// 使用者權限查詢條件
/// </summary>
public class UserPermissionQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? SystemId { get; set; }
    public string? SubSystemId { get; set; }
    public string? ProgramId { get; set; }
    public string? ButtonId { get; set; }
}

/// <summary>
/// 使用者權限 DTO（含關聯資訊）
/// </summary>
public class UserPermissionDto
{
    public long TKey { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string ButtonId { get; set; } = string.Empty;
    public string? SystemId { get; set; }
    public string? SystemName { get; set; }
    public string? SubSystemId { get; set; }
    public string? SubSystemName { get; set; }
    public string? ProgramId { get; set; }
    public string? ProgramName { get; set; }
    public string? ButtonName { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 使用者權限統計 DTO
/// </summary>
public class UserPermissionStatsDto
{
    public string SystemId { get; set; } = string.Empty;
    public string SystemName { get; set; } = string.Empty;
    public int TotalButtons { get; set; }
    public int AuthorizedButtons { get; set; }
    public bool IsFullyAuthorized { get; set; }
    public double AuthorizedRate { get; set; }
}

