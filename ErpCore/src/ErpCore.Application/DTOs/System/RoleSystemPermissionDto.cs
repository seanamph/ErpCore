using System.ComponentModel.DataAnnotations;

namespace ErpCore.Application.DTOs.System;

/// <summary>
/// 角色系統權限列表查詢請求 DTO (SYS0731)
/// </summary>
public class RoleSystemPermissionListRequestDto
{
    /// <summary>
    /// 角色代碼（必填）
    /// </summary>
    [Required(ErrorMessage = "角色代碼為必填")]
    public string RoleId { get; set; } = string.Empty;

    /// <summary>
    /// 系統代碼（篩選條件）
    /// </summary>
    public string? SystemId { get; set; }

    /// <summary>
    /// 選單代碼（篩選條件）
    /// </summary>
    public string? MenuId { get; set; }

    /// <summary>
    /// 作業代碼（篩選條件）
    /// </summary>
    public string? ProgramId { get; set; }
}

/// <summary>
/// 角色系統權限列表回應 DTO (SYS0731)
/// </summary>
public class RoleSystemPermissionListResponseDto
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
    /// 權限列表
    /// </summary>
    public List<RoleSystemPermissionDto> Permissions { get; set; } = new();
}

/// <summary>
/// 角色系統權限 DTO (SYS0731)
/// </summary>
public class RoleSystemPermissionDto
{
    /// <summary>
    /// 系統代碼
    /// </summary>
    public string SystemId { get; set; } = string.Empty;

    /// <summary>
    /// 系統名稱
    /// </summary>
    public string SystemName { get; set; } = string.Empty;

    /// <summary>
    /// 選單列表
    /// </summary>
    public List<RoleMenuPermissionDto> Menus { get; set; } = new();
}

/// <summary>
/// 角色選單權限 DTO (SYS0731)
/// </summary>
public class RoleMenuPermissionDto
{
    /// <summary>
    /// 選單代碼
    /// </summary>
    public string MenuId { get; set; } = string.Empty;

    /// <summary>
    /// 選單名稱
    /// </summary>
    public string MenuName { get; set; } = string.Empty;

    /// <summary>
    /// 作業列表
    /// </summary>
    public List<RoleProgramPermissionDto> Programs { get; set; } = new();
}

/// <summary>
/// 角色作業權限 DTO (SYS0731)
/// </summary>
public class RoleProgramPermissionDto
{
    /// <summary>
    /// 作業代碼
    /// </summary>
    public string ProgramId { get; set; } = string.Empty;

    /// <summary>
    /// 作業名稱
    /// </summary>
    public string ProgramName { get; set; } = string.Empty;

    /// <summary>
    /// 按鈕列表
    /// </summary>
    public List<RoleButtonPermissionDto> Buttons { get; set; } = new();
}

/// <summary>
/// 角色按鈕權限 DTO (SYS0731)
/// </summary>
public class RoleButtonPermissionDto
{
    /// <summary>
    /// 按鈕代碼
    /// </summary>
    public string ButtonId { get; set; } = string.Empty;

    /// <summary>
    /// 按鈕名稱
    /// </summary>
    public string ButtonName { get; set; } = string.Empty;

    /// <summary>
    /// 頁面代碼
    /// </summary>
    public string? PageId { get; set; }
}

/// <summary>
/// 角色系統權限報表匯出請求 DTO (SYS0731)
/// </summary>
public class RoleSystemPermissionExportRequestDto
{
    /// <summary>
    /// 查詢條件
    /// </summary>
    public RoleSystemPermissionListRequestDto Request { get; set; } = new();

    /// <summary>
    /// 匯出格式 (Excel|PDF)
    /// </summary>
    public string ExportFormat { get; set; } = "Excel";
}

