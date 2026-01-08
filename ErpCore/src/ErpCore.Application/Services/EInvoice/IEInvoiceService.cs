using ErpCore.Application.DTOs.EInvoice;
using ErpCore.Shared.Common;
using Microsoft.AspNetCore.Http;

namespace ErpCore.Application.Services.EInvoice;

/// <summary>
/// 電子發票服務介面
/// </summary>
public interface IEInvoiceService
{
    Task<EInvoiceUploadDto> UploadFileAsync(IFormFile file, string? storeId, string? retailerId, string? uploadType);
    Task<PagedResult<EInvoiceUploadDto>> GetUploadsAsync(EInvoiceUploadQueryDto query);
    Task<EInvoiceUploadDto> GetUploadAsync(long uploadId);
    Task<EInvoiceProcessStatusDto> GetProcessStatusAsync(long uploadId);
    Task StartProcessAsync(long uploadId);
    Task<PagedResult<EInvoiceDto>> GetEInvoicesAsync(EInvoiceQueryDto query);
    Task<EInvoiceDto> GetEInvoiceAsync(long invoiceId);
    Task DeleteUploadAsync(long uploadId);
    Task<PagedResult<EInvoiceReportDto>> GetEInvoiceReportsAsync(EInvoiceReportQueryDto query);

    /// <summary>
    /// 匯出電子發票報表到 Excel
    /// </summary>
    Task<byte[]> ExportEInvoiceReportsToExcelAsync(EInvoiceReportQueryDto query);

    /// <summary>
    /// 匯出電子發票報表到 PDF
    /// </summary>
    Task<byte[]> ExportEInvoiceReportsToPdfAsync(EInvoiceReportQueryDto query);
}

