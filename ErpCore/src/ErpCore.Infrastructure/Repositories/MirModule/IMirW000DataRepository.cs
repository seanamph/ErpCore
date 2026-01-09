using ErpCore.Domain.Entities.MirModule;

namespace ErpCore.Infrastructure.Repositories.MirModule;

/// <summary>
/// MIRW000 資料 Repository 介面
/// </summary>
public interface IMirW000DataRepository
{
    Task<MirW000Data?> GetByIdAsync(string dataId);
    Task<IEnumerable<MirW000Data>> QueryAsync(MirW000DataQuery query);
    Task<int> GetCountAsync(MirW000DataQuery query);
    Task<string> CreateAsync(MirW000Data entity);
    Task UpdateAsync(MirW000Data entity);
    Task DeleteAsync(string dataId);
}

/// <summary>
/// 查詢條件
/// </summary>
public class MirW000DataQuery
{
    public string? DataId { get; set; }
    public string? DataName { get; set; }
    public string? DataType { get; set; }
    public string? Status { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

