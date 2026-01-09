using ErpCore.Domain.Entities.Energy;

namespace ErpCore.Infrastructure.Repositories.Energy;

/// <summary>
/// 能源基礎 Repository 介面 (SYSO100-SYSO130 - 能源基礎功能)
/// </summary>
public interface IEnergyBaseRepository
{
    Task<EnergyBase?> GetByIdAsync(long tKey);
    Task<EnergyBase?> GetByEnergyIdAsync(string energyId);
    Task<IEnumerable<EnergyBase>> QueryAsync(EnergyBaseQuery query);
    Task<int> GetCountAsync(EnergyBaseQuery query);
    Task<long> CreateAsync(EnergyBase entity);
    Task UpdateAsync(EnergyBase entity);
    Task DeleteAsync(long tKey);
}

/// <summary>
/// 能源基礎查詢條件
/// </summary>
public class EnergyBaseQuery
{
    public string? EnergyId { get; set; }
    public string? EnergyName { get; set; }
    public string? EnergyType { get; set; }
    public string? Status { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

