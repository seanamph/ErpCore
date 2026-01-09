using ErpCore.Application.DTOs.CustomerCustomJgjn;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.CustomerCustomJgjn;

/// <summary>
/// JGJN發票服務介面
/// </summary>
public interface IJgjNInvoiceService
{
    Task<PagedResult<JgjNInvoiceDto>> GetJgjNInvoiceListAsync(JgjNInvoiceQueryDto query);
    Task<JgjNInvoiceDto?> GetJgjNInvoiceByIdAsync(long tKey);
    Task<JgjNInvoiceDto?> GetJgjNInvoiceByInvoiceIdAsync(string invoiceId);
    Task<long> CreateJgjNInvoiceAsync(CreateJgjNInvoiceDto dto);
    Task UpdateJgjNInvoiceAsync(long tKey, UpdateJgjNInvoiceDto dto);
    Task DeleteJgjNInvoiceAsync(long tKey);
    Task PrintJgjNInvoiceAsync(long tKey, PrintJgjNInvoiceDto dto);
}

