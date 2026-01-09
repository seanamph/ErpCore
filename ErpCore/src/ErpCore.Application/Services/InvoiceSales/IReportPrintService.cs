using ErpCore.Application.DTOs.InvoiceSales;

namespace ErpCore.Application.Services.InvoiceSales;

/// <summary>
/// 報表列印服務接口 (SYSG710-SYSG7I0 - 報表列印作業)
/// </summary>
public interface IReportPrintService
{
    /// <summary>
    /// 列印報表
    /// </summary>
    Task<ReportPrintResultDto> PrintReportAsync(ReportPrintDto dto);

    /// <summary>
    /// 預覽報表
    /// </summary>
    Task<string> PreviewReportAsync(ReportPrintDto dto);

    /// <summary>
    /// 查詢報表模板列表
    /// </summary>
    Task<List<ReportTemplateDto>> GetTemplatesAsync(string reportType, string? status = null);
}

