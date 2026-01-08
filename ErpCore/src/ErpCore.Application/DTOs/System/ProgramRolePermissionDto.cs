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
    public List<ProgramRolePermissionDto> Roles { get; set; } = new();
}

/// <summary>
/// 作業角色權限 DTO (SYS0740)
/// </summary>
public class ProgramRolePermissionDto
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
    /// 按鈕列表
    /// </summary>
    public List<ProgramButtonPermissionDto> Buttons { get; set; } = new();
}

/// <summary>
/// 作業按鈕權限 DTO (SYS0740)
/// </summary>
public class ProgramButtonPermissionDto
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

