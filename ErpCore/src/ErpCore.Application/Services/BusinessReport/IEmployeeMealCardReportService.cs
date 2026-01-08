using ErpCore.Application.DTOs.BusinessReport;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.BusinessReport;

/// <summary>
/// 員餐卡報表服務介面 (SYSL210)
/// </summary>
public interface IEmployeeMealCardReportService
{
    /// <summary>
    /// 查詢員餐卡報表資料
    /// </summary>
    Task<PagedResult<EmployeeMealCardReportDto>> GetReportsAsync(EmployeeMealCardReportQueryDto query);

    /// <summary>
    /// 列印員餐卡報表
    /// </summary>
    Task<BusinessReportPrintResultDto> PrintReportAsync(string reportType, BusinessReportPrintRequestDto request);

    /// <summary>
    /// 匯出員餐卡報表
    /// </summary>
    Task<BusinessReportPrintResultDto> ExportReportAsync(string reportType, BusinessReportExportRequestDto request);

    /// <summary>
    /// 取得報表類型選項
    /// </summary>
    Task<List<ReportTypeOptionDto>> GetReportTypesAsync();
}

