using ErpCore.Domain.Entities.InvoiceSales;

namespace ErpCore.Infrastructure.Repositories.InvoiceSales;

/// <summary>
/// 電子發票列印設定 Repository 接口
/// </summary>
public interface IElectronicInvoicePrintSettingRepository
{
    /// <summary>
    /// 根據設定ID查詢
    /// </summary>
    Task<ElectronicInvoicePrintSetting?> GetByIdAsync(string settingId);

    /// <summary>
    /// 查詢所有啟用的設定
    /// </summary>
    Task<List<ElectronicInvoicePrintSetting>> GetAllActiveAsync();

    /// <summary>
    /// 新增設定
    /// </summary>
    Task<int> CreateAsync(ElectronicInvoicePrintSetting setting);

    /// <summary>
    /// 修改設定
    /// </summary>
    Task<int> UpdateAsync(ElectronicInvoicePrintSetting setting);

    /// <summary>
    /// 刪除設定
    /// </summary>
    Task<int> DeleteAsync(string settingId);
}

