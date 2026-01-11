using System.ComponentModel.DataAnnotations;

namespace ErpCore.Application.DTOs.System;

/// <summary>
/// 角色之使用者列表查詢請求 DTO (SYS0750)
/// </summary>
public class RoleUserListRequestDto
{
    /// <summary>
    /// 角色代碼（必填）
    /// </summary>
    [Required(ErrorMessage = "角色代碼為必填")]
    public string RoleId { get; set; } = string.Empty;
}

/// <summary>
/// 角色之使用者列表回應 DTO (SYS0750)
/// </summary>
public class RoleUserListResponseDto
{
    /// <summary>
    /// 角色代碼
    /// </summary>
    public string RoleId { get; set; } = string.Empty;

    /// <summary>
    /// 角色名稱
    /// </summary>
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// 使用者列表
    /// </summary>
    public List<RoleUserItemDto> Users { get; set; } = new();
}

/// <summary>
/// 角色使用者項目 DTO (SYS0750)
/// </summary>
public class RoleUserItemDto
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
    /// 使用者型態
    /// </summary>
    public string? UserType { get; set; }

    /// <summary>
    /// 使用者型態名稱
    /// </summary>
    public string? UserTypeName { get; set; }

    /// <summary>
    /// 帳號狀態
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 帳號狀態名稱
    /// </summary>
    public string? StatusName { get; set; }

    /// <summary>
    /// 職稱
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// 組織代碼
    /// </summary>
    public string? OrgId { get; set; }

    /// <summary>
    /// 組織名稱
    /// </summary>
    public string? OrgName { get; set; }
}

/// <summary>
/// 批次刪除角色使用者請求 DTO (SYS0750)
/// </summary>
public class BatchDeleteRoleUsersRequestDto
{
    /// <summary>
    /// 角色代碼
    /// </summary>
    public string RoleId { get; set; } = string.Empty;

    /// <summary>
    /// 使用者代碼列表
    /// </summary>
    public List<string> UserIds { get; set; } = new();
}

/// <summary>
/// 角色之使用者報表匯出請求 DTO (SYS0750)
/// </summary>
public class RoleUserExportRequestDto
{
    /// <summary>
    /// 查詢條件
    /// </summary>
    public RoleUserListRequestDto Request { get; set; } = new();

    /// <summary>
    /// 匯出格式 (Excel|PDF)
    /// </summary>
    public string ExportFormat { get; set; } = "Excel";
}

// ========== SYS0230 - 角色之使用者設定維護 DTO ==========

/// <summary>
/// 角色使用者查詢 DTO (SYS0230)
/// </summary>
public class RoleUserQueryDto
{
    /// <summary>
    /// 角色代碼（必填）
    /// </summary>
    public string RoleId { get; set; } = string.Empty;

    /// <summary>
    /// 部門代碼（選填）
    /// </summary>
    public string? OrgId { get; set; }

    /// <summary>
    /// 店別代碼（選填）
    /// </summary>
    public string? StoreId { get; set; }

    /// <summary>
    /// 使用者類型（選填，1:公司人員, 2:專櫃人員）
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
/// 批量設定角色使用者 DTO (SYS0230)
/// </summary>
public class BatchAssignRoleUsersDto
{
    /// <summary>
    /// 要新增的使用者ID列表
    /// </summary>
    public List<string> AddUserIds { get; set; } = new();

    /// <summary>
    /// 要移除的使用者ID列表
    /// </summary>
    public List<string> RemoveUserIds { get; set; } = new();
}

/// <summary>
/// 批量設定結果 DTO (SYS0230)
/// </summary>
public class BatchAssignRoleUsersResultDto
{
    /// <summary>
    /// 新增數量
    /// </summary>
    public int AddedCount { get; set; }

    /// <summary>
    /// 移除數量
    /// </summary>
    public int RemovedCount { get; set; }
}

