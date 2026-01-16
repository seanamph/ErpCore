using ErpCore.Application.DTOs.Procurement;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Procurement;

/// <summary>
/// 採購擴展維護服務介面 (SYSPA10-SYSPB60)
/// </summary>
public interface IPurchaseExtendedMaintenanceService
{
    /// <summary>
    /// 查詢採購擴展維護列表
    /// </summary>
    Task<PagedResult<PurchaseExtendedMaintenanceDto>> GetPurchaseExtendedMaintenancesAsync(PurchaseExtendedMaintenanceQueryDto query);

    /// <summary>
    /// 查詢單筆採購擴展維護（根據主鍵）
    /// </summary>
    Task<PurchaseExtendedMaintenanceDto> GetPurchaseExtendedMaintenanceByTKeyAsync(long tKey);

    /// <summary>
    /// 查詢單筆採購擴展維護（根據維護代碼）
    /// </summary>
    Task<PurchaseExtendedMaintenanceDto> GetPurchaseExtendedMaintenanceByMaintenanceIdAsync(string maintenanceId);

    /// <summary>
    /// 新增採購擴展維護
    /// </summary>
    Task<long> CreatePurchaseExtendedMaintenanceAsync(CreatePurchaseExtendedMaintenanceDto dto);

    /// <summary>
    /// 修改採購擴展維護
    /// </summary>
    Task UpdatePurchaseExtendedMaintenanceAsync(long tKey, UpdatePurchaseExtendedMaintenanceDto dto);

    /// <summary>
    /// 刪除採購擴展維護
    /// </summary>
    Task DeletePurchaseExtendedMaintenanceAsync(long tKey);

    /// <summary>
    /// 檢查採購擴展維護是否存在
    /// </summary>
    Task<bool> ExistsAsync(string maintenanceId);
}
