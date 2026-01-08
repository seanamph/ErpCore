using ErpCore.Domain.Entities.HumanResource;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.HumanResource;

/// <summary>
/// 員工 Repository 介面 (SYSH110)
/// </summary>
public interface IEmployeeRepository
{
    /// <summary>
    /// 根據員工編號查詢員工
    /// </summary>
    Task<Employee?> GetByIdAsync(string employeeId);

    /// <summary>
    /// 查詢員工列表（分頁）
    /// </summary>
    Task<PagedResult<Employee>> QueryAsync(EmployeeQuery query);

    /// <summary>
    /// 新增員工
    /// </summary>
    Task<Employee> CreateAsync(Employee employee);

    /// <summary>
    /// 修改員工
    /// </summary>
    Task<Employee> UpdateAsync(Employee employee);

    /// <summary>
    /// 刪除員工
    /// </summary>
    Task DeleteAsync(string employeeId);

    /// <summary>
    /// 檢查員工編號是否存在
    /// </summary>
    Task<bool> ExistsAsync(string employeeId);
}

/// <summary>
/// 員工查詢條件
/// </summary>
public class EmployeeQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public string? DepartmentId { get; set; }
    public string? PositionId { get; set; }
    public string? Status { get; set; }
}

