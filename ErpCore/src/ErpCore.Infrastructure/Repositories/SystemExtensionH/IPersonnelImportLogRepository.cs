using ErpCore.Domain.Entities.SystemExtensionH;

namespace ErpCore.Infrastructure.Repositories.SystemExtensionH;

/// <summary>
/// 人事匯入記錄 Repository 介面 (SYSH3D0_FMI - 人事批量新增)
/// </summary>
public interface IPersonnelImportLogRepository
{
    Task<PersonnelImportLog?> GetByIdAsync(string importId);
    Task<IEnumerable<PersonnelImportLog>> QueryAsync(PersonnelImportLogQuery query);
    Task<int> GetCountAsync(PersonnelImportLogQuery query);
    Task<string> CreateAsync(PersonnelImportLog entity);
    Task UpdateAsync(PersonnelImportLog entity);
    Task UpdateStatusAsync(string importId, string status);
    Task<string> GenerateImportIdAsync();
}

/// <summary>
/// 人事匯入記錄查詢條件
/// </summary>
public class PersonnelImportLogQuery
{
    public string? ImportId { get; set; }
    public string? ImportStatus { get; set; }
    public DateTime? ImportDateFrom { get; set; }
    public DateTime? ImportDateTo { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

