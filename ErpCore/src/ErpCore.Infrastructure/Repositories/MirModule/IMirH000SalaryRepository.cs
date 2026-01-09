using ErpCore.Domain.Entities.MirModule;

namespace ErpCore.Infrastructure.Repositories.MirModule;

/// <summary>
/// MIRH000 薪資 Repository 介面
/// </summary>
public interface IMirH000SalaryRepository
{
    Task<MirH000Salary?> GetByIdAsync(string salaryId);
    Task<IEnumerable<MirH000Salary>> QueryAsync(MirH000SalaryQuery query);
    Task<int> GetCountAsync(MirH000SalaryQuery query);
    Task<string> CreateAsync(MirH000Salary entity);
    Task UpdateAsync(MirH000Salary entity);
    Task DeleteAsync(string salaryId);
}

/// <summary>
/// 查詢條件
/// </summary>
public class MirH000SalaryQuery
{
    public string? SalaryId { get; set; }
    public string? PersonnelId { get; set; }
    public string? SalaryMonth { get; set; }
    public string? Status { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

