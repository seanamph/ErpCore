using ErpCore.Application.DTOs.SystemExtensionE;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.SystemExtensionE;

/// <summary>
/// 員工資料服務介面 (SYSPE10-SYSPE11 - 員工資料維護)
/// </summary>
public interface IEmployeeDataService
{
    Task<PagedResult<EmployeeDto>> GetEmployeesAsync(EmployeeQueryDto query);
    Task<EmployeeDto> GetEmployeeByIdAsync(string employeeId);
    Task<string> CreateEmployeeAsync(CreateEmployeeDto dto);
    Task UpdateEmployeeAsync(string employeeId, UpdateEmployeeDto dto);
    Task DeleteEmployeeAsync(string employeeId);
}

/// <summary>
/// 人事資料服務介面 (SYSPED0 - 人事資料維護)
/// </summary>
public interface IPersonnelDataService
{
    Task<PagedResult<PersonnelDto>> GetPersonnelAsync(PersonnelQueryDto query);
    Task<PersonnelDto> GetPersonnelByIdAsync(string personnelId);
    Task<string> CreatePersonnelAsync(CreatePersonnelDto dto);
    Task UpdatePersonnelAsync(string personnelId, UpdatePersonnelDto dto);
    Task DeletePersonnelAsync(string personnelId);
}

