using ErpCore.Application.DTOs.AnalysisReport;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.AnalysisReport;

/// <summary>
/// 分析報表服務介面 (SYSA1000)
/// </summary>
public interface IAnalysisReportService
{
    /// <summary>
    /// 查詢商品分析報表 (SYSA1011)
    /// </summary>
    Task<PagedResult<SYSA1011ReportDto>> GetSYSA1011ReportAsync(SYSA1011QueryDto query);

    /// <summary>
    /// 匯出商品分析報表
    /// </summary>
    Task<byte[]> ExportSYSA1011ReportAsync(SYSA1011QueryDto query, string format);

    /// <summary>
    /// 列印商品分析報表
    /// </summary>
    Task<byte[]> PrintSYSA1011ReportAsync(SYSA1011QueryDto query);

    /// <summary>
    /// 查詢進銷存分析報表 (SYSA1000) - 通用查詢方法
    /// </summary>
    Task<PagedResult<Dictionary<string, object>>> GetAnalysisReportAsync(string reportId, AnalysisReportQueryDto query);

    /// <summary>
    /// 匯出進銷存分析報表
    /// </summary>
    Task<byte[]> ExportAnalysisReportAsync(string reportId, AnalysisReportQueryDto query, string format);

    /// <summary>
    /// 列印進銷存分析報表
    /// </summary>
    Task<byte[]> PrintAnalysisReportAsync(string reportId, AnalysisReportQueryDto query);

    /// <summary>
    /// 查詢商品分類列表 (SYSA1011)
    /// </summary>
    Task<IEnumerable<GoodsCategoryDto>> GetGoodsCategoriesAsync(string categoryType, string? parentId = null);

    /// <summary>
    /// 查詢進銷存月報表 (SYSA1012)
    /// </summary>
    Task<PagedResult<SYSA1012ReportDto>> GetSYSA1012ReportAsync(SYSA1012QueryDto query);

    /// <summary>
    /// 匯出進銷存月報表 (SYSA1012)
    /// </summary>
    Task<byte[]> ExportSYSA1012ReportAsync(SYSA1012QueryDto query, string format);

    /// <summary>
    /// 列印進銷存月報表 (SYSA1012)
    /// </summary>
    Task<byte[]> PrintSYSA1012ReportAsync(SYSA1012QueryDto query);
}
