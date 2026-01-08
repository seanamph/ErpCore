using ErpCore.Domain.Entities.Query;

namespace ErpCore.Infrastructure.Repositories.Query;

/// <summary>
/// 保管人及額度設定 Repository 介面 (SYSQ120)
/// </summary>
public interface IPcKeepRepository
{
    Task<PcKeep?> GetByIdAsync(long tKey);
    Task<PcKeep?> GetByKeepEmpIdAsync(string keepEmpId, string? siteId);
    Task<IEnumerable<PcKeep>> QueryAsync(PcKeepQuery query);
    Task<int> GetCountAsync(PcKeepQuery query);
    Task<bool> ExistsAsync(string keepEmpId, string? siteId);
    Task<PcKeep> CreateAsync(PcKeep entity);
    Task<PcKeep> UpdateAsync(PcKeep entity);
    Task DeleteAsync(long tKey);
}

/// <summary>
/// 保管人及額度查詢條件
/// </summary>
public class PcKeepQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SiteId { get; set; }
    public string? KeepEmpId { get; set; }
}

