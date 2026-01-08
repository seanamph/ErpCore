using ErpCore.Application.DTOs.System;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 角色系統權限列表服務介面 (SYS0731)
/// </summary>
public interface IRoleSystemPermissionService
{
    /// <summary>
    /// 查詢角色系統權限列表
    /// </summary>
    Task<RoleSystemPermissionListResponseDto> GetRoleSystemPermissionListAsync(RoleSystemPermissionListRequestDto request);

    /// <summary>
    /// 匯出角色系統權限報表
    /// </summary>
    Task<byte[]> ExportRoleSystemPermissionReportAsync(RoleSystemPermissionListRequestDto request, string exportFormat);
}

