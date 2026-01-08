using ErpCore.Application.DTOs.System;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 作業權限之使用者列表服務介面 (SYS0720)
/// </summary>
public interface IProgramUserPermissionService
{
    /// <summary>
    /// 查詢作業權限之使用者列表
    /// </summary>
    Task<ProgramUserPermissionListResponseDto> GetProgramUserPermissionListAsync(
        ProgramUserPermissionListRequestDto request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 匯出作業權限之使用者報表
    /// </summary>
    Task<byte[]> ExportProgramUserPermissionReportAsync(
        ProgramUserPermissionListRequestDto request,
        string exportFormat,
        CancellationToken cancellationToken = default);
}

