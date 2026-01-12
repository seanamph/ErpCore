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

    /// <summary>
    /// 查詢耗材出庫明細表 (SYSA1013)
    /// </summary>
    Task<PagedResult<SYSA1013ReportDto>> GetSYSA1013ReportAsync(SYSA1013QueryDto query);

    /// <summary>
    /// 匯出耗材出庫明細表 (SYSA1013)
    /// </summary>
    Task<byte[]> ExportSYSA1013ReportAsync(SYSA1013QueryDto query, string format);

    /// <summary>
    /// 列印耗材出庫明細表 (SYSA1013)
    /// </summary>
    Task<byte[]> PrintSYSA1013ReportAsync(SYSA1013QueryDto query);

    /// <summary>
    /// 查詢商品分析報表 (SYSA1014)
    /// </summary>
    Task<PagedResult<SYSA1014ReportDto>> GetSYSA1014ReportAsync(SYSA1014QueryDto query);

    /// <summary>
    /// 匯出商品分析報表 (SYSA1014)
    /// </summary>
    Task<byte[]> ExportSYSA1014ReportAsync(SYSA1014QueryDto query, string format);

    /// <summary>
    /// 列印商品分析報表 (SYSA1014)
    /// </summary>
    Task<byte[]> PrintSYSA1014ReportAsync(SYSA1014QueryDto query);

    /// <summary>
    /// 查詢商品分析報表 (SYSA1015)
    /// </summary>
    Task<PagedResult<SYSA1015ReportDto>> GetSYSA1015ReportAsync(SYSA1015QueryDto query);

    /// <summary>
    /// 匯出商品分析報表 (SYSA1015)
    /// </summary>
    Task<byte[]> ExportSYSA1015ReportAsync(SYSA1015QueryDto query, string format);

    /// <summary>
    /// 列印商品分析報表 (SYSA1015)
    /// </summary>
    Task<byte[]> PrintSYSA1015ReportAsync(SYSA1015QueryDto query);

    /// <summary>
    /// 查詢商品分析報表 (SYSA1016)
    /// </summary>
    Task<PagedResult<SYSA1016ReportDto>> GetSYSA1016ReportAsync(SYSA1016QueryDto query);

    /// <summary>
    /// 匯出商品分析報表 (SYSA1016)
    /// </summary>
    Task<byte[]> ExportSYSA1016ReportAsync(SYSA1016QueryDto query, string format);

    /// <summary>
    /// 列印商品分析報表 (SYSA1016)
    /// </summary>
    Task<byte[]> PrintSYSA1016ReportAsync(SYSA1016QueryDto query);

    /// <summary>
    /// 查詢商品分析報表 (SYSA1017)
    /// </summary>
    Task<PagedResult<SYSA1017ReportDto>> GetSYSA1017ReportAsync(SYSA1017QueryDto query);

    /// <summary>
    /// 匯出商品分析報表 (SYSA1017)
    /// </summary>
    Task<byte[]> ExportSYSA1017ReportAsync(SYSA1017QueryDto query, string format);

    /// <summary>
    /// 列印商品分析報表 (SYSA1017)
    /// </summary>
    Task<byte[]> PrintSYSA1017ReportAsync(SYSA1017QueryDto query);

    /// <summary>
    /// 查詢工務維修件數統計報表 (SYSA1018)
    /// </summary>
    Task<PagedResult<SYSA1018ReportDto>> GetSYSA1018ReportAsync(SYSA1018QueryDto query);

    /// <summary>
    /// 匯出工務維修件數統計報表 (SYSA1018)
    /// </summary>
    Task<byte[]> ExportSYSA1018ReportAsync(SYSA1018QueryDto query, string format);

    /// <summary>
    /// 列印工務維修件數統計報表 (SYSA1018)
    /// </summary>
    Task<byte[]> PrintSYSA1018ReportAsync(SYSA1018QueryDto query);

    /// <summary>
    /// 查詢商品分析報表 (SYSA1019)
    /// </summary>
    Task<PagedResult<SYSA1019ReportDto>> GetSYSA1019ReportAsync(SYSA1019QueryDto query);

    /// <summary>
    /// 匯出商品分析報表 (SYSA1019)
    /// </summary>
    Task<byte[]> ExportSYSA1019ReportAsync(SYSA1019QueryDto query, string format);

    /// <summary>
    /// 列印商品分析報表 (SYSA1019)
    /// </summary>
    Task<byte[]> PrintSYSA1019ReportAsync(SYSA1019QueryDto query);

    /// <summary>
    /// 查詢商品分析報表 (SYSA1020)
    /// </summary>
    Task<PagedResult<SYSA1020ReportDto>> GetSYSA1020ReportAsync(SYSA1020QueryDto query);

    /// <summary>
    /// 匯出商品分析報表 (SYSA1020)
    /// </summary>
    Task<byte[]> ExportSYSA1020ReportAsync(SYSA1020QueryDto query, string format);

    /// <summary>
    /// 列印商品分析報表 (SYSA1020)
    /// </summary>
    Task<byte[]> PrintSYSA1020ReportAsync(SYSA1020QueryDto query);

    /// <summary>
    /// 查詢月成本報表 (SYSA1021)
    /// </summary>
    Task<PagedResult<SYSA1021ReportDto>> GetSYSA1021ReportAsync(SYSA1021QueryDto query);

    /// <summary>
    /// 匯出月成本報表 (SYSA1021)
    /// </summary>
    Task<byte[]> ExportSYSA1021ReportAsync(SYSA1021QueryDto query, string format);

    /// <summary>
    /// 列印月成本報表 (SYSA1021)
    /// </summary>
    Task<byte[]> PrintSYSA1021ReportAsync(SYSA1021QueryDto query);

    /// <summary>
    /// 查詢工務維修統計報表 (SYSA1022)
    /// </summary>
    Task<PagedResult<SYSA1022ReportDto>> GetSYSA1022ReportAsync(SYSA1022QueryDto query);

    /// <summary>
    /// 匯出工務維修統計報表 (SYSA1022)
    /// </summary>
    Task<byte[]> ExportSYSA1022ReportAsync(SYSA1022QueryDto query, string format);

    /// <summary>
    /// 列印工務維修統計報表 (SYSA1022)
    /// </summary>
    Task<byte[]> PrintSYSA1022ReportAsync(SYSA1022QueryDto query);

    /// <summary>
    /// 查詢工務維修統計報表(報表類型) (SYSA1023)
    /// </summary>
    Task<PagedResult<SYSA1023ReportDto>> GetSYSA1023ReportAsync(SYSA1023QueryDto query);

    /// <summary>
    /// 匯出工務維修統計報表(報表類型) (SYSA1023)
    /// </summary>
    Task<byte[]> ExportSYSA1023ReportAsync(SYSA1023QueryDto query, string format);

    /// <summary>
    /// 列印工務維修統計報表(報表類型) (SYSA1023)
    /// </summary>
    Task<byte[]> PrintSYSA1023ReportAsync(SYSA1023QueryDto query);
}
