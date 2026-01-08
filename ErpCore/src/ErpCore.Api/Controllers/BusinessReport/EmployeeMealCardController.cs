using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.BusinessReport;
using ErpCore.Application.Services.BusinessReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.BusinessReport;

/// <summary>
/// 員工餐卡申請維護作業控制器 (SYSL130)
/// </summary>
[Route("api/v1/business-reports/meal-cards")]
public class EmployeeMealCardController : BaseController
{
    private readonly IEmployeeMealCardService _service;

    public EmployeeMealCardController(
        IEmployeeMealCardService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢員工餐卡申請列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<EmployeeMealCardDto>>>> GetMealCards(
        [FromQuery] EmployeeMealCardQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetMealCardsAsync(query);
            return result;
        }, "查詢員工餐卡申請列表失敗");
    }

    /// <summary>
    /// 查詢單筆員工餐卡申請
    /// </summary>
    [HttpGet("{tKey}")]
    public async Task<ActionResult<ApiResponse<EmployeeMealCardDto>>> GetMealCard(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetMealCardByIdAsync(tKey);
            return result;
        }, $"查詢員工餐卡申請失敗: {tKey}");
    }

    /// <summary>
    /// 新增員工餐卡申請
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<long>>> CreateMealCard(
        [FromBody] CreateEmployeeMealCardDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateMealCardAsync(dto);
            return result;
        }, "新增員工餐卡申請失敗");
    }

    /// <summary>
    /// 修改員工餐卡申請
    /// </summary>
    [HttpPut("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateMealCard(
        long tKey,
        [FromBody] UpdateEmployeeMealCardDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateMealCardAsync(tKey, dto);
        }, $"修改員工餐卡申請失敗: {tKey}");
    }

    /// <summary>
    /// 刪除員工餐卡申請
    /// </summary>
    [HttpDelete("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteMealCard(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteMealCardAsync(tKey);
        }, $"刪除員工餐卡申請失敗: {tKey}");
    }

    /// <summary>
    /// 批次審核員工餐卡申請
    /// </summary>
    [HttpPost("batch-verify")]
    public async Task<ActionResult<ApiResponse<BatchVerifyResultDto>>> BatchVerify(
        [FromBody] BatchVerifyDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.BatchVerifyAsync(dto);
            return result;
        }, "批次審核員工餐卡申請失敗");
    }

    /// <summary>
    /// 取得下拉選單資料
    /// </summary>
    [HttpGet("dropdowns")]
    public async Task<ActionResult<ApiResponse<MealCardDropdownsDto>>> GetDropdowns()
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetDropdownsAsync();
            return result;
        }, "取得下拉選單資料失敗");
    }
}

