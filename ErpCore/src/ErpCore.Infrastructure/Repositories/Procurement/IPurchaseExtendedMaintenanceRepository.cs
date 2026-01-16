using ErpCore.Domain.Entities.Procurement;

namespace ErpCore.Infrastructure.Repositories.Procurement;

/// <summary>
/// 採購擴展維護 Repository 介面 (SYSPA10-SYSPB60)
/// </summary>
public interface IPurchaseExtendedMaintenanceRepository
{
    Task<PurchaseExtendedMaintenance?> GetByIdAsync(long tKey);
    Task<PurchaseExtendedMaintenance?> GetByMaintenanceIdAsync(string maintenanceId);
    Task<IEnumerable<PurchaseExtendedMaintenance>> QueryAsync(PurchaseExtendedMaintenanceQuery query);
    Task<int> GetCountAsync(PurchaseExtendedMaintenanceQuery query);
    Task<bool> ExistsAsync(string maintenanceId);
    Task<PurchaseExtendedMaintenance> CreateAsync(PurchaseExtendedMaintenance purchaseExtendedMaintenance);
    Task<PurchaseExtendedMaintenance> UpdateAsync(PurchaseExtendedMaintenance purchaseExtendedMaintenance);
    Task DeleteAsync(long tKey);
}

/// <summary>
/// 採購擴展維護查詢條件
/// </summary>
public class PurchaseExtendedMaintenanceQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? MaintenanceId { get; set; }
    public string? MaintenanceName { get; set; }
    public string? MaintenanceType { get; set; }
    public string? Status { get; set; }
}
