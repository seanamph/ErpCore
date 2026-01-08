using ErpCore.Application.DTOs.BusinessReport;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.BusinessReport;

/// <summary>
/// 業務報表列印記錄服務介面 (SYSL161)
/// </summary>
public interface IBusinessReportPrintLogService
{
    /// <summary>
    /// 查詢業務報表列印記錄列表
    /// </summary>
    Task<PagedResult<BusinessReportPrintLogDto>> GetBusinessReportPrintLogsAsync(BusinessReportPrintLogQueryDto query);

    /// <summary>
    /// 根據主鍵查詢單筆資料
    /// </summary>
    Task<BusinessReportPrintLogDto?> GetBusinessReportPrintLogByIdAsync(long tKey);

    /// <summary>
    /// 根據 ReportId 查詢列印記錄列表
    /// </summary>
    Task<List<BusinessReportPrintLogDto>> GetBusinessReportPrintLogsByReportIdAsync(string reportId);

    /// <summary>
    /// 列印業務報表
    /// </summary>
    Task<BusinessReportPrintResultDto> PrintReportAsync(string reportId, BusinessReportPrintRequestDto request);

    /// <summary>
    /// 預覽業務報表
    /// </summary>
    Task<object> PreviewReportAsync(string reportId, BusinessReportPrintRequestDto request);

    /// <summary>
    /// 匯出業務報表
    /// </summary>
    Task<BusinessReportPrintResultDto> ExportReportAsync(string reportId, BusinessReportExportRequestDto request);

    /// <summary>
    /// 刪除業務報表列印記錄
    /// </summary>
    Task<bool> DeleteBusinessReportPrintLogAsync(long tKey);

    /// <summary>
    /// 取得檔案位元組陣列（用於下載）
    /// </summary>
    Task<byte[]> GetFileBytesAsync(long tKey);

    /// <summary>
    /// 取得檔案名稱
    /// </summary>
    Task<string> GetFileNameAsync(long tKey);
}

