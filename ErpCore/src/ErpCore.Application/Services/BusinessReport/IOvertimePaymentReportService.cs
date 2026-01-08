using ErpCore.Application.DTOs.BusinessReport;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.BusinessReport;

/// <summary>
/// 加班發放報表服務介面 (SYSL510)
/// </summary>
public interface IOvertimePaymentReportService
{
    /// <summary>
    /// 查詢加班發放報表資料
    /// </summary>
    Task<OvertimePaymentReportResultDto> GetReportAsync(OvertimePaymentReportQueryDto query);

    /// <summary>
    /// 列印加班發放報表
    /// </summary>
    Task<BusinessReportPrintResultDto> PrintReportAsync(BusinessReportPrintRequestDto request);

    /// <summary>
    /// 匯出加班發放報表
    /// </summary>
    Task<BusinessReportPrintResultDto> ExportReportAsync(BusinessReportExportRequestDto request);
}

