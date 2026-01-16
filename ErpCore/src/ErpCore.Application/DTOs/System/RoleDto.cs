namespace ErpCore.Application.DTOs.System;

/// <summary>
/// 角色資料傳輸物件 (SYS0210)
/// </summary>
public class RoleDto
{
    public string RoleId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string? RoleNote { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 角色查詢 DTO
/// </summary>
public class RoleQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string SortOrder { get; set; } = "ASC";
    public string? RoleId { get; set; }
    public string? RoleName { get; set; }
}

/// <summary>
/// 新增角色 DTO
/// </summary>
public class CreateRoleDto
{
    public string RoleId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string? RoleNote { get; set; }
}

/// <summary>
/// 修改角色 DTO
/// </summary>
public class UpdateRoleDto
{
    public string RoleName { get; set; } = string.Empty;
    public string? RoleNote { get; set; }
}

/// <summary>
/// 複製角色 DTO (SYS0210 - 創建新角色)
/// </summary>
public class CopyRoleDto
{
    public string NewRoleId { get; set; } = string.Empty;
    public string NewRoleName { get; set; } = string.Empty;
}

/// <summary>
/// 複製角色到目標角色 DTO (SYS0240)
/// </summary>
public class CopyRoleToTargetDto
{
    public string SourceRoleId { get; set; } = string.Empty;
    public string TargetRoleId { get; set; } = string.Empty;
    public bool CopyUsers { get; set; } = false;
}

/// <summary>
/// 驗證角色複製 DTO (SYS0240)
/// </summary>
public class ValidateCopyRoleDto
{
    public string SourceRoleId { get; set; } = string.Empty;
    public string TargetRoleId { get; set; } = string.Empty;
}

/// <summary>
/// 角色複製結果 DTO (SYS0240)
/// </summary>
public class CopyRoleResultDto
{
    public string SourceRoleId { get; set; } = string.Empty;
    public string TargetRoleId { get; set; } = string.Empty;
    public int PermissionsCopied { get; set; }
    public int UsersCopied { get; set; }
}

/// <summary>
/// 驗證角色複製結果 DTO (SYS0240)
/// </summary>
public class ValidateCopyRoleResultDto
{
    public bool SourceRoleExists { get; set; }
    public bool TargetRoleExists { get; set; }
    public bool SourceHasPermissions { get; set; }
    public bool IsSameRole { get; set; }
}
