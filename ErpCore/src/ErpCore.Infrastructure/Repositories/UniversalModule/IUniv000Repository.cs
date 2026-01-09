using ErpCore.Domain.Entities.UniversalModule;

namespace ErpCore.Infrastructure.Repositories.UniversalModule;

/// <summary>
/// 通用模組資料 Repository 介面 (UNIV000系列)
/// </summary>
public interface IUniv000Repository
{
    Task<Univ000?> GetByIdAsync(long tKey);
    Task<Univ000?> GetByDataIdAsync(string dataId);
    Task<IEnumerable<Univ000>> QueryAsync(Univ000Query query);
    Task<int> GetCountAsync(Univ000Query query);
    Task<long> CreateAsync(Univ000 entity);
    Task UpdateAsync(Univ000 entity);
    Task DeleteAsync(long tKey);
}

/// <summary>
/// 通用模組資料查詢條件
/// </summary>
public class Univ000Query
{
    public string? DataId { get; set; }
    public string? DataName { get; set; }
    public string? DataType { get; set; }
    public string? Status { get; set; }
    public string? Keyword { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

