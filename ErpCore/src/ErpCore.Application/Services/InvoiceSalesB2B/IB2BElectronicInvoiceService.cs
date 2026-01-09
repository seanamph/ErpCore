using ErpCore.Application.DTOs.InvoiceSalesB2B;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.InvoiceSalesB2B;

/// <summary>
/// B2B電子發票服務接口 (SYSG000_B2B - B2B電子發票列印)
/// </summary>
public interface IB2BElectronicInvoiceService
{
    /// <summary>
    /// 查詢B2B電子發票列表
    /// </summary>
    Task<PagedResult<B2BElectronicInvoiceDto>> GetB2BElectronicInvoicesAsync(B2BElectronicInvoiceQueryDto query);

    /// <summary>
    /// 查詢單筆B2B電子發票
    /// </summary>
    Task<B2BElectronicInvoiceDto> GetB2BElectronicInvoiceByIdAsync(long tKey);

    /// <summary>
    /// 新增B2B電子發票
    /// </summary>
    Task<long> CreateB2BElectronicInvoiceAsync(CreateB2BElectronicInvoiceDto dto);

    /// <summary>
    /// 修改B2B電子發票
    /// </summary>
    Task UpdateB2BElectronicInvoiceAsync(UpdateB2BElectronicInvoiceDto dto);

    /// <summary>
    /// 刪除B2B電子發票
    /// </summary>
    Task DeleteB2BElectronicInvoiceAsync(long tKey);

    /// <summary>
    /// B2B電子發票手動取號列印
    /// </summary>
    Task<B2BPrintDataDto> ManualPrintAsync(B2BManualPrintDto dto);

    /// <summary>
    /// 查詢B2B中獎清冊
    /// </summary>
    Task<PagedResult<B2BElectronicInvoiceAwardDto>> GetAwardListAsync(B2BAwardListQueryDto query);

    /// <summary>
    /// B2B中獎清冊列印
    /// </summary>
    Task<B2BPrintDataDto> AwardPrintAsync(B2BAwardPrintDto dto);
}

