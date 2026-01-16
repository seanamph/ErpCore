using ErpCore.Application.DTOs.Procurement;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Procurement;

/// <summary>
/// 採購報表列印服務介面
/// </summary>
public interface IPurchaseReportPrintService
{
    Task<PagedResult<PurchaseReportPrintDto>> GetPurchaseReportPrintsAsync(PurchaseReportPrintQueryDto query);
    Task<PurchaseReportPrintDto> GetPurchaseReportPrintByIdAsync(long tKey);
    Task<PurchaseReportPrintResultDto> CreatePurchaseReportPrintAsync(CreatePurchaseReportPrintDto dto);
    Task UpdatePurchaseReportPrintAsync(long tKey, UpdatePurchaseReportPrintDto dto);
    Task DeletePurchaseReportPrintAsync(long tKey);
    Task<byte[]> DownloadPurchaseReportPrintAsync(long tKey);
    Task<ReportPreviewDto> PreviewPurchaseReportPrintAsync(CreatePurchaseReportPrintDto dto);
    Task<List<PurchaseReportTemplateDto>> GetPurchaseReportTemplatesAsync(string? reportType, string? reportCode);
}
