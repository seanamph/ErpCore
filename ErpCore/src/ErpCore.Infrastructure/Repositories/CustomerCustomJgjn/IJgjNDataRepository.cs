using ErpCore.Domain.Entities.CustomerCustomJgjn;

namespace ErpCore.Infrastructure.Repositories.CustomerCustomJgjn;

/// <summary>
/// JGJN資料 Repository 介面
/// </summary>
public interface IJgjNDataRepository
{
    Task<JgjNData?> GetByIdAsync(long tKey);
    Task<JgjNData?> GetByDataIdAndModuleCodeAsync(string dataId, string moduleCode);
    Task<IEnumerable<JgjNData>> QueryAsync(JgjNDataQuery query);
    Task<int> GetCountAsync(JgjNDataQuery query);
    Task<long> CreateAsync(JgjNData entity);
    Task UpdateAsync(JgjNData entity);
    Task DeleteAsync(long tKey);
}

/// <summary>
/// JGJN資料查詢條件
/// </summary>
public class JgjNDataQuery
{
    public string? ModuleCode { get; set; }
    public string? DataType { get; set; }
    public string? Status { get; set; }
    public string? Keyword { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

