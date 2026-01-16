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
    
    /// <summary>
    /// 匯出電子發票查詢結果到 Excel (ECA3020)
    /// </summary>
    Task<byte[]> ExportEInvoicesToExcelAsync(EInvoiceQueryDto query);
    
    /// <summary>
    /// 匯出電子發票查詢結果到 PDF (ECA3020)
    /// </summary>
    Task<byte[]> ExportEInvoicesToPdfAsync(EInvoiceQueryDto query);
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
    
    /// <summary>
    /// 下載上傳檔案 (ECA2050)
    /// </summary>
    Task<(byte[] fileBytes, string fileName, string contentType)> DownloadUploadFileAsync(long uploadId);
    
    /// <summary>
    /// 下載處理結果 (成功/失敗清單) (ECA3010)
    /// </summary>
    Task<byte[]> DownloadResultAsync(long uploadId, string type);
}

