namespace ErpCore.Application.DTOs.System;

/// <summary>
/// 角色權限查詢 DTO
/// </summary>
public class RolePermissionQueryDto
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
/// 角色權限 DTO
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
/// 新增角色權限 DTO
/// </summary>
public class CreateRolePermissionDto
{
    public List<string> ButtonIds { get; set; } = new();
}

/// <summary>
/// 批量設定角色系統權限 DTO
/// </summary>
public class BatchSetRoleSystemPermissionDto
{
    public List<SystemPermissionItem> SystemPermissions { get; set; } = new();
}

/// <summary>
/// 系統權限項目
/// </summary>
public class SystemPermissionItem
{
    public string SystemId { get; set; } = string.Empty;
    public bool IsAuthorized { get; set; }
}

/// <summary>
/// 批量設定角色選單權限 DTO
/// </summary>
public class BatchSetRoleMenuPermissionDto
{
    public List<MenuPermissionItem> MenuPermissions { get; set; } = new();
}

/// <summary>
/// 選單權限項目
/// </summary>
public class MenuPermissionItem
{
    public string SubSystemId { get; set; } = string.Empty;
    public bool IsAuthorized { get; set; }
}

/// <summary>
/// 批量設定角色作業權限 DTO
/// </summary>
public class BatchSetRoleProgramPermissionDto
{
    public List<ProgramPermissionItem> ProgramPermissions { get; set; } = new();
}

/// <summary>
/// 作業權限項目
/// </summary>
public class ProgramPermissionItem
{
    public string ProgramId { get; set; } = string.Empty;
    public bool IsAuthorized { get; set; }
}

/// <summary>
/// 批量設定角色按鈕權限 DTO
/// </summary>
public class BatchSetRoleButtonPermissionDto
{
    public List<ButtonPermissionItem> ButtonPermissions { get; set; } = new();
}

/// <summary>
/// 按鈕權限項目
/// </summary>
public class ButtonPermissionItem
{
    public string ButtonId { get; set; } = string.Empty;
    public bool IsAuthorized { get; set; }
}

/// <summary>
/// 批量刪除角色權限 DTO
/// </summary>
public class BatchDeleteRolePermissionDto
{
    public List<long> TKeys { get; set; } = new();
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

