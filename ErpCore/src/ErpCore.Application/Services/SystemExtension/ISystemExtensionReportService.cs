using ErpCore.Application.DTOs.SystemExtension;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.SystemExtension;

/// <summary>
/// 系統擴展報表服務介面 (SYSX140)
/// </summary>
public interface ISystemExtensionReportService
{
    /// <summary>
    /// 查詢報表資料
    /// </summary>
    Task<SystemExtensionReportQueryResultDto> QueryReportDataAsync(SystemExtensionReportQueryRequestDto request);

    /// <summary>
    /// 產生 PDF 報表
    /// </summary>
    Task<SystemExtensionReportDto> GeneratePdfReportAsync(GenerateSystemExtensionReportDto request);

    /// <summary>
    /// 產生 Excel 報表
    /// </summary>
    Task<SystemExtensionReportDto> GenerateExcelReportAsync(GenerateSystemExtensionReportDto request);

    /// <summary>
    /// 查詢報表記錄列表
    /// </summary>
    Task<PagedResult<SystemExtensionReportDto>> GetReportsAsync(SystemExtensionReportQueryDto query);

    /// <summary>
    /// 下載報表檔案
    /// </summary>
    Task<byte[]> DownloadReportAsync(long reportId);

    /// <summary>
    /// 刪除報表記錄
    /// </summary>
    Task DeleteReportAsync(long reportId);
}

