using ErpCore.Domain.Entities.HumanResource;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.HumanResource;

/// <summary>
/// 薪資 Repository 介面 (SYSH210)
/// </summary>
public interface IPayrollRepository
{
    /// <summary>
    /// 根據薪資編號查詢薪資
    /// </summary>
    Task<Payroll?> GetByIdAsync(string payrollId);

    /// <summary>
    /// 根據員工編號、年度、月份查詢薪資
    /// </summary>
    Task<Payroll?> GetByEmployeeYearMonthAsync(string employeeId, int year, int month);

    /// <summary>
    /// 查詢薪資列表（分頁）
    /// </summary>
    Task<PagedResult<Payroll>> QueryAsync(PayrollQuery query);

    /// <summary>
    /// 新增薪資
    /// </summary>
    Task<Payroll> CreateAsync(Payroll payroll);

    /// <summary>
    /// 修改薪資
    /// </summary>
    Task<Payroll> UpdateAsync(Payroll payroll);

    /// <summary>
    /// 刪除薪資
    /// </summary>
    Task DeleteAsync(string payrollId);

    /// <summary>
    /// 檢查員工年度月份組合是否存在
    /// </summary>
    Task<bool> ExistsAsync(string employeeId, int year, int month);
}

/// <summary>
/// 薪資查詢條件
/// </summary>
public class PayrollQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? EmployeeId { get; set; }
    public int? PayrollYear { get; set; }
    public int? PayrollMonth { get; set; }
    public string? Status { get; set; }
}

