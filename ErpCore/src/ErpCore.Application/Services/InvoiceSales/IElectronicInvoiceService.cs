using ErpCore.Application.DTOs.InvoiceSales;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.InvoiceSales;

/// <summary>
/// 電子發票服務接口 (SYSG210-SYSG2B0 - 電子發票列印)
/// </summary>
public interface IElectronicInvoiceService
{
    /// <summary>
    /// 查詢電子發票列表
    /// </summary>
    Task<PagedResult<ElectronicInvoiceDto>> GetElectronicInvoicesAsync(ElectronicInvoiceQueryDto query);

    /// <summary>
    /// 查詢單筆電子發票
    /// </summary>
    Task<ElectronicInvoiceDto> GetElectronicInvoiceByIdAsync(long tKey);

    /// <summary>
    /// 新增電子發票
    /// </summary>
    Task<long> CreateElectronicInvoiceAsync(CreateElectronicInvoiceDto dto);

    /// <summary>
    /// 修改電子發票
    /// </summary>
    Task UpdateElectronicInvoiceAsync(UpdateElectronicInvoiceDto dto);

    /// <summary>
    /// 刪除電子發票
    /// </summary>
    Task DeleteElectronicInvoiceAsync(long tKey);

    /// <summary>
    /// 電子發票手動取號列印
    /// </summary>
    Task<PrintDataDto> ManualPrintAsync(ManualPrintDto dto);

    /// <summary>
    /// 查詢中獎清冊
    /// </summary>
    Task<PagedResult<ElectronicInvoiceAwardDto>> GetAwardListAsync(AwardListQueryDto query);

    /// <summary>
    /// 中獎清冊列印
    /// </summary>
    Task<PrintDataDto> AwardPrintAsync(AwardPrintDto dto);
}

