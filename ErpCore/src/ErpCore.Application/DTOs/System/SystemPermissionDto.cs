namespace ErpCore.Application.DTOs.System;

/// <summary>
/// 系統權限列表查詢請求 DTO (SYS0710)
/// </summary>
public class SystemPermissionListRequestDto
{
    /// <summary>
    /// 使用者代碼
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// 角色代碼
    /// </summary>
    public string? RoleId { get; set; }

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
/// 系統權限列表回應 DTO (SYS0710)
/// </summary>
public class SystemPermissionListResponseDto
{
    /// <summary>
    /// 使用者代碼
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// 使用者名稱
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 角色代碼
    /// </summary>
    public string? RoleId { get; set; }

    /// <summary>
    /// 角色名稱
    /// </summary>
    public string? RoleName { get; set; }

    /// <summary>
    /// 權限列表
    /// </summary>
    public List<SystemPermissionDto> Permissions { get; set; } = new();
}

/// <summary>
/// 系統權限 DTO (SYS0710)
/// </summary>
public class SystemPermissionDto
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
    public List<MenuPermissionDto> Menus { get; set; } = new();
}

/// <summary>
/// 選單權限 DTO (SYS0710)
/// </summary>
public class MenuPermissionDto
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
    public List<ProgramPermissionDto> Programs { get; set; } = new();
}

/// <summary>
/// 作業權限 DTO (SYS0710)
/// </summary>
public class ProgramPermissionDto
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
    public List<ButtonPermissionDto> Buttons { get; set; } = new();
}

/// <summary>
/// 按鈕權限 DTO (SYS0710)
/// </summary>
public class ButtonPermissionDto
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
/// 系統權限報表匯出請求 DTO (SYS0710)
/// </summary>
public class SystemPermissionExportRequestDto
{
    /// <summary>
    /// 查詢條件
    /// </summary>
    public SystemPermissionListRequestDto Request { get; set; } = new();

    /// <summary>
    /// 匯出格式 (Excel|PDF)
    /// </summary>
    public string ExportFormat { get; set; } = "Excel";
}

