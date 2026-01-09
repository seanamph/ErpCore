using ErpCore.Domain.Entities.StandardModule;

namespace ErpCore.Infrastructure.Repositories.StandardModule;

/// <summary>
/// STD5000 基礎資料 Repository 介面 (SYS5110-SYS5150 - 基礎資料維護)
/// </summary>
public interface IStd5000BaseDataRepository
{
    Task<Std5000BaseData?> GetByIdAsync(long tKey);
    Task<Std5000BaseData?> GetByDataIdAndTypeAsync(string dataId, string dataType);
    Task<IEnumerable<Std5000BaseData>> QueryAsync(Std5000BaseDataQuery query);
    Task<int> GetCountAsync(Std5000BaseDataQuery query);
    Task<long> CreateAsync(Std5000BaseData entity);
    Task UpdateAsync(Std5000BaseData entity);
    Task DeleteAsync(long tKey);
}

/// <summary>
/// STD5000 基礎資料查詢條件
/// </summary>
public class Std5000BaseDataQuery
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

