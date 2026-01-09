using ErpCore.Domain.Entities.ChartTools;

namespace ErpCore.Infrastructure.Repositories.ChartTools;

/// <summary>
/// 圖表配置 Repository 介面
/// </summary>
public interface IChartConfigRepository
{
    Task<ChartConfig?> GetByIdAsync(Guid chartConfigId);
    Task<IEnumerable<ChartConfig>> QueryAsync(ChartConfigQuery query);
    Task<int> GetCountAsync(ChartConfigQuery query);
    Task<Guid> CreateAsync(ChartConfig entity);
    Task UpdateAsync(ChartConfig entity);
    Task DeleteAsync(Guid chartConfigId);
}

/// <summary>
/// 查詢條件
/// </summary>
public class ChartConfigQuery
{
    public string? ChartName { get; set; }
    public string? ChartType { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

