using ErpCore.Domain.Entities.InvoiceSalesB2B;

namespace ErpCore.Infrastructure.Repositories.InvoiceSalesB2B;

/// <summary>
/// B2B電子發票列印設定 Repository 接口 (SYSG000_B2B - B2B電子發票列印)
/// </summary>
public interface IB2BElectronicInvoicePrintSettingRepository
{
    /// <summary>
    /// 根據設定ID查詢
    /// </summary>
    Task<B2BElectronicInvoicePrintSetting?> GetByIdAsync(string settingId);

    /// <summary>
    /// 查詢所有啟用的設定
    /// </summary>
    Task<List<B2BElectronicInvoicePrintSetting>> GetAllActiveAsync();

    /// <summary>
    /// 新增設定
    /// </summary>
    Task<int> CreateAsync(B2BElectronicInvoicePrintSetting setting);

    /// <summary>
    /// 修改設定
    /// </summary>
    Task<int> UpdateAsync(B2BElectronicInvoicePrintSetting setting);

    /// <summary>
    /// 刪除設定
    /// </summary>
    Task<int> DeleteAsync(string settingId);
}

