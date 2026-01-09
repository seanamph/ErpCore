using ErpCore.Application.DTOs.InvoiceSales;

namespace ErpCore.Application.Services.InvoiceSales;

/// <summary>
/// 電子發票列印設定服務接口
/// </summary>
public interface IElectronicInvoicePrintSettingService
{
    /// <summary>
    /// 查詢列印設定
    /// </summary>
    Task<ElectronicInvoicePrintSettingDto?> GetSettingAsync(string settingId);

    /// <summary>
    /// 查詢所有啟用的設定
    /// </summary>
    Task<List<ElectronicInvoicePrintSettingDto>> GetAllActiveSettingsAsync();

    /// <summary>
    /// 更新列印設定
    /// </summary>
    Task UpdateSettingAsync(UpdateElectronicInvoicePrintSettingDto dto);
}

