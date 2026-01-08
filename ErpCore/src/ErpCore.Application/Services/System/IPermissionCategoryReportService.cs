using ErpCore.Application.DTOs.System;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 權限分類報表服務介面 (SYS0770)
/// </summary>
public interface IPermissionCategoryReportService
{
    /// <summary>
    /// 查詢權限分類報表
    /// </summary>
    Task<PermissionCategoryReportResponseDto> GetPermissionCategoryReportAsync(PermissionCategoryReportRequestDto request);

    /// <summary>
    /// 匯出權限分類報表
    /// </summary>
    Task<byte[]> ExportPermissionCategoryReportAsync(PermissionCategoryReportRequestDto request, string exportFormat);
}

