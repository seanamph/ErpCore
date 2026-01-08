using ErpCore.Application.DTOs.HumanResource;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.HumanResource;

/// <summary>
/// 員工服務介面 (SYSH110)
/// </summary>
public interface IEmployeeService
{
    /// <summary>
    /// 查詢員工列表
    /// </summary>
    Task<PagedResult<EmployeeDto>> GetEmployeesAsync(EmployeeQueryDto query);

    /// <summary>
    /// 根據員工編號查詢員工
    /// </summary>
    Task<EmployeeDto> GetEmployeeByIdAsync(string employeeId);

    /// <summary>
    /// 新增員工
    /// </summary>
    Task<string> CreateEmployeeAsync(CreateEmployeeDto dto);

    /// <summary>
    /// 修改員工
    /// </summary>
    Task UpdateEmployeeAsync(string employeeId, UpdateEmployeeDto dto);

    /// <summary>
    /// 刪除員工
    /// </summary>
    Task DeleteEmployeeAsync(string employeeId);
}

