using ErpCore.Application.DTOs.System;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 系統權限列表服務介面 (SYS0710)
/// </summary>
public interface ISystemPermissionService
{
    /// <summary>
    /// 查詢系統權限列表
    /// </summary>
    Task<SystemPermissionListResponseDto> GetSystemPermissionListAsync(SystemPermissionListRequestDto request);

    /// <summary>
    /// 匯出系統權限報表
    /// </summary>
    Task<byte[]> ExportSystemPermissionReportAsync(SystemPermissionListRequestDto request, string exportFormat);
}

