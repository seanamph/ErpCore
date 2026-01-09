using ErpCore.Domain.Entities.MirModule;

namespace ErpCore.Infrastructure.Repositories.MirModule;

/// <summary>
/// MIRV000 資料 Repository 介面
/// </summary>
public interface IMirV000DataRepository
{
    Task<MirV000Data?> GetByIdAsync(string dataId);
    Task<IEnumerable<MirV000Data>> QueryAsync(MirV000DataQuery query);
    Task<int> GetCountAsync(MirV000DataQuery query);
    Task<string> CreateAsync(MirV000Data entity);
    Task UpdateAsync(MirV000Data entity);
    Task DeleteAsync(string dataId);
}

/// <summary>
/// 查詢條件
/// </summary>
public class MirV000DataQuery
{
    public string? DataId { get; set; }
    public string? DataName { get; set; }
    public string? DataType { get; set; }
    public string? Status { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

