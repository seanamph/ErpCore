using ErpCore.Application.DTOs.InvoiceSalesB2B;

namespace ErpCore.Application.Services.InvoiceSalesB2B;

/// <summary>
/// B2B電子發票列印設定服務接口 (SYSG000_B2B - B2B電子發票列印)
/// </summary>
public interface IB2BElectronicInvoicePrintSettingService
{
    /// <summary>
    /// 查詢B2B電子發票列印設定
    /// </summary>
    Task<B2BElectronicInvoicePrintSettingDto?> GetSettingAsync(string settingId);

    /// <summary>
    /// 查詢所有啟用的B2B電子發票列印設定
    /// </summary>
    Task<List<B2BElectronicInvoicePrintSettingDto>> GetAllActiveSettingsAsync();

    /// <summary>
    /// 更新B2B電子發票列印設定
    /// </summary>
    Task UpdateSettingAsync(UpdateB2BElectronicInvoicePrintSettingDto dto);
}

