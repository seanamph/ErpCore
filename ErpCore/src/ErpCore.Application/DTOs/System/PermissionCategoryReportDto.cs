namespace ErpCore.Application.DTOs.System;

/// <summary>
/// 權限分類報表查詢請求 DTO (SYS0770)
/// </summary>
public class PermissionCategoryReportRequestDto
{
    /// <summary>
    /// 授權型態 (1:角色, 2:使用者)
    /// </summary>
    public string PermissionType { get; set; } = "2";
}

/// <summary>
/// 權限分類報表回應 DTO (SYS0770)
/// </summary>
public class PermissionCategoryReportResponseDto
{
    /// <summary>
    /// 授權型態 (1:角色, 2:使用者)
    /// </summary>
    public string PermissionType { get; set; } = string.Empty;

    /// <summary>
    /// 授權型態名稱
    /// </summary>
    public string PermissionTypeName { get; set; } = string.Empty;

    /// <summary>
    /// 權限分類列表
    /// </summary>
    public List<PermissionCategoryReportItemDto> Items { get; set; } = new();
}

/// <summary>
/// 權限分類報表項目 DTO (SYS0770)
/// </summary>
public class PermissionCategoryReportItemDto
{
    /// <summary>
    /// 序號
    /// </summary>
    public int SeqNo { get; set; }

    /// <summary>
    /// 使用者代碼
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 使用者名稱
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 角色代碼（僅當授權型態為角色時有值）
    /// </summary>
    public string? RoleId { get; set; }

    /// <summary>
    /// 角色名稱（僅當授權型態為角色時有值）
    /// </summary>
    public string? RoleName { get; set; }
}

/// <summary>
/// 權限分類報表匯出請求 DTO (SYS0770)
/// </summary>
public class PermissionCategoryReportExportRequestDto
{
    /// <summary>
    /// 查詢條件
    /// </summary>
    public PermissionCategoryReportRequestDto Request { get; set; } = new();

    /// <summary>
    /// 匯出格式 (Excel|PDF)
    /// </summary>
    public string ExportFormat { get; set; } = "Excel";
}

