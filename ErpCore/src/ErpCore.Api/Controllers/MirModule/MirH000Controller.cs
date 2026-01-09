using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.MirModule;
using ErpCore.Application.Services.MirModule;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.MirModule;

/// <summary>
/// MIRH000 人事資源管理模組控制器
/// </summary>
[Route("api/v1/mirh000")]
public class MirH000Controller : BaseController
{
    private readonly IMirH000PersonnelService _personnelService;
    private readonly IMirH000SalaryService _salaryService;

    public MirH000Controller(
        IMirH000PersonnelService personnelService,
        IMirH000SalaryService salaryService,
        ILoggerService logger) : base(logger)
    {
        _personnelService = personnelService;
        _salaryService = salaryService;
    }

    /// <summary>
    /// 查詢人事列表
    /// </summary>
    [HttpGet("personnel")]
    public async Task<ActionResult<ApiResponse<PagedResult<MirH000PersonnelDto>>>> GetPersonnelList(
        [FromQuery] MirH000PersonnelQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _personnelService.GetPersonnelListAsync(query);
            return result;
        }, "查詢人事列表失敗");
    }

    /// <summary>
    /// 查詢單筆人事
    /// </summary>
    [HttpGet("personnel/{personnelId}")]
    public async Task<ActionResult<ApiResponse<MirH000PersonnelDto>>> GetPersonnel(string personnelId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _personnelService.GetPersonnelByIdAsync(personnelId);
            return result;
        }, $"查詢人事資料失敗: {personnelId}");
    }

    /// <summary>
    /// 新增人事
    /// </summary>
    [HttpPost("personnel")]
    public async Task<ActionResult<ApiResponse<string>>> CreatePersonnel(
        [FromBody] CreateMirH000PersonnelDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _personnelService.CreatePersonnelAsync(dto);
            return result;
        }, "新增人事資料失敗");
    }

    /// <summary>
    /// 修改人事
    /// </summary>
    [HttpPut("personnel/{personnelId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdatePersonnel(
        string personnelId,
        [FromBody] UpdateMirH000PersonnelDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _personnelService.UpdatePersonnelAsync(personnelId, dto);
            return new object();
        }, $"修改人事資料失敗: {personnelId}");
    }

    /// <summary>
    /// 刪除人事
    /// </summary>
    [HttpDelete("personnel/{personnelId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeletePersonnel(string personnelId)
    {
        return await ExecuteAsync(async () =>
        {
            await _personnelService.DeletePersonnelAsync(personnelId);
            return new object();
        }, $"刪除人事資料失敗: {personnelId}");
    }

    /// <summary>
    /// 查詢薪資列表
    /// </summary>
    [HttpGet("salaries")]
    public async Task<ActionResult<ApiResponse<PagedResult<MirH000SalaryDto>>>> GetSalaryList(
        [FromQuery] MirH000SalaryQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _salaryService.GetSalaryListAsync(query);
            return result;
        }, "查詢薪資列表失敗");
    }

    /// <summary>
    /// 查詢單筆薪資
    /// </summary>
    [HttpGet("salaries/{salaryId}")]
    public async Task<ActionResult<ApiResponse<MirH000SalaryDto>>> GetSalary(string salaryId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _salaryService.GetSalaryByIdAsync(salaryId);
            return result;
        }, $"查詢薪資資料失敗: {salaryId}");
    }

    /// <summary>
    /// 新增薪資
    /// </summary>
    [HttpPost("salaries")]
    public async Task<ActionResult<ApiResponse<string>>> CreateSalary(
        [FromBody] CreateMirH000SalaryDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _salaryService.CreateSalaryAsync(dto);
            return result;
        }, "新增薪資資料失敗");
    }

    /// <summary>
    /// 修改薪資
    /// </summary>
    [HttpPut("salaries/{salaryId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateSalary(
        string salaryId,
        [FromBody] UpdateMirH000SalaryDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _salaryService.UpdateSalaryAsync(salaryId, dto);
            return new object();
        }, $"修改薪資資料失敗: {salaryId}");
    }

    /// <summary>
    /// 刪除薪資
    /// </summary>
    [HttpDelete("salaries/{salaryId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteSalary(string salaryId)
    {
        return await ExecuteAsync(async () =>
        {
            await _salaryService.DeleteSalaryAsync(salaryId);
            return new object();
        }, $"刪除薪資資料失敗: {salaryId}");
    }
}

