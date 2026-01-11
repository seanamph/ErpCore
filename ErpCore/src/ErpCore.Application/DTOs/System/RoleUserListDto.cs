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
    public List<RoleUserDto> Users { get; set; } = new();
}

/// <summary>
/// 角色使用者 DTO (SYS0750)
/// </summary>
public class RoleUserDto
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

/// <summary>
/// 批次刪除角色使用者請求 DTO (SYS0750)
/// </summary>
public class BatchDeleteRoleUsersRequestDto
{
    /// <summary>
    /// 角色代碼
    /// </summary>
    [Required(ErrorMessage = "角色代碼為必填")]
    public string RoleId { get; set; } = string.Empty;

    /// <summary>
    /// 使用者代碼列表
    /// </summary>
    [Required(ErrorMessage = "使用者代碼列表為必填")]
    public List<string> UserIds { get; set; } = new();
}
