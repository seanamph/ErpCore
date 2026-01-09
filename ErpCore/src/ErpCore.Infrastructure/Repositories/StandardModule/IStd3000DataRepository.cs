using ErpCore.Domain.Entities.StandardModule;

namespace ErpCore.Infrastructure.Repositories.StandardModule;

/// <summary>
/// STD3000 資料 Repository 介面 (SYS3620 - 標準資料維護)
/// </summary>
public interface IStd3000DataRepository
{
    Task<Std3000Data?> GetByIdAsync(long tKey);
    Task<Std3000Data?> GetByDataIdAsync(string dataId);
    Task<IEnumerable<Std3000Data>> QueryAsync(Std3000DataQuery query);
    Task<int> GetCountAsync(Std3000DataQuery query);
    Task<long> CreateAsync(Std3000Data entity);
    Task UpdateAsync(Std3000Data entity);
    Task DeleteAsync(long tKey);
}

/// <summary>
/// STD3000 資料查詢條件
/// </summary>
public class Std3000DataQuery
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

