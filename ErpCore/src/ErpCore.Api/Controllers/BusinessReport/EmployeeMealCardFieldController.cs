using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.BusinessReport;
using ErpCore.Application.Services.BusinessReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.BusinessReport;

/// <summary>
/// 員餐卡欄位管理控制器 (SYSL206/SYSL207)
/// </summary>
[Route("api/v1/employee-meal-cards/fields")]
public class EmployeeMealCardFieldController : BaseController
{
    private readonly IEmployeeMealCardFieldService _service;

    public EmployeeMealCardFieldController(
        IEmployeeMealCardFieldService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢員餐卡欄位列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<EmployeeMealCardFieldDto>>>> GetFields(
        [FromQuery] EmployeeMealCardFieldQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetFieldsAsync(query);
            return result;
        }, "查詢員餐卡欄位列表失敗");
    }

    /// <summary>
    /// 根據主鍵查詢單筆資料
    /// </summary>
    [HttpGet("{tKey}")]
    public async Task<ActionResult<ApiResponse<EmployeeMealCardFieldDto>>> GetField(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetFieldAsync(tKey);
            if (result == null)
            {
                throw new Exception($"找不到員餐卡欄位資料: {tKey}");
            }
            return result;
        }, "查詢員餐卡欄位失敗");
    }

    /// <summary>
    /// 新增員餐卡欄位
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<long>>> CreateField(
        [FromBody] CreateEmployeeMealCardFieldDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var tKey = await _service.CreateFieldAsync(dto);
            return tKey;
        }, "新增員餐卡欄位失敗");
    }

    /// <summary>
    /// 修改員餐卡欄位
    /// </summary>
    [HttpPut("{tKey}")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateField(
        long tKey,
        [FromBody] UpdateEmployeeMealCardFieldDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.UpdateFieldAsync(tKey, dto);
            return result;
        }, "修改員餐卡欄位失敗");
    }

    /// <summary>
    /// 刪除員餐卡欄位
    /// </summary>
    [HttpDelete("{tKey}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteField(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.DeleteFieldAsync(tKey);
            return result;
        }, "刪除員餐卡欄位失敗");
    }

    /// <summary>
    /// 載入上一筆名稱
    /// </summary>
    [HttpGet("previous/{fieldId}")]
    public async Task<ActionResult<ApiResponse<EmployeeMealCardFieldDto>>> GetPreviousField(string fieldId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPreviousFieldAsync(fieldId);
            return result ?? new EmployeeMealCardFieldDto();
        }, "載入上一筆員餐卡欄位失敗");
    }

    /// <summary>
    /// 切換Y/N值
    /// </summary>
    [HttpPost("{tKey}/toggle-yn")]
    public async Task<ActionResult<ApiResponse<EmployeeMealCardFieldDto>>> ToggleYn(
        long tKey,
        [FromBody] ToggleYnDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.ToggleYnAsync(tKey, dto);
            return result;
        }, "切換Y/N值失敗");
    }
}

