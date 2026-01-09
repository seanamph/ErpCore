using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.SystemExtensionE;
using ErpCore.Application.Services.SystemExtensionE;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.SystemExtensionE;

/// <summary>
/// 員工資料維護控制器 (SYSPE10-SYSPE11 - 員工資料維護)
/// </summary>
[Route("api/v1/employees")]
public class EmployeeDataController : BaseController
{
    private readonly IEmployeeDataService _service;

    public EmployeeDataController(
        IEmployeeDataService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢員工列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<EmployeeDto>>>> GetEmployees(
        [FromQuery] EmployeeQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetEmployeesAsync(query);
            return result;
        }, "查詢員工列表失敗");
    }

    /// <summary>
    /// 查詢單筆員工
    /// </summary>
    [HttpGet("{employeeId}")]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> GetEmployee(string employeeId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetEmployeeByIdAsync(employeeId);
            return result;
        }, $"查詢員工失敗: {employeeId}");
    }

    /// <summary>
    /// 新增員工
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateEmployee(
        [FromBody] CreateEmployeeDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateEmployeeAsync(dto);
            return result;
        }, "新增員工失敗");
    }

    /// <summary>
    /// 修改員工
    /// </summary>
    [HttpPut("{employeeId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateEmployee(
        string employeeId,
        [FromBody] UpdateEmployeeDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateEmployeeAsync(employeeId, dto);
            return new object();
        }, $"修改員工失敗: {employeeId}");
    }

    /// <summary>
    /// 刪除員工
    /// </summary>
    [HttpDelete("{employeeId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteEmployee(string employeeId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteEmployeeAsync(employeeId);
            return new object();
        }, $"刪除員工失敗: {employeeId}");
    }
}

/// <summary>
/// 人事資料維護控制器 (SYSPED0 - 人事資料維護)
/// </summary>
[Route("api/v1/personnel")]
public class PersonnelDataController : BaseController
{
    private readonly IPersonnelDataService _service;

    public PersonnelDataController(
        IPersonnelDataService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢人事列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<PersonnelDto>>>> GetPersonnel(
        [FromQuery] PersonnelQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPersonnelAsync(query);
            return result;
        }, "查詢人事列表失敗");
    }

    /// <summary>
    /// 查詢單筆人事
    /// </summary>
    [HttpGet("{personnelId}")]
    public async Task<ActionResult<ApiResponse<PersonnelDto>>> GetPersonnel(string personnelId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPersonnelByIdAsync(personnelId);
            return result;
        }, $"查詢人事失敗: {personnelId}");
    }

    /// <summary>
    /// 新增人事
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreatePersonnel(
        [FromBody] CreatePersonnelDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreatePersonnelAsync(dto);
            return result;
        }, "新增人事失敗");
    }

    /// <summary>
    /// 修改人事
    /// </summary>
    [HttpPut("{personnelId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdatePersonnel(
        string personnelId,
        [FromBody] UpdatePersonnelDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdatePersonnelAsync(personnelId, dto);
            return new object();
        }, $"修改人事失敗: {personnelId}");
    }

    /// <summary>
    /// 刪除人事
    /// </summary>
    [HttpDelete("{personnelId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeletePersonnel(string personnelId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeletePersonnelAsync(personnelId);
            return new object();
        }, $"刪除人事失敗: {personnelId}");
    }
}

