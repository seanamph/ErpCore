using ErpCore.Domain.Entities.Energy;

namespace ErpCore.Infrastructure.Repositories.Energy;

/// <summary>
/// 能源擴展 Repository 介面 (SYSOU10-SYSOU33 - 能源擴展功能)
/// </summary>
public interface IEnergyExtensionRepository
{
    Task<EnergyExtension?> GetByIdAsync(long tKey);
    Task<EnergyExtension?> GetByExtensionIdAsync(string extensionId);
    Task<IEnumerable<EnergyExtension>> QueryAsync(EnergyExtensionQuery query);
    Task<int> GetCountAsync(EnergyExtensionQuery query);
    Task<long> CreateAsync(EnergyExtension entity);
    Task UpdateAsync(EnergyExtension entity);
    Task DeleteAsync(long tKey);
}

/// <summary>
/// 能源擴展查詢條件
/// </summary>
public class EnergyExtensionQuery
{
    public string? ExtensionId { get; set; }
    public string? EnergyId { get; set; }
    public string? ExtensionType { get; set; }
    public string? Status { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

