using ErpCore.Application.DTOs.BusinessReport;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.BusinessReport;

/// <summary>
/// 銷退卡報表服務介面 (SYSL310)
/// </summary>
public interface IReturnCardReportService
{
    /// <summary>
    /// 查詢銷退卡報表資料
    /// </summary>
    Task<ReturnCardReportResultDto> GetReportAsync(ReturnCardReportQueryDto query);

    /// <summary>
    /// 列印銷退卡報表
    /// </summary>
    Task<BusinessReportPrintResultDto> PrintReportAsync(BusinessReportPrintRequestDto request);

    /// <summary>
    /// 匯出銷退卡報表
    /// </summary>
    Task<BusinessReportPrintResultDto> ExportReportAsync(BusinessReportExportRequestDto request);
}

