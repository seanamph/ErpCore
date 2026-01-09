using ErpCore.Domain.Entities.Energy;

namespace ErpCore.Infrastructure.Repositories.Energy;

/// <summary>
/// 能源處理 Repository 介面 (SYSO310 - 能源處理功能)
/// </summary>
public interface IEnergyProcessRepository
{
    Task<EnergyProcess?> GetByIdAsync(long tKey);
    Task<EnergyProcess?> GetByProcessIdAsync(string processId);
    Task<IEnumerable<EnergyProcess>> QueryAsync(EnergyProcessQuery query);
    Task<int> GetCountAsync(EnergyProcessQuery query);
    Task<long> CreateAsync(EnergyProcess entity);
    Task UpdateAsync(EnergyProcess entity);
    Task DeleteAsync(long tKey);
}

/// <summary>
/// 能源處理查詢條件
/// </summary>
public class EnergyProcessQuery
{
    public string? ProcessId { get; set; }
    public string? EnergyId { get; set; }
    public DateTime? ProcessDateFrom { get; set; }
    public DateTime? ProcessDateTo { get; set; }
    public string? ProcessType { get; set; }
    public string? Status { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

