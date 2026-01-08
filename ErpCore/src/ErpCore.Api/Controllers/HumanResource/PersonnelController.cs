using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.HumanResource;
using ErpCore.Application.Services.HumanResource;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.HumanResource;

/// <summary>
/// 人事管理控制器 (SYSH110)
/// 提供員工基本資料維護功能
/// </summary>
[Route("api/v1/human-resource/personnel")]
public class PersonnelController : BaseController
{
    private readonly IEmployeeService _service;

    public PersonnelController(
        IEmployeeService service,
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
        }, $"刪除員工失敗: {employeeId}");
    }
}

