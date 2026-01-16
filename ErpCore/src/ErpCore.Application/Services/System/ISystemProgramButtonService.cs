using ErpCore.Application.DTOs.System;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 系統作業與功能列表服務介面 (SYS0810, SYS0780, SYS0999)
/// </summary>
public interface ISystemProgramButtonService
{
    /// <summary>
    /// 查詢系統作業與功能列表
    /// </summary>
    Task<SystemProgramButtonResponseDto> GetSystemProgramButtonsAsync(string systemId);

    /// <summary>
    /// 匯出系統作業與功能列表報表
    /// </summary>
    Task<byte[]> ExportSystemProgramButtonsAsync(string systemId, string exportFormat);

    /// <summary>
    /// 查詢系統作業與功能列表（出庫用）(SYS0999)
    /// </summary>
    Task<SystemProgramButtonResponseDto> GetSystemProgramButtonsForExportAsync(string systemId);

    /// <summary>
    /// 匯出系統作業與功能列表報表（出庫用）(SYS0999)
    /// </summary>
    Task<byte[]> ExportSystemProgramButtonsReportAsync(string systemId, string exportFormat);
}

