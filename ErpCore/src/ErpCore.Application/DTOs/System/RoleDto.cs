namespace ErpCore.Application.DTOs.System;

/// <summary>
/// 角色資料傳輸物件
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
/// 複製角色 DTO
/// </summary>
public class CopyRoleDto
{
    public string NewRoleId { get; set; } = string.Empty;
    public string NewRoleName { get; set; } = string.Empty;
}

/// <summary>
/// 批次刪除角色 DTO
/// </summary>
public class BatchDeleteRoleDto
{
    public List<string> RoleIds { get; set; } = new();
}

/// <summary>
/// 使用者角色 DTO (SYS0220)
/// </summary>
public class UserRoleDto
{
    public string UserId { get; set; } = string.Empty;
    public string RoleId { get; set; } = string.Empty;
    public string? RoleName { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 分配角色 DTO (SYS0220)
/// </summary>
public class AssignRolesDto
{
    public List<string> RoleIds { get; set; } = new();
}

/// <summary>
/// 移除角色 DTO (SYS0220)
/// </summary>
public class RemoveRolesDto
{
    public List<string> RoleIds { get; set; } = new();
}

/// <summary>
/// 批量更新角色 DTO (SYS0220)
/// </summary>
public class BatchUpdateRolesDto
{
    public List<string> RoleIds { get; set; } = new();
}

/// <summary>
/// 分配角色結果 DTO (SYS0220)
/// </summary>
public class AssignRoleResultDto
{
    public int AssignedCount { get; set; }
}

/// <summary>
/// 移除角色結果 DTO (SYS0220)
/// </summary>
public class RemoveRoleResultDto
{
    public int RemovedCount { get; set; }
}

/// <summary>
/// 批量更新角色結果 DTO (SYS0220)
/// </summary>
public class BatchUpdateRoleResultDto
{
    public int AddedCount { get; set; }
    public int RemovedCount { get; set; }
}

