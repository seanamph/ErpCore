using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.BasicData;
using ErpCore.Application.Services.BasicData;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.BasicData;

/// <summary>
/// 部別資料維護控制器 (SYSWB40)
/// </summary>
[Route("api/v1/departments")]
public class DepartmentsController : BaseController
{
    private readonly IDepartmentService _service;

    public DepartmentsController(
        IDepartmentService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢部別列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<DepartmentDto>>>> GetDepartments(
        [FromQuery] DepartmentQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetDepartmentsAsync(query);
            return result;
        }, "查詢部別列表失敗");
    }

    /// <summary>
    /// 查詢單筆部別
    /// </summary>
    [HttpGet("{deptId}")]
    public async Task<ActionResult<ApiResponse<DepartmentDto>>> GetDepartment(string deptId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetDepartmentAsync(deptId);
            return result;
        }, $"查詢部別失敗: {deptId}");
    }

    /// <summary>
    /// 新增部別
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateDepartment(
        [FromBody] CreateDepartmentDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateDepartmentAsync(dto);
            return result;
        }, "新增部別失敗");
    }

    /// <summary>
    /// 修改部別
    /// </summary>
    [HttpPut("{deptId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateDepartment(
        string deptId,
        [FromBody] UpdateDepartmentDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateDepartmentAsync(deptId, dto);
        }, $"修改部別失敗: {deptId}");
    }

    /// <summary>
    /// 刪除部別
    /// </summary>
    [HttpDelete("{deptId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteDepartment(string deptId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteDepartmentAsync(deptId);
        }, $"刪除部別失敗: {deptId}");
    }

    /// <summary>
    /// 批次刪除部別
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteDepartmentsBatch(
        [FromBody] BatchDeleteDepartmentDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteDepartmentsBatchAsync(dto);
        }, "批次刪除部別失敗");
    }

    /// <summary>
    /// 更新部別狀態
    /// </summary>
    [HttpPut("{deptId}/status")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateDepartmentStatus(
        string deptId,
        [FromBody] UpdateDepartmentStatusDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateStatusAsync(deptId, dto.Status);
        }, $"更新部別狀態失敗: {deptId}");
    }
}
