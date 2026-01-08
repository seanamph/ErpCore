using ErpCore.Application.DTOs.System;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 作業權限之角色列表服務介面 (SYS0740)
/// </summary>
public interface IProgramRolePermissionService
{
    /// <summary>
    /// 查詢作業權限之角色列表
    /// </summary>
    Task<ProgramRolePermissionListResponseDto> GetProgramRolePermissionListAsync(ProgramRolePermissionListRequestDto request);

    /// <summary>
    /// 匯出作業權限之角色報表
    /// </summary>
    Task<byte[]> ExportProgramRolePermissionReportAsync(ProgramRolePermissionListRequestDto request, string exportFormat);
}

