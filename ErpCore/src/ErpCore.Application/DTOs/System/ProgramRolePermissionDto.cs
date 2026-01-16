using System.ComponentModel.DataAnnotations;

namespace ErpCore.Application.DTOs.System;

/// <summary>
/// 作業權限之角色列表查詢請求 DTO (SYS0740)
/// </summary>
public class ProgramRolePermissionListRequestDto
{
    /// <summary>
    /// 作業代碼（必填）
    /// </summary>
    [Required(ErrorMessage = "作業代碼為必填")]
    public string ProgramId { get; set; } = string.Empty;
}

/// <summary>
/// 作業權限之角色列表回應 DTO (SYS0740)
/// </summary>
public class ProgramRolePermissionListResponseDto
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
    /// 角色列表
    /// </summary>
    public List<ProgramRolePermissionItemDto> Roles { get; set; } = new();
}

/// <summary>
/// 角色權限 DTO (SYS0740)
/// </summary>
public class ProgramRolePermissionItemDto
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
    /// 按鈕權限列表
    /// </summary>
    public List<ButtonPermissionDto> Buttons { get; set; } = new();
}

/// <summary>
/// 作業權限之角色報表匯出請求 DTO (SYS0740)
/// </summary>
public class ProgramRolePermissionExportRequestDto
{
    /// <summary>
    /// 查詢條件
    /// </summary>
    public ProgramRolePermissionListRequestDto Request { get; set; } = new();

    /// <summary>
    /// 匯出格式 (Excel|PDF)
    /// </summary>
    public string ExportFormat { get; set; } = "Excel";
}
