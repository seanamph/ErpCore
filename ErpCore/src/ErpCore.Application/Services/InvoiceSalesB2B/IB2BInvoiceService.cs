using ErpCore.Application.DTOs.InvoiceSalesB2B;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.InvoiceSalesB2B;

/// <summary>
/// B2B發票服務接口 (SYSG000_B2B - B2B發票資料維護)
/// </summary>
public interface IB2BInvoiceService
{
    Task<PagedResult<B2BInvoiceDto>> GetB2BInvoicesAsync(B2BInvoiceQueryDto query);
    Task<B2BInvoiceDto> GetB2BInvoiceByIdAsync(long tKey);
    Task<long> CreateB2BInvoiceAsync(CreateB2BInvoiceDto dto);
    Task UpdateB2BInvoiceAsync(UpdateB2BInvoiceDto dto);
    Task DeleteB2BInvoiceAsync(long tKey);
}

