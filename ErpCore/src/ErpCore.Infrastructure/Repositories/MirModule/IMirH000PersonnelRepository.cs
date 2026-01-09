using ErpCore.Domain.Entities.MirModule;

namespace ErpCore.Infrastructure.Repositories.MirModule;

/// <summary>
/// MIRH000 人事 Repository 介面
/// </summary>
public interface IMirH000PersonnelRepository
{
    Task<MirH000Personnel?> GetByIdAsync(string personnelId);
    Task<IEnumerable<MirH000Personnel>> QueryAsync(MirH000PersonnelQuery query);
    Task<int> GetCountAsync(MirH000PersonnelQuery query);
    Task<string> CreateAsync(MirH000Personnel entity);
    Task UpdateAsync(MirH000Personnel entity);
    Task DeleteAsync(string personnelId);
}

/// <summary>
/// 查詢條件
/// </summary>
public class MirH000PersonnelQuery
{
    public string? PersonnelId { get; set; }
    public string? PersonnelName { get; set; }
    public string? DepartmentId { get; set; }
    public string? Status { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

