using System.ComponentModel.DataAnnotations;

namespace ErpCore.Application.DTOs.System;

/// <summary>
/// 角色使用者查詢 DTO (SYS0230)
/// </summary>
public class RoleUserQueryDto
{
    /// <summary>
    /// 角色代碼（必填）
    /// </summary>
    [Required(ErrorMessage = "角色代碼為必填")]
    public string RoleId { get; set; } = string.Empty;

    /// <summary>
    /// 部門代碼
    /// </summary>
    public string? OrgId { get; set; }

    /// <summary>
    /// 店別代碼
    /// </summary>
    public string? StoreId { get; set; }

    /// <summary>
    /// 使用者類型 (1:公司人員, 2:專櫃人員)
    /// </summary>
    public string? UserType { get; set; }

    /// <summary>
    /// 篩選條件（使用者代碼或名稱）
    /// </summary>
    public string? Filter { get; set; }

    /// <summary>
    /// 頁碼
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// 每頁筆數
    /// </summary>
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// 角色使用者列表項目 DTO (SYS0230)
/// </summary>
public class RoleUserListItemDto
{
    /// <summary>
    /// 使用者代碼
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 使用者名稱
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 部門代碼
    /// </summary>
    public string? OrgId { get; set; }

    /// <summary>
    /// 部門名稱
    /// </summary>
    public string? OrgName { get; set; }

    /// <summary>
    /// 是否已分配
    /// </summary>
    public bool IsAssigned { get; set; }
}

/// <summary>
/// 批量設定角色使用者請求 DTO (SYS0230)
/// </summary>
public class BatchAssignRoleUsersDto
{
    /// <summary>
    /// 要新增的使用者編號列表
    /// </summary>
    public List<string> AddUserIds { get; set; } = new();

    /// <summary>
    /// 要移除的使用者編號列表
    /// </summary>
    public List<string> RemoveUserIds { get; set; } = new();
}

/// <summary>
/// 批量設定角色使用者結果 DTO (SYS0230)
/// </summary>
public class BatchAssignRoleUsersResultDto
{
    /// <summary>
    /// 新增的使用者數量
    /// </summary>
    public int AddedCount { get; set; }

    /// <summary>
    /// 移除的使用者數量
    /// </summary>
    public int RemovedCount { get; set; }
}

/// <summary>
/// 新增角色使用者請求 DTO (SYS0230)
/// </summary>
public class AssignUserToRoleDto
{
    /// <summary>
    /// 使用者編號
    /// </summary>
    [Required(ErrorMessage = "使用者編號為必填")]
    public string UserId { get; set; } = string.Empty;
}
