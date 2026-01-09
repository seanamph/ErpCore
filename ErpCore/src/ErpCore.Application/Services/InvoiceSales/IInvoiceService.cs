using ErpCore.Application.DTOs.InvoiceSales;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.InvoiceSales;

/// <summary>
/// 發票服務接口 (SYSG110-SYSG190 - 發票資料維護)
/// </summary>
public interface IInvoiceService
{
    /// <summary>
    /// 查詢發票列表
    /// </summary>
    Task<PagedResult<InvoiceDto>> GetInvoicesAsync(InvoiceQueryDto query);

    /// <summary>
    /// 查詢單筆發票
    /// </summary>
    Task<InvoiceDto> GetInvoiceByIdAsync(long tKey);

    /// <summary>
    /// 新增發票
    /// </summary>
    Task<long> CreateInvoiceAsync(CreateInvoiceDto dto);

    /// <summary>
    /// 修改發票
    /// </summary>
    Task UpdateInvoiceAsync(UpdateInvoiceDto dto);

    /// <summary>
    /// 刪除發票
    /// </summary>
    Task DeleteInvoiceAsync(long tKey);
}

