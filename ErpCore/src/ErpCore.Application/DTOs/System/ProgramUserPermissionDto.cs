using System.ComponentModel.DataAnnotations;

namespace ErpCore.Application.DTOs.System;

/// <summary>
/// 作業權限之使用者列表查詢請求 DTO (SYS0720)
/// </summary>
public class ProgramUserPermissionListRequestDto
{
    /// <summary>
    /// 作業代碼（必填）
    /// </summary>
    [Required(ErrorMessage = "作業代碼為必填")]
    public string ProgramId { get; set; } = string.Empty;
}

/// <summary>
/// 作業權限之使用者列表回應 DTO (SYS0720)
/// </summary>
public class ProgramUserPermissionListResponseDto
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
    /// 使用者列表
    /// </summary>
    public List<ProgramUserPermissionItemDto> Users { get; set; } = new();
}

/// <summary>
/// 使用者權限 DTO (SYS0720)
/// </summary>
public class ProgramUserPermissionItemDto
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
    /// 按鈕權限列表
    /// </summary>
    public List<ButtonPermissionDto> Buttons { get; set; } = new();
}

/// <summary>
/// 作業權限之使用者報表匯出請求 DTO (SYS0720)
/// </summary>
public class ProgramUserPermissionExportRequestDto
{
    /// <summary>
    /// 查詢條件
    /// </summary>
    public ProgramUserPermissionListRequestDto Request { get; set; } = new();

    /// <summary>
    /// 匯出格式 (Excel|PDF)
    /// </summary>
    public string ExportFormat { get; set; } = "Excel";
}

