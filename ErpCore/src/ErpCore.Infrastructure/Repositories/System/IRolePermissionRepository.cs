using ErpCore.Domain.Entities.System;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 角色權限 Repository 介面 (SYS0310)
/// </summary>
public interface IRolePermissionRepository
{
    /// <summary>
    /// 查詢角色權限列表（含關聯資訊）
    /// </summary>
    Task<PagedResult<RolePermissionDto>> QueryPermissionsAsync(string roleId, RolePermissionQuery query);

    /// <summary>
    /// 查詢角色權限統計（依系統）
    /// </summary>
    Task<List<RolePermissionStatsDto>> GetSystemStatsAsync(string roleId);

    /// <summary>
    /// 查詢角色權限（單筆）
    /// </summary>
    Task<RoleButton?> GetByIdAsync(long tKey);

    /// <summary>
    /// 新增角色權限
    /// </summary>
    Task<RoleButton> CreateAsync(RoleButton roleButton);

    /// <summary>
    /// 批量新增角色權限
    /// </summary>
    Task<int> BatchCreateAsync(string roleId, List<string> buttonIds, string? createdBy);

    /// <summary>
    /// 修改角色權限
    /// </summary>
    Task<RoleButton> UpdateAsync(RoleButton roleButton);

    /// <summary>
    /// 刪除角色權限
    /// </summary>
    Task DeleteAsync(long tKey);

    /// <summary>
    /// 批量刪除角色權限
    /// </summary>
    Task<int> BatchDeleteAsync(List<long> tKeys);

    /// <summary>
    /// 檢查權限是否存在
    /// </summary>
    Task<bool> ExistsAsync(string roleId, string buttonId);

    /// <summary>
    /// 根據系統ID批量設定權限
    /// </summary>
    Task<int> SetPermissionsBySystemAsync(string roleId, string systemId, bool isAuthorized, string? createdBy);

    /// <summary>
    /// 根據子系統ID批量設定權限
    /// </summary>
    Task<int> SetPermissionsBySubSystemAsync(string roleId, string subSystemId, bool isAuthorized, string? createdBy);

    /// <summary>
    /// 根據作業ID批量設定權限
    /// </summary>
    Task<int> SetPermissionsByProgramAsync(string roleId, string programId, bool isAuthorized, string? createdBy);

    /// <summary>
    /// 根據角色ID刪除所有權限 (SYS0240)
    /// </summary>
    Task DeleteByRoleIdAsync(string roleId, System.Data.IDbTransaction? transaction = null);

    /// <summary>
    /// 從來源角色複製權限到目的角色 (SYS0240)
    /// </summary>
    Task<int> CopyFromRoleAsync(string sourceRoleId, string targetRoleId, string createdBy, System.Data.IDbTransaction? transaction = null);

    /// <summary>
    /// 根據角色ID查詢權限列表
    /// </summary>
    Task<IEnumerable<RoleButton>> GetByRoleIdAsync(string roleId);

    /// <summary>
    /// 查詢角色系統列表 (SYS0310)
    /// </summary>
    Task<List<RoleSystemListDto>> GetRoleSystemsAsync(string roleId);

    /// <summary>
    /// 查詢角色選單列表 (SYS0310)
    /// </summary>
    Task<List<RoleMenuListDto>> GetRoleMenusAsync(string roleId, string? systemId = null);

    /// <summary>
    /// 查詢角色作業列表 (SYS0310)
    /// </summary>
    Task<List<RoleProgramListDto>> GetRoleProgramsAsync(string roleId, string? menuId = null);

    /// <summary>
    /// 查詢角色按鈕列表 (SYS0310)
    /// </summary>
    Task<List<RoleButtonListDto>> GetRoleButtonsAsync(string roleId, string? programId = null);
}

/// <summary>
/// 角色權限查詢條件
/// </summary>
public class RolePermissionQuery
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
/// 角色權限 DTO（含關聯資訊）
/// </summary>
public class RolePermissionDto
{
    public long TKey { get; set; }
    public string RoleId { get; set; } = string.Empty;
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
/// 角色權限統計 DTO
/// </summary>
public class RolePermissionStatsDto
{
    public string SystemId { get; set; } = string.Empty;
    public string SystemName { get; set; } = string.Empty;
    public int TotalButtons { get; set; }
    public int AuthorizedButtons { get; set; }
    public bool IsFullyAuthorized { get; set; }
    public double AuthorizedRate { get; set; }
}

/// <summary>
/// 角色系統列表 DTO (SYS0310)
/// </summary>
public class RoleSystemListDto
{
    public string SystemId { get; set; } = string.Empty;
    public string SystemName { get; set; } = string.Empty;
    public int TotalButtons { get; set; }
    public int AuthorizedButtons { get; set; }
    public bool IsFullyAuthorized { get; set; }
    public double AuthorizedRate { get; set; }
}

/// <summary>
/// 角色選單列表 DTO (SYS0310)
/// </summary>
public class RoleMenuListDto
{
    public string MenuId { get; set; } = string.Empty;
    public string MenuName { get; set; } = string.Empty;
    public string SystemId { get; set; } = string.Empty;
    public int TotalButtons { get; set; }
    public int AuthorizedButtons { get; set; }
    public bool IsFullyAuthorized { get; set; }
}

/// <summary>
/// 角色作業列表 DTO (SYS0310)
/// </summary>
public class RoleProgramListDto
{
    public string ProgramId { get; set; } = string.Empty;
    public string ProgramName { get; set; } = string.Empty;
    public string MenuId { get; set; } = string.Empty;
    public int TotalButtons { get; set; }
    public int AuthorizedButtons { get; set; }
    public bool IsFullyAuthorized { get; set; }
}

/// <summary>
/// 角色按鈕列表 DTO (SYS0310)
/// </summary>
public class RoleButtonListDto
{
    public string ButtonId { get; set; } = string.Empty;
    public string ButtonName { get; set; } = string.Empty;
    public string ProgramId { get; set; } = string.Empty;
    public string? Funs { get; set; }
    public string? PageId { get; set; }
    public bool IsAuthorized { get; set; }
}

