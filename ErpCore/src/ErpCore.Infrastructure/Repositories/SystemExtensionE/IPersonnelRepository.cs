using ErpCore.Domain.Entities.SystemExtensionE;

namespace ErpCore.Infrastructure.Repositories.SystemExtensionE;

/// <summary>
/// 人事 Repository 介面 (SYSPED0 - 人事資料維護)
/// </summary>
public interface IPersonnelRepository
{
    Task<Personnel?> GetByIdAsync(string personnelId);
    Task<IEnumerable<Personnel>> QueryAsync(PersonnelQuery query);
    Task<int> GetCountAsync(PersonnelQuery query);
    Task<string> CreateAsync(Personnel entity);
    Task UpdateAsync(Personnel entity);
    Task DeleteAsync(string personnelId);
    Task<bool> ExistsAsync(string personnelId);
}

/// <summary>
/// 人事查詢條件
/// </summary>
public class PersonnelQuery
{
    public string? PersonnelId { get; set; }
    public string? PersonnelName { get; set; }
    public string? DepartmentId { get; set; }
    public string? PositionId { get; set; }
    public string? Status { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

