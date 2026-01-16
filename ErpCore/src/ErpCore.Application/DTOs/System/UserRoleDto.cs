namespace ErpCore.Application.DTOs.System;

/// <summary>
/// 使用者角色資料傳輸物件 (SYS0220)
/// </summary>
public class UserRoleDto
{
    /// <summary>
    /// 使用者編號
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 角色編號
    /// </summary>
    public string RoleId { get; set; } = string.Empty;

    /// <summary>
    /// 角色名稱
    /// </summary>
    public string? RoleName { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 分配角色請求 DTO (SYS0220)
/// </summary>
public class AssignRolesDto
{
    /// <summary>
    /// 角色編號列表
    /// </summary>
    public List<string> RoleIds { get; set; } = new();
}

/// <summary>
/// 分配角色結果 DTO (SYS0220)
/// </summary>
public class AssignRoleResultDto
{
    /// <summary>
    /// 已分配的角色數量
    /// </summary>
    public int AssignedCount { get; set; }
}

/// <summary>
/// 移除角色請求 DTO (SYS0220)
/// </summary>
public class RemoveRolesDto
{
    /// <summary>
    /// 角色編號列表
    /// </summary>
    public List<string> RoleIds { get; set; } = new();
}

/// <summary>
/// 移除角色結果 DTO (SYS0220)
/// </summary>
public class RemoveRoleResultDto
{
    /// <summary>
    /// 已移除的角色數量
    /// </summary>
    public int RemovedCount { get; set; }
}

/// <summary>
/// 批量更新角色請求 DTO (SYS0220)
/// </summary>
public class BatchUpdateRolesDto
{
    /// <summary>
    /// 角色編號列表
    /// </summary>
    public List<string> RoleIds { get; set; } = new();
}

/// <summary>
/// 批量更新角色結果 DTO (SYS0220)
/// </summary>
public class BatchUpdateRoleResultDto
{
    /// <summary>
    /// 新增的角色數量
    /// </summary>
    public int AddedCount { get; set; }

    /// <summary>
    /// 移除的角色數量
    /// </summary>
    public int RemovedCount { get; set; }
}
