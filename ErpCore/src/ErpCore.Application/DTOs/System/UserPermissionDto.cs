namespace ErpCore.Application.DTOs.System;

/// <summary>
/// 使用者權限查詢 DTO
/// </summary>
public class UserPermissionQueryDto
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
/// 使用者權限 DTO
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
/// 新增使用者權限 DTO
/// </summary>
public class CreateUserPermissionDto
{
    public List<string> ButtonIds { get; set; } = new();
    public bool ClearRolePermissions { get; set; } = true;
}

/// <summary>
/// 批量設定使用者系統權限 DTO
/// </summary>
public class BatchSetUserSystemPermissionDto
{
    public List<SystemPermissionItem> SystemPermissions { get; set; } = new();
    public bool ClearRolePermissions { get; set; } = true;
}

/// <summary>
/// 批量設定使用者選單權限 DTO
/// </summary>
public class BatchSetUserMenuPermissionDto
{
    public List<MenuPermissionItem> MenuPermissions { get; set; } = new();
    public bool ClearRolePermissions { get; set; } = true;
}

/// <summary>
/// 批量設定使用者作業權限 DTO
/// </summary>
public class BatchSetUserProgramPermissionDto
{
    public List<ProgramPermissionItem> ProgramPermissions { get; set; } = new();
    public bool ClearRolePermissions { get; set; } = true;
}

/// <summary>
/// 批量設定使用者按鈕權限 DTO
/// </summary>
public class BatchSetUserButtonPermissionDto
{
    public List<ButtonPermissionItem> ButtonPermissions { get; set; } = new();
    public bool ClearRolePermissions { get; set; } = true;
}

/// <summary>
/// 批量刪除使用者權限 DTO
/// </summary>
public class BatchDeleteUserPermissionDto
{
    public List<long> TKeys { get; set; } = new();
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

