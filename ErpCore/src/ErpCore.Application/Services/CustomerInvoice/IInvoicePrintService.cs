using ErpCore.Application.DTOs.CustomerInvoice;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.CustomerInvoice;

/// <summary>
/// 發票列印服務介面 (SYS2000 - 發票列印作業)
/// </summary>
public interface IInvoicePrintService
{
    /// <summary>
    /// 查詢發票列表
    /// </summary>
    Task<PagedResult<InvoiceDto>> GetInvoicesAsync(InvoiceQueryDto query);

    /// <summary>
    /// 查詢單筆發票
    /// </summary>
    Task<InvoiceDto> GetInvoiceByIdAsync(string invoiceNo);

    /// <summary>
    /// 列印發票
    /// </summary>
    Task PrintInvoiceAsync(string invoiceNo, InvoicePrintRequestDto request);

    /// <summary>
    /// 批次列印發票
    /// </summary>
    Task BatchPrintInvoicesAsync(BatchPrintInvoiceDto dto);

    /// <summary>
    /// 查詢列印記錄
    /// </summary>
    Task<IEnumerable<InvoicePrintLogDto>> GetPrintLogsAsync(string invoiceNo);
}

