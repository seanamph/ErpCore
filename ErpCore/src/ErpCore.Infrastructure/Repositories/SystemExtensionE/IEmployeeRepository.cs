using ErpCore.Domain.Entities.SystemExtensionE;

namespace ErpCore.Infrastructure.Repositories.SystemExtensionE;

/// <summary>
/// 員工 Repository 介面 (SYSPE10-SYSPE11 - 員工資料維護)
/// </summary>
public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(string employeeId);
    Task<IEnumerable<Employee>> QueryAsync(EmployeeQuery query);
    Task<int> GetCountAsync(EmployeeQuery query);
    Task<string> CreateAsync(Employee entity);
    Task UpdateAsync(Employee entity);
    Task DeleteAsync(string employeeId);
    Task<bool> ExistsAsync(string employeeId);
}

/// <summary>
/// 員工查詢條件
/// </summary>
public class EmployeeQuery
{
    public string? EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public string? DepartmentId { get; set; }
    public string? PositionId { get; set; }
    public string? Status { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

