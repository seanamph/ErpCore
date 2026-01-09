using ErpCore.Domain.Entities.MshModule;

namespace ErpCore.Infrastructure.Repositories.MshModule;

/// <summary>
/// MSH3000 資料 Repository 介面
/// </summary>
public interface IMsh3000DataRepository
{
    Task<Msh3000Data?> GetByIdAsync(string dataId);
    Task<IEnumerable<Msh3000Data>> QueryAsync(Msh3000DataQuery query);
    Task<int> GetCountAsync(Msh3000DataQuery query);
    Task<string> CreateAsync(Msh3000Data entity);
    Task UpdateAsync(Msh3000Data entity);
    Task DeleteAsync(string dataId);
}

/// <summary>
/// 查詢條件
/// </summary>
public class Msh3000DataQuery
{
    public string? DataId { get; set; }
    public string? DataName { get; set; }
    public string? DataType { get; set; }
    public string? Status { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

